using IPWebsiteMVC.Data;
using IPWebsiteMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IPWebsiteMVC.Services;

/// <summary>
/// Legt beim Start den festen Admin-Benutzer an (Zugangsdaten aus Admin:Benutzername / Admin:Passwort,
/// lokal in appsettings.Local.json, nicht in git). Das Passwort wird nur als Hash gespeichert.
/// </summary>
public static class AdminSeeder
{
    public static async Task SicherstellenAsync(IServiceProvider services)
    {
        var config = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(AdminSeeder));

        var name = config["Admin:Benutzername"]?.Trim();
        var passwort = config["Admin:Passwort"];
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(passwort))
        {
            logger.LogWarning("Kein Admin konfiguriert (Admin:Benutzername / Admin:Passwort fehlen), Admin wird nicht angelegt.");
            return;
        }

        try
        {
            var db = services.GetRequiredService<MyBooksDbContext>();
            var hasher = services.GetRequiredService<IPasswordHasher<Benutzer>>();

            var admin = await db.Benutzer.FirstOrDefaultAsync(b => b.Name == name);
            if (admin is null)
            {
                admin = new Benutzer { Name = name, Rolle = Rollen.Admin };
                admin.PasswortHash = hasher.HashPassword(admin, passwort);
                db.Benutzer.Add(admin);
                logger.LogInformation("Admin-Benutzer '{Name}' angelegt.", name);
            }
            else
            {
                admin.Rolle = Rollen.Admin;
                if (hasher.VerifyHashedPassword(admin, admin.PasswortHash, passwort) == PasswordVerificationResult.Failed)
                    admin.PasswortHash = hasher.HashPassword(admin, passwort);
            }

            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Admin-Benutzer konnte nicht angelegt werden (laeuft die Datenbank und existiert die Tabelle Benutzer?).");
        }
    }
}
