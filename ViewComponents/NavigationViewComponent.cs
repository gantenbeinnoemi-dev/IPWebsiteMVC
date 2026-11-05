using IPWebsiteMVC.Models;
using IPWebsiteMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace IPWebsiteMVC.ViewComponents;

public class NavigationViewComponent(KatalogService katalog) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var pageData = ViewContext.ViewData;
        var filter = pageData["Filter"] as string ?? "genre";
        var activeId = pageData["ActiveId"] as int?;
        var benutzerId = UserClaimsPrincipal.BenutzerId();

        if (filter == "benutzer" && benutzerId is null) filter = "genre";

        IEnumerable<NavTab> tabs;
        if (pageData["NavTabs"] is IEnumerable<NavTab> vorgegeben)
        {
            tabs = vorgegeben;
        }
        else if (filter == "benutzer")
        {
            var kategorien = await katalog.GetKategorieNamenAsync(benutzerId!.Value);
            tabs = kategorien
                .Select(k => new NavTab(k.Name, Url.Action("Liste", "Home", new { filter, id = k.Id })!, Active: activeId == k.Id))
                .Append(new NavTab("+ Neue Kategorie", Url.Action("Neu", "Kategorie")!, Active: pageData["NeuAktiv"] is true));
        }
        else
        {
            var gruppen = await katalog.GetGruppenAsync(filter);
            tabs = gruppen.Select(g => new NavTab(g.Name, Url.Action("Liste", "Home", new { filter, id = g.Id })!, Active: activeId == g.Id));
        }

        var benutzername = User.Identity?.IsAuthenticated == true ? User.Identity.Name : null;
        return View(new NavigationModel(tabs, filter, benutzername, User.IsInRole(Rollen.Admin)));
    }
}
