using System.Security.Claims;
using IPWebsiteMVC.Data;
using IPWebsiteMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IPWebsiteMVC.Controllers;

public class KontoController(MyBooksDbContext db, IPasswordHasher<Benutzer> hasher) : Controller
{
    [HttpGet]
    public IActionResult Anmelden(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return ZurueckZu(returnUrl);
        ViewData["ReturnUrl"] = returnUrl;
        return View(new AnmeldenModel());
    }

    [HttpPost]
    public async Task<IActionResult> Anmelden(AnmeldenModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);

        var name = model.Benutzername.Trim();
        var benutzer = await db.Benutzer.FirstOrDefaultAsync(b => b.Name == name);
        if (benutzer is null
            || hasher.VerifyHashedPassword(benutzer, benutzer.PasswortHash, model.Passwort) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Benutzername oder Passwort ist falsch.");
            return View(model);
        }

        await SignInAsync(benutzer);
        return ZurueckZu(returnUrl);
    }

    [HttpGet]
    public IActionResult Registrieren(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return ZurueckZu(returnUrl);
        ViewData["ReturnUrl"] = returnUrl;
        return View(new RegistrierenModel());
    }

    [HttpPost]
    public async Task<IActionResult> Registrieren(RegistrierenModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);

        var name = model.Benutzername.Trim();
        if (await db.Benutzer.AnyAsync(b => b.Name == name))
            return NameVergeben(model);

        var benutzer = new Benutzer { Name = name, Rolle = Rollen.Benutzer };
        benutzer.PasswortHash = hasher.HashPassword(benutzer, model.Passwort);
        db.Benutzer.Add(benutzer);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Zwei gleichzeitige Registrierungen mit demselben Namen: die UNIQUE-Regel der Datenbank greift.
            return NameVergeben(model);
        }

        await SignInAsync(benutzer);
        return ZurueckZu(returnUrl);
    }

    [HttpGet]
    public IActionResult ZugriffVerweigert() => View();

    [HttpPost]
    public async Task<IActionResult> Abmelden()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private IActionResult NameVergeben(RegistrierenModel model)
    {
        ModelState.AddModelError(nameof(RegistrierenModel.Benutzername), "Dieser Benutzername ist bereits vergeben.");
        return View(model);
    }

    private IActionResult ZurueckZu(string? returnUrl) =>
        Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : RedirectToAction("Index", "Home");

    private Task SignInAsync(Benutzer benutzer)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, benutzer.BenutzerID.ToString()),
            new(ClaimTypes.Name, benutzer.Name),
            new(ClaimTypes.Role, benutzer.Rolle)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }
}
