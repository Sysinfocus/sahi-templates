using System.IdentityModel.Tokens.Jwt;

namespace ProjectName.UI.Services;

public class ApplicationStateProvider(BrowserExtensions browserExtensions, Settings settings) : AuthenticationStateProvider
{
    //private ClaimsPrincipal _user = new(new ClaimsIdentity());
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await GetToken();
        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(new(new ClaimsIdentity()));

        var claims = GetClaimsFromAccessToken(token);
        //foreach(var c in claims ?? []) Console.WriteLine(c.Type + " = " + c.Value);
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
    }

    public async Task<string?> GetToken()
    {        
        var authData = await browserExtensions.GetFromLocalStorage("authToken");
        if (authData is null) return null;
        var auth = JsonSerializer.Deserialize<UserAuthentication>(authData);
        if (auth is null) return null;
        return auth.AccessToken;
    }

    public async Task<string?> GetRefreshToken()
    {
        var authData = await browserExtensions.GetFromLocalStorage("authToken");
        if (authData is null) return null;
        var auth = JsonSerializer.Deserialize<UserAuthentication>(authData);
        if (auth is null) return null;
        return auth.RefreshToken;
    }

    public static ICollection<Claim>? GetClaimsFromAccessToken(string? accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken)) return null;
        accessToken = accessToken.Replace("bearer ", "", StringComparison.OrdinalIgnoreCase);
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(accessToken);        
        List<Claim> newClaims = [];
        foreach (var claim in token.Claims)
        {
            if (claim.Type != "role")
            {
                newClaims.Add(claim);
                continue;
            }

            var claimValue = claim.Value.Split(',');
            foreach(var cv in claimValue)
                newClaims.Add(new(ClaimTypes.Role, cv.Trim()));
        }            
        return newClaims;
    }

    public async Task Login(UserAuthentication response)
    {
        var auth = JsonSerializer.Serialize(response);
        var token = response.AccessToken;
        await browserExtensions.SetToLocalStorage("authToken", auth);
        var claims = GetClaimsFromAccessToken(token);
        var authState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        NotifyAuthenticationStateChanged(Task.FromResult(authState));        
    }

    public async Task Logout()
    {
        var refreshToken = await GetRefreshToken();
        using var client = new HttpClient() { BaseAddress = new Uri(settings.ApiUrl) };
        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer " + await GetToken());
        _ = await client.GetAsync($"/api/v1/Auth/logout/{refreshToken}");
        await browserExtensions.RemoveFromLocalStorage("authToken");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new(new ClaimsIdentity()))));
        browserExtensions.Goto("/", true, true);
    }
}