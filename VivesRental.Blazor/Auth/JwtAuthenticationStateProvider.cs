using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace VivesRental.Blazor.Auth;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthState _authState;

    public JwtAuthenticationStateProvider(AuthState authState)
    {
        _authState = authState;
        _authState.OnChange += HandleAuthStateChanged;
    }

    private void HandleAuthStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _authState.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.Name, "Admin"),
                new Claim(ClaimTypes.Role, "Admin")
            },
            authenticationType: "jwt"
        );

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
}
