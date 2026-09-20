namespace IPWebsiteMVC.Models;

public class Autor
{
    public int AutorID { get; set; }
    public string Vorname { get; set; } = "";
    public string Nachname { get; set; } = "";
    public int? LandID { get; set; }
    public Land? Land { get; set; }
    public List<Buch> Buecher { get; set; } = [];

    public string VollerName => $"{Vorname} {Nachname}";
}
