using System.Net.Http.Headers;

namespace VivesRental.Blazor.Auth;

public class AuthHttpMessageHandler : DelegatingHandler
{
    private readonly AuthState _authState;

    public AuthHttpMessageHandler(AuthState authState)
    {
        _authState = authState;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _authState.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

