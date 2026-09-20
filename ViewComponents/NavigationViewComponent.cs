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

        if (pageData["NavTabs"] is not List<NavTab> tabs)
        {
            var gruppen = await katalog.GetGruppenAsync(filter);
            tabs = gruppen
                .Select(g => new NavTab(
                    g.Name,
                    Url.Action("Liste", "Home", new { filter, id = g.Id })!,
                    Active: activeId == g.Id))
                .ToList();
        }

        var benutzername = User.Identity?.IsAuthenticated == true ? User.Identity.Name : null;
        return View(new NavigationModel(tabs, filter, benutzername, User.IsInRole(Rollen.Admin)));
    }
}
