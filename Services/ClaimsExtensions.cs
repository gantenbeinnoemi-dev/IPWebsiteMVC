using System.Security.Claims;

namespace IPWebsiteMVC.Services;

public static class ClaimsExtensions
{
    /// <summary>Die BenutzerID des angemeldeten Benutzers oder null, wenn niemand angemeldet ist.</summary>
    public static int? BenutzerId(this ClaimsPrincipal user) =>
        user.Identity?.IsAuthenticated == true && int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : null;
}
