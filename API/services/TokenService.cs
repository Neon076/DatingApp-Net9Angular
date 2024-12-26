using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.services;

public class TokenService (IConfiguration config): ITokenInterface
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot acces token key from appsettings");
        if(tokenKey.Length < 64) throw new Exception("Tokenkey must be longer");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier , user.Id.ToString()),
            new(ClaimTypes.Name , user.UserName)
        };

        var creds = new SigningCredentials(key , SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor{
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(2),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
