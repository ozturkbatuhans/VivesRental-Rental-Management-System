using System.Net;
using System.Net.Http.Json;
using VivesRental.Blazor.Models;

namespace VivesRental.Blazor.Services;

public class ArticleReservationService
{
    private readonly HttpClient _http;

    public ArticleReservationService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ArticleReservationDto>> GetAllAsync()
    {
        var result = await _http.GetFromJsonAsync<List<ArticleReservationDto>>("api/ArticleReservations");
        return result ?? new List<ArticleReservationDto>();
    }

    public async Task<ArticleReservationDto?> GetByIdAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<ArticleReservationDto>($"api/ArticleReservations/{id}");
    }

    public async Task CreateAsync(ArticleReservationCreateRequest request)
    {
        var resp = await _http.PostAsJsonAsync("api/ArticleReservations", request);

        if (resp.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new InvalidOperationException("Not authorized.");

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(body) ? "Unable to create reservation." : body);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var resp = await _http.DeleteAsync($"api/ArticleReservations/{id}");

        if (resp.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new InvalidOperationException("Not authorized.");

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(body) ? "Unable to delete reservation." : body);
        }
    }
}
