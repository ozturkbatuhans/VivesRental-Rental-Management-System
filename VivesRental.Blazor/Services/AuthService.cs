using System.Net.Http.Json;
using System.Text.Json;
using VivesRental.Blazor.Auth;
using VivesRental.Blazor.Models;

namespace VivesRental.Blazor.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly AuthState _authState;

    public AuthService(HttpClient http, AuthState authState)
    {
        _http = http;
        _authState = authState;
    }

    public Task InitializeAsync()
        => _authState.InitializeAsync();

    public async Task<bool> LoginAsync(string username, string password)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/login", new LoginRequest
        {
            Username = username,
            Password = password
        });

        if (!response.IsSuccessStatusCode)
            return false;

        var json = await response.Content.ReadAsStringAsync();

        string? token = null;

        using (var doc = JsonDocument.Parse(json))
        {
            if (doc.RootElement.TryGetProperty("token", out var t))
                token = t.GetString();
            else if (doc.RootElement.TryGetProperty("Token", out var t2))
                token = t2.GetString();
        }

        if (string.IsNullOrWhiteSpace(token))
            return false;

        await _authState.SetTokenAsync(token);
        return true;
    }

    public Task LogoutAsync()
        => _authState.ClearTokenAsync();
}
