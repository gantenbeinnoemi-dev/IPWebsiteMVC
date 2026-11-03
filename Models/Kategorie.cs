namespace IPWebsiteMVC.Models;

public class Kategorie
{
    public int KategorieID { get; set; }
    public int BenutzerID { get; set; }
    public string Name { get; set; } = "";
    public DateTime ErstelltAm { get; set; } = DateTime.Now;
    public List<KategorieBuch> Eintraege { get; set; } = [];
}

public class KategorieBuch
{
    public int KategorieID { get; set; }
    public int BuchID { get; set; }
    public DateTime HinzugefuegtAm { get; set; } = DateTime.Now;
    public Kategorie? Kategorie { get; set; }
    public Buch? Buch { get; set; }
}
