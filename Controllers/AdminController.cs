using IPWebsiteMVC.Data;
using IPWebsiteMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IPWebsiteMVC.Controllers;

[Authorize(Roles = Rollen.Admin)]
public class AdminController(MyBooksDbContext db, IWebHostEnvironment env) : Controller
{
    private const long MaxCoverBytes = 5 * 1024 * 1024;
    private const string UploadPrefix = "/images/covers/upload-";

    // ---------------------------------------------------------------- Uebersicht

    public async Task<IActionResult> Index() =>
        View(new AdminUebersichtModel(
            await db.Buecher.CountAsync(), await db.Autoren.CountAsync(), await db.Laender.CountAsync(),
            await db.Genres.CountAsync(), await db.Altersklassifikationen.CountAsync()));

    // ---------------------------------------------------------------- Buecher

    public async Task<IActionResult> Buecher(string? q)
    {
        var query = db.Buecher.Include(b => b.Autor).Include(b => b.Genres).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var t = q.Trim();
            query = query.Where(b => b.Titel.Contains(t)
                || (b.Reihe != null && b.Reihe.Contains(t))
                || (b.Autor != null && (b.Autor.Vorname.Contains(t) || b.Autor.Nachname.Contains(t))));
        }
        ViewData["Q"] = q;
        return View(await query.OrderByDescending(b => b.BuchID).ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> BuchNeu() =>
        View("BuchForm", await ListenBefuellen(new BuchFormModel()));

    [HttpPost]
    [RequestSizeLimit(12_000_000)]
    public Task<IActionResult> BuchNeu(BuchFormModel model) => BuchSpeichern(model, null);

    [HttpGet]
    public async Task<IActionResult> BuchBearbeiten(int id)
    {
        var buch = await db.Buecher.Include(b => b.Genres).Include(b => b.Altersklassifikationen)
            .FirstOrDefaultAsync(b => b.BuchID == id);
        if (buch is null) return NotFound();

        var model = new BuchFormModel
        {
            BuchID = buch.BuchID,
            Titel = buch.Titel,
            Reihe = buch.Reihe,
            Band = buch.Band,
            AnzahlKapitel = buch.AnzahlKapitel,
            ErscheinungsJahr = buch.ErscheinungsJahr,
            AutorID = buch.AutorID,
            Beschreibung = buch.Beschreibung,
            AktuellesCover = buch.CoverBildUrl,
            GenreIds = buch.Genres.Select(g => g.GenreID).ToList(),
            AlterIds = buch.Altersklassifikationen.Select(a => a.AltersklassifikationID).ToList()
        };
        return View("BuchForm", await ListenBefuellen(model));
    }

    [HttpPost]
    [RequestSizeLimit(12_000_000)]
    public Task<IActionResult> BuchBearbeiten(int id, BuchFormModel model) => BuchSpeichern(model, id);

    [HttpPost]
    public async Task<IActionResult> BuchLoeschen(int id)
    {
        var buch = await db.Buecher.Include(b => b.Genres).Include(b => b.Altersklassifikationen)
            .FirstOrDefaultAsync(b => b.BuchID == id);
        if (buch is null) return NotFound();

        var titel = buch.Titel;
        var cover = buch.CoverBildUrl;
        db.Buecher.Remove(buch);
        await db.SaveChangesAsync();
        await UploadLoeschenFallsUnbenutzt(cover);

        TempData["Meldung"] = $"Das Buch „{titel}“ wurde gelöscht.";
        return RedirectToAction(nameof(Buecher));
    }

    private async Task<IActionResult> BuchSpeichern(BuchFormModel model, int? id)
    {
        Buch? buch = null;
        if (id is not null)
        {
            buch = await db.Buecher.Include(b => b.Genres).Include(b => b.Altersklassifikationen)
                .FirstOrDefaultAsync(b => b.BuchID == id);
            if (buch is null) return NotFound();
        }
        model.BuchID = id ?? 0;

        // Cover pruefen
        byte[]? bild = null;
        string? bildEndung = null;
        string? adresse = string.IsNullOrWhiteSpace(model.CoverBildUrl) ? null : model.CoverBildUrl.Trim();

        if (model.CoverDatei is { Length: > 0 } datei)
        {
            if (datei.Length > MaxCoverBytes)
            {
                ModelState.AddModelError(nameof(model.CoverDatei), "Das Bild ist zu gross (maximal 5 MB).");
            }
            else
            {
                using var ms = new MemoryStream();
                await datei.CopyToAsync(ms);
                bild = ms.ToArray();
                bildEndung = ErkenneBildEndung(bild);
                if (bildEndung is null)
                    ModelState.AddModelError(nameof(model.CoverDatei), "Diese Datei ist kein gültiges Bild (erlaubt: JPG, PNG, WebP, GIF).");
            }
        }
        else if (adresse is not null && !GueltigeCoverAdresse(adresse))
        {
            ModelState.AddModelError(nameof(model.CoverBildUrl), "Bitte eine Adresse mit http:// oder https:// angeben, oder einen Pfad wie /images/covers/bild.jpg.");
        }

        if (model.AutorID is not null && !await db.Autoren.AnyAsync(a => a.AutorID == model.AutorID))
            ModelState.AddModelError(nameof(model.AutorID), "Dieser Autor existiert nicht.");

        if (!ModelState.IsValid)
        {
            model.AktuellesCover = buch?.CoverBildUrl;
            return View("BuchForm", await ListenBefuellen(model));
        }

        var neu = buch is null;
        buch ??= new Buch();
        buch.Titel = model.Titel.Trim();
        buch.Reihe = LeerZuNull(model.Reihe);
        buch.Band = model.Band;
        buch.AnzahlKapitel = model.AnzahlKapitel;
        buch.ErscheinungsJahr = model.ErscheinungsJahr;
        buch.AutorID = model.AutorID;
        buch.Beschreibung = LeerZuNull(model.Beschreibung);

        var altesCover = buch.CoverBildUrl;
        string? neuesCover = altesCover;
        string? neueDatei = null;
        if (bild is not null && bildEndung is not null)
        {
            var name = $"upload-{Guid.NewGuid():N}{bildEndung}";
            neueDatei = Path.Combine(CoverOrdner(), name);
            await System.IO.File.WriteAllBytesAsync(neueDatei, bild);
            neuesCover = "/images/covers/" + name;
        }
        else if (adresse is not null)
        {
            neuesCover = adresse;
        }
        else if (model.CoverEntfernen)
        {
            neuesCover = null;
        }
        buch.CoverBildUrl = neuesCover;

        buch.Genres.Clear();
        if (model.GenreIds.Count > 0)
            buch.Genres.AddRange(await db.Genres.Where(g => model.GenreIds.Contains(g.GenreID)).ToListAsync());
        buch.Altersklassifikationen.Clear();
        if (model.AlterIds.Count > 0)
            buch.Altersklassifikationen.AddRange(await db.Altersklassifikationen
                .Where(a => model.AlterIds.Contains(a.AltersklassifikationID)).ToListAsync());

        if (neu) db.Buecher.Add(buch);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (neueDatei is not null) System.IO.File.Delete(neueDatei);
            ModelState.AddModelError("", "Das Buch konnte nicht gespeichert werden. Bitte prüfe die Eingaben.");
            model.AktuellesCover = altesCover;
            return View("BuchForm", await ListenBefuellen(model));
        }

        if (altesCover != neuesCover) await UploadLoeschenFallsUnbenutzt(altesCover);

        TempData["Meldung"] = neu
            ? $"Das Buch „{buch.Titel}“ wurde hinzugefügt."
            : $"Das Buch „{buch.Titel}“ wurde gespeichert.";
        return RedirectToAction(nameof(Buecher));
    }

