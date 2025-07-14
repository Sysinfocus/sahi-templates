using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProjectName.API.Settings;

public static class JwtAuthenticationService
{
    public static void AddJwtAuthenticationService(this WebApplicationBuilder builder)
    {
        var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>()!;

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            });
        builder.Services.AddAuthorization();
    }

    public static AuthDetails GenerateTokens(ClaimsIdentity claims, JwtSettings jwtSettings)
    {
        var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddSeconds(jwtSettings.ValidSeconds),
            Issuer = jwtSettings.Issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);
        var refreshToken = Guid.NewGuid().ToString();        
        return new AuthDetails("Bearer", accessToken, jwtSettings.ValidSeconds, refreshToken);
    }

}
public sealed record JwtSettings(string SecretKey, string Audience, string Issuer, int ValidSeconds, int RefreshHours);
public sealed record AuthDetails(string TokenType, string AccessToken, int ExpiresIn, string RefreshToken);