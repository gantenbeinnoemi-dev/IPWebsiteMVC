using IPWebsiteMVC.Data;
using IPWebsiteMVC.Models;
using IPWebsiteMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IPWebsiteMVC.Controllers;

/// <summary>Eigene (benutzerdefinierte) Kategorien. Jeder Benutzer sieht und ändert nur seine eigenen.</summary>
[Authorize]
public class KategorieController(MyBooksDbContext db) : Controller
{
    private int BenutzerId => User.BenutzerId() ?? throw new InvalidOperationException("Nicht angemeldet.");

    // ---------------------------------------------------------------- Neue Kategorie (Formular)

    [HttpGet]
    public Task<IActionResult> Neu() => FormularZeigen(new KategorieFormModel());

    /// <summary>"Hinzufügen": legt die Kategorie samt der ausgewählten Bücher endgültig an.</summary>
    [HttpPost]
    public async Task<IActionResult> Neu(KategorieFormModel model)
    {
        model.Name = (model.Name ?? "").Trim();
        model.BuchIds = model.BuchIds.Distinct().ToList();

        if (ModelState.IsValid && await db.Kategorien.AnyAsync(k => k.BenutzerID == BenutzerId && k.Name == model.Name))
            ModelState.AddModelError(nameof(model.Name), "Du hast bereits eine Kategorie mit diesem Namen.");
        if (!ModelState.IsValid) return await FormularZeigen(model);

        var vorhandene = await db.Buecher.Where(b => model.BuchIds.Contains(b.BuchID)).Select(b => b.BuchID).ToListAsync();
        var jetzt = DateTime.Now;
        var kategorie = new Kategorie { BenutzerID = BenutzerId, Name = model.Name, ErstelltAm = jetzt };
        foreach (var id in model.BuchIds.Where(vorhandene.Contains))
            kategorie.Eintraege.Add(new KategorieBuch { BuchID = id, HinzugefuegtAm = jetzt });
        db.Kategorien.Add(kategorie);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError("", "Die Kategorie konnte nicht gespeichert werden. Bitte melde dich neu an und versuche es nochmal.");
            return await FormularZeigen(model);
        }

