using IPWebsiteMVC.Models;
using IPWebsiteMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IPWebsiteMVC.Controllers
{
    public class HomeController(KatalogService katalog) : Controller
    {
        public async Task<IActionResult> Index(string? filter)
        {
            filter = KatalogService.NormalizeFilter(filter);
            ViewData["Filter"] = filter;

            if (filter == "benutzer")
            {
                if (User.BenutzerId() is not { } benutzerId) return Challenge();
                return View(await katalog.GetKategorienAsync(benutzerId));
            }
            return View(await katalog.GetGruppenAsync(filter));
        }

        public async Task<IActionResult> Liste(string? filter, int id)
        {
            filter = KatalogService.NormalizeFilter(filter);

            BuchGruppe? gruppe;
            if (filter == "benutzer")
            {
                if (User.BenutzerId() is not { } benutzerId) return Challenge();
                gruppe = await katalog.GetKategorieAsync(benutzerId, id);
            }
            else
            {
                gruppe = await katalog.GetGruppeAsync(filter, id);
            }
            if (gruppe is null) return NotFound();

            ViewData["Filter"] = filter;
            ViewData["ActiveId"] = id;
            return View(gruppe);
        }

        public async Task<IActionResult> Details(int id)
        {
            var buch = await katalog.GetBuchAsync(id);
            if (buch is null) return NotFound();

            var tabs = new List<NavTab>();
            if (buch.Genres.FirstOrDefault() is { } genre)
                tabs.Add(new NavTab(genre.Bezeichnung, Url.Action(nameof(Liste), new { filter = "genre", id = genre.GenreID })!));
            if (buch.Altersklassifikationen.FirstOrDefault() is { } alter)
                tabs.Add(new NavTab(alter.Bezeichnung, Url.Action(nameof(Liste), new { filter = "alter", id = alter.AltersklassifikationID })!));
            if (buch.Autor is { } autor)
                tabs.Add(new NavTab(autor.VollerName, Url.Action(nameof(Liste), new { filter = "autor", id = autor.AutorID })!));

            ViewData["NavTabs"] = tabs;
            return View(buch);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
