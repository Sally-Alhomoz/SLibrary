using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using SLibraryBlazor.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private readonly IAccountService _accountService;

    public JwtAuthenticationStateProvider(ProtectedSessionStorage sessionStorage, IAccountService accountService)
    {
        _sessionStorage = sessionStorage;
        _accountService = accountService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        try
        {
            var token = await _accountService.GetJwtToken();

            if (token != null)
            {
                principal = BuildClaimsPrincipalFromJwt(token);
            }
        }
        catch (Exception)
        {
            principal = new ClaimsPrincipal(new ClaimsIdentity());
        }

        return new AuthenticationState(principal);
    }

    public void NotifyUserLoggedIn(string jwtToken)
    {
        var principal = BuildClaimsPrincipalFromJwt(jwtToken);
        var authState = Task.FromResult(new AuthenticationState(principal));
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLoggedOut()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(principal));
        NotifyAuthenticationStateChanged(authState);
    }
    private static ClaimsPrincipal BuildClaimsPrincipalFromJwt(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwtToken);
        var claimsIdentity = new ClaimsIdentity(token.Claims, "jwtAuth");
        return new ClaimsPrincipal(claimsIdentity);
    }

    public void Dispose() { }
}