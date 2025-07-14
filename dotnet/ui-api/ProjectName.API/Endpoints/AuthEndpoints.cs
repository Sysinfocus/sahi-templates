using Microsoft.AspNetCore.Identity.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProjectName.API.Endpoints;

public class AuthEndpoints(
    ILogger<UserEndpoints> logger,
    HybridCache cache,
    INotifications notifications) : IEndpoints
{    
    private const string version = "/api/v1/";
    private const string keyName = "Auth";
    public void Register(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(version + keyName);

        group.MapPost("/login", LoginUser);
        group.MapPost("/register", RegisterUser);
        group.MapPost("/refresh", RefreshUser);
        group.MapGet("/logout/{token}", LogoutUser);
    }

    private async Task<IResult> LoginUser(UserDto request, IConfiguration config, AppDbContext context, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Username == request.Username, cancellationToken: cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User Login: {Username} not found.", request.Username);
            return Results.Unauthorized();
        }

        if (user.IsLocked)
        {
            logger.LogWarning("User Login: {Username} is locked.", request.Username);
            return Results.Forbid();
        }
        var hasher = new PasswordHasher<object>();
        var result = hasher.VerifyHashedPassword(null!, user.Password, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            logger.LogWarning("User Login: Invalid password for username {Username}.", request.Username);
            return Results.Unauthorized();
        }

        var claims = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.Fullname),
            new Claim(ClaimTypes.Role, user.Roles),
        ]);
        AuthDetails token = await GenerateAccessRefreshToken(config, context, claims, user.Id.ToString(), cancellationToken);
        await notifications.Notify(request, "User Login", NotificationAction.Read);
        return Results.Ok(token);
    }

    private async Task<IResult> RegisterUser(UserDto request, IConfiguration config, AppDbContext context, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Username == request.Username || x.Email == request.Email,
            cancellationToken: cancellationToken);
        if (user is not null)
        {
            logger.LogWarning("User Registration: Duplicate registration for username {Username} / email {Email}.", request.Username, request.Email);
            return Results.BadRequest("Either username and/or email already exists. Try with different username/email address.");
        }

        var hasher = new PasswordHasher<object>();
        request.Password = hasher.HashPassword(null!, request.Password);

        var model = request.ToModel();
        context.Users.Add(model);
        var result = await context.SaveChangesAsync(cancellationToken);
        if (result > 0)
        {
            await notifications.Notify(model, "User Registration", NotificationAction.Created);
            return Results.Created();
        }
        logger.LogError("User Registration: Failed to create for username {Username} / email {Email}.", request.Username, request.Email);
        return Results.BadRequest();
    }

    private async Task<IResult> RefreshUser(RefreshRequest request, IConfiguration config, HttpContext httpContext, AppDbContext context, CancellationToken cancellationToken)
    {
        var accessToken = httpContext.Request.Headers.Authorization;
        // get claims from accessToken
        var claims = GetClaimsFromAccessToken(accessToken);
        if (claims is null)
        {
            logger.LogError("User Refresh: Failed to refresh.");
            return Results.Unauthorized();
        }

        var userId = claims.FirstOrDefault(x => x.Type == "nameid")?.Value;

        var exists = await context.RefreshTokens.AnyAsync(x => x.UserId.ToString() == userId &&
            x.Token == request.RefreshToken, cancellationToken);

        if (!exists || string.IsNullOrWhiteSpace(userId))
        {
            logger.LogError("User Refresh: Failed to refresh for UserId {userId}.", userId);
            return Results.Unauthorized();
        }

        AuthDetails token = await GenerateAccessRefreshToken(config, context,
            new ClaimsIdentity(claims), userId, cancellationToken, request.RefreshToken);

        await notifications.Notify(request, "User Token Refresh", NotificationAction.Updated);
        return Results.Ok(token);
    }
    private async Task<IResult> LogoutUser(string token, HttpContext httpContext, AppDbContext context, CancellationToken cancellationToken)
    {
        var accessToken = httpContext.Request.Headers.Authorization;
        var claims = GetClaimsFromAccessToken(accessToken);
        if (claims is null)
        {
            logger.LogError("User Logout: Failed to logout.");
            return Results.Unauthorized();
        }

        var userId = claims.FirstOrDefault(x => x.Type == "nameid")?.Value;

        var exists = await context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId.ToString() == userId &&
            x.Token == token, cancellationToken);

        if (exists is null || string.IsNullOrWhiteSpace(userId))
        {
            logger.LogError("User Logout: Failed to logout UserId {userId}.", userId);
            return Results.Unauthorized();
        }
        
        context.Remove(exists);
        if (await context.SaveChangesAsync(cancellationToken) > 0)
        {
            await notifications.Notify("", $"User {userId} logged out.", NotificationAction.Updated);
            return Results.Ok();
        }

        logger.LogError("User Logout: Failed to logout UserId {userId}.", userId);
        return Results.BadRequest();
    }

    private static IEnumerable<Claim>? GetClaimsFromAccessToken(string? accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken)) return null;
        accessToken = accessToken.Replace("bearer ", "", StringComparison.OrdinalIgnoreCase);
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(accessToken);
        return token.Claims;
    }

    private async Task<AuthDetails> GenerateAccessRefreshToken(IConfiguration config, AppDbContext context, ClaimsIdentity claims, string userId, CancellationToken cancellationToken, string? lastToken = null)
    {
        var jwtSettings = config.GetSection(nameof(JwtSettings)).Get<JwtSettings>()!;
        var token = JwtAuthenticationService.GenerateTokens(claims, jwtSettings);
        var removed = await context.RefreshTokens.Where(a => a.Token == lastToken || a.ExpiresOn < DateTime.UtcNow).ExecuteDeleteAsync(cancellationToken);
        if (removed > 0) await cache.RemoveByTagAsync("RefreshTokens", cancellationToken);
        var newToken = new RefreshToken(Guid.NewGuid(), userId, token.RefreshToken, DateTime.UtcNow.AddHours(jwtSettings.RefreshHours));
        context.RefreshTokens.Add(newToken);
        await context.SaveChangesAsync(cancellationToken);
        return token;
    }
}