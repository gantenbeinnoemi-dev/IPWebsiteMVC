using IPWebsiteMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IPWebsiteMVC.Controllers;

/// <summary>
/// Liefert Buchcover ueber eine stabile, ID-basierte Adresse statt eines festen Dateipfads
/// im HTML. So bleibt der Link gueltig, auch wenn sich CoverBildUrl in der Datenbank aendert
/// (z.B. nach dem Bearbeiten). Fehlt die Datei doch einmal, wird ein Platzhalter ausgeliefert
/// statt eines kaputten Bild-Symbols.
/// </summary>
public class BildController(MyBooksDbContext db, IWebHostEnvironment env) : Controller
{
    private static readonly Dictionary<string, string> ContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".webp"] = "image/webp",
        [".gif"] = "image/gif"
    };

    [HttpGet("/Bild/BuchCover/{id:int}")]
    public async Task<IActionResult> BuchCover(int id)
    {
        var url = await db.Buecher.Where(b => b.BuchID == id).Select(b => b.CoverBildUrl).FirstOrDefaultAsync();
        return CoverUrlAufloesen(url);
    }

    private IActionResult CoverUrlAufloesen(string? url)
    {
        if (string.IsNullOrEmpty(url)) return Platzhalter();

        // Externe Adresse (z.B. https://...): der Browser laedt sie direkt von dort.
        if (Uri.TryCreate(url, UriKind.Absolute, out var abs) && (abs.Scheme == Uri.UriSchemeHttp || abs.Scheme == Uri.UriSchemeHttps))
            return Redirect(url);

        // Lokaler Pfad: bei jeder Anfrage frisch pruefen, ob die Datei (noch) existiert.
        var wurzel = Path.GetFullPath(env.WebRootPath) + Path.DirectorySeparatorChar;
        var pfad = Path.GetFullPath(Path.Combine(env.WebRootPath, url.TrimStart('/')));
        if (!pfad.StartsWith(wurzel, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(pfad))
            return Platzhalter();

        var endung = Path.GetExtension(pfad);
        return PhysicalFile(pfad, ContentTypes.GetValueOrDefault(endung, "application/octet-stream"));
    }

    private PhysicalFileResult Platzhalter() =>
        PhysicalFile(Path.Combine(env.WebRootPath, "images", "kein-cover.svg"), "image/svg+xml");
}
