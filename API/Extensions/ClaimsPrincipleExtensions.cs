using System;
using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipleExtensions
{
    // Estensione per ottenere il nome utente dal ClaimsPrincipal
    public static string GetUsername(this ClaimsPrincipal user)
    {
        // Trova il valore del nome utente usando ClaimTypes.NameIdentifier
        var username = user.FindFirstValue(ClaimTypes.NameIdentifier) 
        ?? throw new Exception("No username found in token"); // Lancia un'eccezione se non viene trovato alcun nome utente
        return username; // Ritorna il nome utente
    }
}
