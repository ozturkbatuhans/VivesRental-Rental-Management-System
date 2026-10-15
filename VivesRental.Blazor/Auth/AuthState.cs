using Blazored.LocalStorage;

namespace VivesRental.Blazor.Auth;

public class AuthState
{
    private const string TokenKey = "authToken";
    private readonly ILocalStorageService _localStorage;

    public AuthState(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public string? Token { get; private set; }
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);

    public event Action? OnChange;

    public async Task InitializeAsync()
    {
        Token = await _localStorage.GetItemAsync<string>(TokenKey);
        OnChange?.Invoke();
    }

    public async Task<string?> GetTokenAsync()
    {
        if (!string.IsNullOrWhiteSpace(Token))
            return Token;

        Token = await _localStorage.GetItemAsync<string>(TokenKey);
        return Token;
    }

    public async Task SetTokenAsync(string token)
    {
        Token = token;
        await _localStorage.SetItemAsync(TokenKey, token);
        OnChange?.Invoke();
    }

    public async Task ClearTokenAsync()
    {
        Token = null;
        await _localStorage.RemoveItemAsync(TokenKey);
        OnChange?.Invoke();
    }
}