    private async Task<BuchFormModel> ListenBefuellen(BuchFormModel model)
    {
        model.Autoren = await db.Autoren.OrderBy(a => a.Nachname).ThenBy(a => a.Vorname).ToListAsync();
        model.Genres = await db.Genres.OrderBy(g => g.GenreID).ToListAsync();
        model.Altersklassen = await db.Altersklassifikationen.OrderBy(a => a.AltersklassifikationID).ToListAsync();
        return model;
    }

    // ---------------------------------------------------------------- Autoren

    public async Task<IActionResult> Autoren()
    {
        var zeilen = await db.Autoren
            .OrderBy(a => a.Nachname).ThenBy(a => a.Vorname)
            .Select(a => new AutorZeile(a.AutorID, a.Vorname, a.Nachname, a.LandID, a.Buecher.Count))
            .ToListAsync();
        return View(new AutorenModel(zeilen, await db.Laender.OrderBy(l => l.Name).ToListAsync()));
    }

    [HttpPost]
    public async Task<IActionResult> AutorSpeichern(int? id, string? vorname, string? nachname, int? landId)
    {
        vorname = vorname?.Trim() ?? "";
        nachname = nachname?.Trim();
        if (string.IsNullOrEmpty(nachname)) return FehlerZu(nameof(Autoren), "Bitte gib mindestens einen Nachnamen ein.");
        if (vorname.Length > 100 || nachname.Length > 100) return FehlerZu(nameof(Autoren), "Vor- und Nachname dürfen höchstens 100 Zeichen lang sein.");
        if (landId is not null && !await db.Laender.AnyAsync(l => l.LandID == landId))
            return FehlerZu(nameof(Autoren), "Dieses Land existiert nicht.");

        if (id is null)
        {
            db.Autoren.Add(new Autor { Vorname = vorname, Nachname = nachname, LandID = landId });
        }
        else
        {
            var autor = await db.Autoren.FindAsync(id);
            if (autor is null) return NotFound();
            autor.Vorname = vorname;
            autor.Nachname = nachname;
            autor.LandID = landId;
        }
        await db.SaveChangesAsync();

        TempData["Meldung"] = id is null ? $"Der Autor „{vorname} {nachname}“ wurde hinzugefügt." : $"Der Autor „{vorname} {nachname}“ wurde gespeichert.";
        return RedirectToAction(nameof(Autoren));
    }

