namespace IPWebsiteMVC.Models;

public record BuchGruppe(int Id, string Name, List<Buch> Buecher);

public record NavTab(string Text, string Url, bool Active = false);

public record NavigationModel(List<NavTab> Tabs, string Filter, string? Benutzername, bool IstAdmin);
