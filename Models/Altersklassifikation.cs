namespace IPWebsiteMVC.Models;

public class Altersklassifikation
{
    public int AltersklassifikationID { get; set; }
    public string Bezeichnung { get; set; } = "";
    public List<Buch> Buecher { get; set; } = [];
}
