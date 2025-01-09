using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API;

public class TokenService(IConfiguration config,UserManager<AppUser> userManager) : ITokenService
{
    public async Task<string> CreateToken(AppUser user)
    {
        // Ottieni la chiave del token dalla configurazione
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot access tokenKey from appsettings");
        
        // Verifica che la chiave del token sia abbastanza lunga
        if (tokenKey.Length < 64) throw new Exception("Your tokenKey needs to be longer");
        
        // Crea una chiave di sicurezza simmetrica
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        if (user.UserName == null) throw new Exception("User does not have a username");

        // Crea i claims per l'utente
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName)
        };

        var roles = await userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Crea le credenziali di firma
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        // Descrittore del token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7), // Imposta la scadenza del token
            SigningCredentials = creds
        };

        // Crea il token
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Restituisce il token come stringa
        return tokenHandler.WriteToken(token);
    }
}