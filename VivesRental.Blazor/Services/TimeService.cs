using System.Net.Http.Json;

namespace VivesRental.Blazor.Services;

public class TimeService
{
    private readonly HttpClient _http;

    public TimeService(HttpClient http)
    {
        _http = http;
    }

    public async Task<DateTime> GetUtcNowAsync()
    {
        var dt = await _http.GetFromJsonAsync<DateTime>("api/time/utcnow");
        return dt;
    }
}
