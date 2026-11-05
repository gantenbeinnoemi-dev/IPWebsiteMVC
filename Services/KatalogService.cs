using IPWebsiteMVC.Data;
using IPWebsiteMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace IPWebsiteMVC.Services;

public class KatalogService(MyBooksDbContext db)
{
    public static string NormalizeFilter(string? filter) => filter?.ToLowerInvariant() switch
    {
        "autor" => "autor",
        "alter" => "alter",
        "benutzer" => "benutzer",
        _ => "genre"
    };

    public static string FilterLabel(string filter) => filter switch
    {
        "autor" => "Autor",
        "alter" => "Altersklasse",
        "benutzer" => "Benutzerdefiniert",
        _ => "Genre"
    };

    // ---------------------------------------------------------------- Benutzerdefinierte Kategorien

    /// <summary>Alle Kategorien des Benutzers (auch leere), Bücher in der Reihenfolge des Hinzufügens.</summary>
    public async Task<IEnumerable<BuchGruppe>> GetKategorienAsync(int benutzerId)
    {
        var kategorien = await db.Kategorien
            .Where(k => k.BenutzerID == benutzerId)
            .Include(k => k.Eintraege).ThenInclude(e => e.Buch)
            .OrderBy(k => k.ErstelltAm).ThenBy(k => k.KategorieID)
            .ToListAsync();
        return kategorien.Select(KategorieZuGruppe);
    }

    public async Task<BuchGruppe?> GetKategorieAsync(int benutzerId, int id)
    {
        var kategorie = await db.Kategorien
            .Where(k => k.BenutzerID == benutzerId && k.KategorieID == id)
            .Include(k => k.Eintraege).ThenInclude(e => e.Buch)
            .FirstOrDefaultAsync();
        return kategorie is null ? null : KategorieZuGruppe(kategorie);
    }

    /// <summary>Nur Id und Name, für die Reiter in der Navigation.</summary>
    public async Task<IEnumerable<(int Id, string Name)>> GetKategorieNamenAsync(int benutzerId)
    {
        var zeilen = await db.Kategorien
            .Where(k => k.BenutzerID == benutzerId)
            .OrderBy(k => k.ErstelltAm).ThenBy(k => k.KategorieID)
            .Select(k => new { k.KategorieID, k.Name })
            .ToListAsync();
        return zeilen.Select(z => (z.KategorieID, z.Name));
    }

    private static BuchGruppe KategorieZuGruppe(Kategorie k) =>
        new(k.KategorieID, k.Name,
            k.Eintraege.OrderBy(e => e.HinzugefuegtAm).ThenBy(e => e.BuchID).Select(e => e.Buch!));

    public async Task<IEnumerable<BuchGruppe>> GetGruppenAsync(string filter)
    {
        switch (filter)
        {
            case "autor":
                var autoren = await db.Autoren.Include(a => a.Buecher).OrderBy(a => a.Nachname).ToListAsync();
                return autoren.Where(a => a.Buecher.Count > 0)
                    .Select(a => new BuchGruppe(a.AutorID, a.VollerName, Sortiert(a.Buecher)));

            case "alter":
                var alter = await db.Altersklassifikationen.Include(a => a.Buecher).OrderBy(a => a.AltersklassifikationID).ToListAsync();
                return alter.Where(a => a.Buecher.Count > 0)
                    .Select(a => new BuchGruppe(a.AltersklassifikationID, a.Bezeichnung, Sortiert(a.Buecher)));

            default:
                var genres = await db.Genres.Include(g => g.Buecher).OrderBy(g => g.GenreID).ToListAsync();
                return genres.Where(g => g.Buecher.Count > 0)
                    .Select(g => new BuchGruppe(g.GenreID, g.Bezeichnung, Sortiert(g.Buecher)));
        }
    }

    public async Task<BuchGruppe?> GetGruppeAsync(string filter, int id) =>
        (await GetGruppenAsync(filter)).FirstOrDefault(g => g.Id == id);

    public Task<Buch?> GetBuchAsync(int id) =>
        db.Buecher
            .Include(b => b.Autor)
            .Include(b => b.Genres)
            .Include(b => b.Altersklassifikationen)
            .FirstOrDefaultAsync(b => b.BuchID == id);

    private static IEnumerable<Buch> Sortiert(IEnumerable<Buch> buecher) =>
        buecher.OrderBy(b => b.BuchID);
}
