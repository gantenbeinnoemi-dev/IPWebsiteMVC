namespace IPWebsiteMVC.Models;

public record BuchGruppe(int Id, string Name, IEnumerable<Buch> Buecher);

public record NavTab(string Text, string Url, bool Active = false);

public record NavigationModel(IEnumerable<NavTab> Tabs, string Filter, string? Benutzername, bool IstAdmin);
