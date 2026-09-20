namespace IPWebsiteMVC.Models;

public static class Rollen
{
    public const string Benutzer = "Benutzer";
    public const string Admin = "Admin";
}

public class Benutzer
{
    public int BenutzerID { get; set; }
    public string Name { get; set; } = "";
    public string? Email { get; set; }
    public string PasswortHash { get; set; } = "";
    public string Rolle { get; set; } = Rollen.Benutzer;
    public DateTime ErstelltAm { get; set; } = DateTime.Now;
}
