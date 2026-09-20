using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace IPWebsiteMVC.Models;

public class BuchFormModel
{
    public int BuchID { get; set; }

    [Required(ErrorMessage = "Bitte gib einen Titel ein.")]
    [StringLength(255, ErrorMessage = "Der Titel darf höchstens 255 Zeichen lang sein.")]
    [Display(Name = "Titel")]
    public string Titel { get; set; } = "";

    [StringLength(255, ErrorMessage = "Die Reihe darf höchstens 255 Zeichen lang sein.")]
    [Display(Name = "Reihe")]
    public string? Reihe { get; set; }

    [Range(1, 1000, ErrorMessage = "Der Band muss zwischen 1 und 1000 liegen.")]
    [Display(Name = "Band")]
    public int? Band { get; set; }

    [Range(0, 10000, ErrorMessage = "Die Anzahl Kapitel muss zwischen 0 und 10000 liegen.")]
    [Display(Name = "Anzahl Kapitel")]
    public int? AnzahlKapitel { get; set; }

    [Range(0, 3000, ErrorMessage = "Das Erscheinungsjahr muss zwischen 0 und 3000 liegen.")]
    [Display(Name = "Erscheinungsjahr")]
    public int? ErscheinungsJahr { get; set; }

    [Display(Name = "Autor")]
    public int? AutorID { get; set; }

    [StringLength(5000, ErrorMessage = "Die Beschreibung darf höchstens 5000 Zeichen lang sein.")]
    [Display(Name = "Beschreibung")]
    public string? Beschreibung { get; set; }

    [StringLength(500, ErrorMessage = "Die Cover-Adresse darf höchstens 500 Zeichen lang sein.")]
    [Display(Name = "Cover-Adresse")]
    public string? CoverBildUrl { get; set; }

    public IFormFile? CoverDatei { get; set; }
    public bool CoverEntfernen { get; set; }

    public List<int> GenreIds { get; set; } = [];
    public List<int> AlterIds { get; set; } = [];

    [BindNever, ValidateNever] public string? AktuellesCover { get; set; }
    [BindNever, ValidateNever] public List<Autor> Autoren { get; set; } = [];
    [BindNever, ValidateNever] public List<Genre> Genres { get; set; } = [];
    [BindNever, ValidateNever] public List<Altersklassifikation> Altersklassen { get; set; } = [];
}

public record AdminUebersichtModel(int Buecher, int Autoren, int Laender, int Genres, int Altersklassen);

public record AutorZeile(int Id, string Vorname, string Nachname, int? LandId, int AnzahlBuecher);

public record AutorenModel(List<AutorZeile> Zeilen, List<Land> Laender);

public record StammdatenZeile(int Id, string Text, int Anzahl);

public record StammdatenModel(string Typ, string Titel, string Einzahl, string AnzahlBeschriftung, List<StammdatenZeile> Zeilen);