    [HttpPost]
    public async Task<IActionResult> AutorLoeschen(int id)
    {
        var autor = await db.Autoren.Include(a => a.Buecher).FirstOrDefaultAsync(a => a.AutorID == id);
        if (autor is null) return NotFound();
        if (autor.Buecher.Count > 0)
            return FehlerZu(nameof(Autoren), $"„{autor.VollerName}“ kann nicht gelöscht werden, weil noch {autor.Buecher.Count} Buch/Bücher zugeordnet sind. Lösche oder ändere zuerst diese Bücher.");

        db.Autoren.Remove(autor);
        await db.SaveChangesAsync();
        TempData["Meldung"] = $"Der Autor „{autor.VollerName}“ wurde gelöscht.";
        return RedirectToAction(nameof(Autoren));
    }

    // ---------------------------------------------------------------- Laender, Genres, Altersklassen

    public async Task<IActionResult> Stammdaten(string typ)
    {
        StammdatenModel? model = typ switch
        {
            "land" => new("land", "Länder", "Land", "Autoren",
                await db.Laender.OrderBy(x => x.LandID)
                    .Select(x => new StammdatenZeile(x.LandID, x.Name, x.Autoren.Count)).ToListAsync()),
            "genre" => new("genre", "Genres", "Genre", "Bücher",
                await db.Genres.OrderBy(x => x.GenreID)
                    .Select(x => new StammdatenZeile(x.GenreID, x.Bezeichnung, x.Buecher.Count)).ToListAsync()),
            "alter" => new("alter", "Altersklassen", "Altersklasse", "Bücher",
                await db.Altersklassifikationen.OrderBy(x => x.AltersklassifikationID)
                    .Select(x => new StammdatenZeile(x.AltersklassifikationID, x.Bezeichnung, x.Buecher.Count)).ToListAsync()),
            _ => null
        };
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    public async Task<IActionResult> StammdatenSpeichern(string typ, int? id, string? text)
    {
        text = text?.Trim();
        var max = typ == "alter" ? 50 : 100;
        if (string.IsNullOrEmpty(text)) return FehlerZu(nameof(Stammdaten), "Bitte gib einen Namen ein.", typ);
        if (text.Length > max) return FehlerZu(nameof(Stammdaten), $"Der Name darf höchstens {max} Zeichen lang sein.", typ);
        IActionResult Vergeben() => FehlerZu(nameof(Stammdaten), $"„{text}“ gibt es schon.", typ);

        switch (typ)
        {
            case "land":
                if (await db.Laender.AnyAsync(x => x.Name == text && x.LandID != (id ?? 0))) return Vergeben();
                if (id is null) db.Laender.Add(new Land { Name = text });
                else if (await db.Laender.FindAsync(id) is { } land) land.Name = text;
                else return NotFound();
                break;
            case "genre":
                if (await db.Genres.AnyAsync(x => x.Bezeichnung == text && x.GenreID != (id ?? 0))) return Vergeben();
                if (id is null) db.Genres.Add(new Genre { Bezeichnung = text });
                else if (await db.Genres.FindAsync(id) is { } genre) genre.Bezeichnung = text;
                else return NotFound();
                break;
            case "alter":
                if (await db.Altersklassifikationen.AnyAsync(x => x.Bezeichnung == text && x.AltersklassifikationID != (id ?? 0))) return Vergeben();
                if (id is null) db.Altersklassifikationen.Add(new Altersklassifikation { Bezeichnung = text });
                else if (await db.Altersklassifikationen.FindAsync(id) is { } alter) alter.Bezeichnung = text;
                else return NotFound();
                break;
            default:
                return NotFound();
        }

        await db.SaveChangesAsync();
        TempData["Meldung"] = id is null ? $"„{text}“ wurde hinzugefügt." : $"„{text}“ wurde gespeichert.";
        return RedirectToAction(nameof(Stammdaten), new { typ });
    }

    [HttpPost]
    public async Task<IActionResult> StammdatenLoeschen(string typ, int id)
    {
        string name;
        switch (typ)
        {
            case "land":
                var land = await db.Laender.Include(x => x.Autoren).FirstOrDefaultAsync(x => x.LandID == id);
                if (land is null) return NotFound();
                if (land.Autoren.Count > 0)
                    return FehlerZu(nameof(Stammdaten), $"„{land.Name}“ kann nicht gelöscht werden, weil noch {land.Autoren.Count} Autor(en) dieses Land haben.", typ);
                name = land.Name;
                db.Laender.Remove(land);
                break;
            case "genre":
                var genre = await db.Genres.Include(x => x.Buecher).FirstOrDefaultAsync(x => x.GenreID == id);
                if (genre is null) return NotFound();
                name = genre.Bezeichnung;
                db.Genres.Remove(genre);
                break;
            case "alter":
                var alter = await db.Altersklassifikationen.Include(x => x.Buecher).FirstOrDefaultAsync(x => x.AltersklassifikationID == id);
                if (alter is null) return NotFound();
                name = alter.Bezeichnung;
                db.Altersklassifikationen.Remove(alter);
                break;
            default:
                return NotFound();
        }

        await db.SaveChangesAsync();
        TempData["Meldung"] = $"„{name}“ wurde gelöscht.";
        return RedirectToAction(nameof(Stammdaten), new { typ });
    }

    // ---------------------------------------------------------------- Hilfsmethoden

    private IActionResult FehlerZu(string aktion, string meldung, string? typ = null)
    {
        TempData["Fehler"] = meldung;
        return typ is null ? RedirectToAction(aktion) : RedirectToAction(aktion, new { typ });
    }

    private static string? LeerZuNull(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

    private static bool GueltigeCoverAdresse(string url)
    {
        if (url.Contains('\\') || url.Contains("..")) return false;
        if (url.StartsWith('/')) return !url.StartsWith("//");
        return Uri.TryCreate(url, UriKind.Absolute, out var u) && (u.Scheme == Uri.UriSchemeHttp || u.Scheme == Uri.UriSchemeHttps);
    }

    private static string? ErkenneBildEndung(byte[] b)
    {
        if (b.Length >= 3 && b[0] == 0xFF && b[1] == 0xD8 && b[2] == 0xFF) return ".jpg";
        if (b.Length >= 8 && b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47 && b[4] == 0x0D && b[5] == 0x0A && b[6] == 0x1A && b[7] == 0x0A) return ".png";
        if (b.Length >= 12 && b[0] == 'R' && b[1] == 'I' && b[2] == 'F' && b[3] == 'F' && b[8] == 'W' && b[9] == 'E' && b[10] == 'B' && b[11] == 'P') return ".webp";
        if (b.Length >= 4 && b[0] == 'G' && b[1] == 'I' && b[2] == 'F' && b[3] == '8') return ".gif";
        return null;
    }

    private string CoverOrdner()
    {
        var ordner = Path.Combine(env.WebRootPath, "images", "covers");
        Directory.CreateDirectory(ordner);
        return ordner;
    }

    /// <summary>Loescht nur Dateien, die ueber die Website hochgeladen wurden (Prefix upload-) und nirgends mehr verwendet werden.</summary>
    private async Task UploadLoeschenFallsUnbenutzt(string? url)
    {
        if (url is null || !url.StartsWith(UploadPrefix)) return;
        if (await db.Buecher.AnyAsync(b => b.CoverBildUrl == url)) return;

        var datei = Path.GetFullPath(Path.Combine(CoverOrdner(), Path.GetFileName(url)));
        if (datei.StartsWith(Path.GetFullPath(CoverOrdner())) && System.IO.File.Exists(datei))
            System.IO.File.Delete(datei);
    }
}
