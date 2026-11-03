using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace IPWebsiteMVC.Models;

public class KategorieFormModel
{
    [Required(ErrorMessage = "Bitte gib einen Namen für die Kategorie ein.")]
    [StringLength(100, ErrorMessage = "Der Name darf höchstens 100 Zeichen lang sein.")]
    [Display(Name = "Name der Kategorie")]
    public string Name { get; set; } = "";

    public List<int> BuchIds { get; set; } = [];

    [BindNever, ValidateNever] public List<Buch> Buecher { get; set; } = [];
}

public class BuchAuswahlModel
{
    /// <summary>Gesetzt, wenn die Bücher zu einer schon bestehenden Kategorie hinzugefügt werden.</summary>
    public int? KategorieId { get; set; }

    /// <summary>Name der bestehenden bzw. der noch nicht angelegten Kategorie (kann beim Entwurf leer sein).</summary>
    public string KategorieName { get; set; } = "";

    /// <summary>Bücher, die schon in der Kategorie (bzw. im Entwurf) sind.</summary>
    public List<int> BereitsIds { get; set; } = [];

    public List<Buch> Buecher { get; set; } = [];

    public bool IstNeu => KategorieId is null;
}
