using System.ComponentModel.DataAnnotations;

namespace IPWebsiteMVC.Models;

public class AnmeldenModel
{
    [Required(ErrorMessage = "Bitte gib deinen Benutzernamen ein.")]
    [Display(Name = "Benutzername")]
    public string Benutzername { get; set; } = "";

    [Required(ErrorMessage = "Bitte gib dein Passwort ein.")]
    [DataType(DataType.Password)]
    [Display(Name = "Passwort")]
    public string Passwort { get; set; } = "";
}

public class RegistrierenModel
{
    [Required(ErrorMessage = "Bitte wähle einen Benutzernamen.")]
    [StringLength(50, ErrorMessage = "Der Benutzername darf höchstens 50 Zeichen lang sein.")]
    [Display(Name = "Benutzername")]
    public string Benutzername { get; set; } = "";

    [Required(ErrorMessage = "Bitte wähle ein Passwort.")]
    [DataType(DataType.Password)]
    [Display(Name = "Passwort")]
    public string Passwort { get; set; } = "";

    [Required(ErrorMessage = "Bitte wiederhole dein Passwort.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Passwort), ErrorMessage = "Die Passwörter stimmen nicht überein.")]
    [Display(Name = "Passwort wiederholen")]
    public string PasswortWiederholen { get; set; } = "";
}
