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
        _ => "genre"
    };

    public static string FilterLabel(string filter) => filter switch
    {
        "autor" => "Autor",
        "alter" => "Altersklasse",
        _ => "Genre"
    };

    public async Task<List<BuchGruppe>> GetGruppenAsync(string filter)
    {
        switch (filter)
        {
            case "autor":
                var autoren = await db.Autoren.Include(a => a.Buecher).OrderBy(a => a.Nachname).ToListAsync();
                return autoren.Where(a => a.Buecher.Count > 0)
                    .Select(a => new BuchGruppe(a.AutorID, a.VollerName, Sortiert(a.Buecher))).ToList();

            case "alter":
                var alter = await db.Altersklassifikationen.Include(a => a.Buecher).OrderBy(a => a.AltersklassifikationID).ToListAsync();
                return alter.Where(a => a.Buecher.Count > 0)
                    .Select(a => new BuchGruppe(a.AltersklassifikationID, a.Bezeichnung, Sortiert(a.Buecher))).ToList();

            default:
                var genres = await db.Genres.Include(g => g.Buecher).OrderBy(g => g.GenreID).ToListAsync();
                return genres.Where(g => g.Buecher.Count > 0)
                    .Select(g => new BuchGruppe(g.GenreID, g.Bezeichnung, Sortiert(g.Buecher))).ToList();
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

    private static List<Buch> Sortiert(IEnumerable<Buch> buecher) =>
        buecher.OrderBy(b => b.BuchID).ToList();
}
