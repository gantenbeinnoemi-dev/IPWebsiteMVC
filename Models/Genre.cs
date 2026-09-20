namespace IPWebsiteMVC.Models;

public class Genre
{
    public int GenreID { get; set; }
    public string Bezeichnung { get; set; } = "";
    public List<Buch> Buecher { get; set; } = [];
}