        return RedirectToAction("Liste", "Home", new { filter = "benutzer", id = kategorie.KategorieID });
    }

    /// <summary>Zurück zum Formular (z.B. nach "Abbrechen" in der Bücherauswahl), ohne etwas zu speichern.</summary>
    [HttpPost]
    public Task<IActionResult> NeuFormular(KategorieFormModel model)
    {
        ModelState.Clear();
        return FormularZeigen(model);
    }

    /// <summary>Bücherauswahl für eine noch nicht angelegte Kategorie: Name und bisherige Bücher werden durchgereicht.</summary>
    [HttpPost]
    public async Task<IActionResult> NeuBuecher(KategorieFormModel model)
    {
        ViewData["Filter"] = "benutzer";
        ViewData["NeuAktiv"] = true;
        return View("BuecherAuswahl", await AuswahlErstellen(null, (model.Name ?? "").Trim(), model.BuchIds.Distinct().ToList()));
    }

    /// <summary>Bestätigung der Auswahl für eine noch nicht angelegte Kategorie: zurück zum Formular mit den neuen Büchern.</summary>
    [HttpPost]
    public Task<IActionResult> NeuUebernehmen(KategorieFormModel model, List<int> neueIds)
    {
        model.BuchIds = model.BuchIds.Concat(neueIds).Distinct().ToList();
        ModelState.Clear();
        return FormularZeigen(model);
    }

    private async Task<IActionResult> FormularZeigen(KategorieFormModel model)
    {
        model.Buecher = await BuecherInReihenfolge(model.BuchIds);
        model.BuchIds = model.Buecher.Select(b => b.BuchID).ToList();
        ViewData["Filter"] = "benutzer";
        ViewData["NeuAktiv"] = true;
        return View("Neu", model);
    }

    // ---------------------------------------------------------------- Bücher zu bestehender Kategorie

    [HttpGet]
    public async Task<IActionResult> BuecherHinzufuegen(int id)
    {
        var kategorie = await EigeneKategorie(id, mitBuechern: true);
        if (kategorie is null) return NotFound();

        ViewData["Filter"] = "benutzer";
        ViewData["ActiveId"] = id;
        var bereits = kategorie.Eintraege.Select(e => e.BuchID).ToList();
        return View("BuecherAuswahl", await AuswahlErstellen(id, kategorie.Name, bereits));
    }

    [HttpPost]
    public async Task<IActionResult> BuecherHinzufuegen(int id, List<int> neueIds)
    {
        var kategorie = await EigeneKategorie(id, mitBuechern: true);
        if (kategorie is null) return NotFound();

        var bereits = kategorie.Eintraege.Select(e => e.BuchID).ToHashSet();
        var gueltig = await db.Buecher.Where(b => neueIds.Contains(b.BuchID)).Select(b => b.BuchID).ToListAsync();
        var jetzt = DateTime.Now;
        foreach (var buchId in gueltig.Where(b => !bereits.Contains(b)))
            kategorie.Eintraege.Add(new KategorieBuch { KategorieID = id, BuchID = buchId, HinzugefuegtAm = jetzt });
        await db.SaveChangesAsync();

        return RedirectToAction("Liste", "Home", new { filter = "benutzer", id });
    }

    // ---------------------------------------------------------------- Entfernen / Löschen

    [HttpPost]
    public async Task<IActionResult> BuchEntfernen(int id, int buchId)
    {
        if (await EigeneKategorie(id, mitBuechern: false) is null) return NotFound();

        var eintrag = await db.KategorieBuecher.FirstOrDefaultAsync(e => e.KategorieID == id && e.BuchID == buchId);
        if (eintrag is not null)
        {
            db.KategorieBuecher.Remove(eintrag);
            await db.SaveChangesAsync();
        }
        return RedirectToAction("Liste", "Home", new { filter = "benutzer", id });
    }

    [HttpPost]
    public async Task<IActionResult> Loeschen(int id)
    {
        var kategorie = await EigeneKategorie(id, mitBuechern: false);
        if (kategorie is null) return NotFound();

        db.Kategorien.Remove(kategorie);
        await db.SaveChangesAsync();
        return RedirectToAction("Index", "Home", new { filter = "benutzer" });
    }

    // ---------------------------------------------------------------- Hilfsmethoden

    private Task<Kategorie?> EigeneKategorie(int id, bool mitBuechern)
    {
        var abfrage = db.Kategorien.Where(k => k.KategorieID == id && k.BenutzerID == BenutzerId);
        if (mitBuechern) abfrage = abfrage.Include(k => k.Eintraege);
        return abfrage.FirstOrDefaultAsync();
    }

    private async Task<List<Buch>> BuecherInReihenfolge(List<int> ids)
    {
        var gefunden = await db.Buecher.Where(b => ids.Contains(b.BuchID)).ToListAsync();
        return ids.Distinct().Select(id => gefunden.FirstOrDefault(b => b.BuchID == id)).OfType<Buch>().ToList();
    }

    private async Task<BuchAuswahlModel> AuswahlErstellen(int? kategorieId, string name, List<int> bereits)
    {
        var buecher = await db.Buecher.Include(b => b.Autor).ToListAsync();
        return new BuchAuswahlModel
        {
            KategorieId = kategorieId,
            KategorieName = name,
            BereitsIds = bereits,
            Buecher = buecher
                .OrderBy(b => b.Autor is null ? 1 : 0)
                .ThenBy(b => b.Autor?.Nachname)
                .ThenBy(b => b.Autor?.Vorname)
                .ThenBy(b => b.Reihe)
                .ThenBy(b => b.Band)
                .ThenBy(b => b.Titel)
                .ToList()
        };
    }
}
