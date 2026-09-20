namespace IPWebsiteMVC.Models;

public class Buch
{
    public int BuchID { get; set; }
    public string Titel { get; set; } = "";
    public string? Reihe { get; set; }
    public int? Band { get; set; }
    public int? AnzahlKapitel { get; set; }
    public int? AutorID { get; set; }
    public Autor? Autor { get; set; }
    public string? CoverBildUrl { get; set; }
    public string? Beschreibung { get; set; }
    public int? ErscheinungsJahr { get; set; }
    public List<Genre> Genres { get; set; } = [];
    public List<Altersklassifikation> Altersklassifikationen { get; set; } = [];

    public string? ReiheBandText => Reihe is not null && Band is not null ? $"{Reihe} Band {Band}" : null;
}
