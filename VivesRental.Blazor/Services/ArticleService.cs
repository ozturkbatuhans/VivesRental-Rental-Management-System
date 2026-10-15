using System.Net;
using System.Net.Http.Json;
using VivesRental.Blazor.Models;

namespace VivesRental.Blazor.Services;

public class ArticleService
{
    private readonly HttpClient _http;

    public ArticleService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ArticleDto>> GetArticlesAsync(Guid? productId = null)
    {
        var url = "api/Articles";
        if (productId.HasValue && productId.Value != Guid.Empty)
            url += $"?productId={productId.Value}";

        var result = await _http.GetFromJsonAsync<List<ArticleDto>>(url);
        return result ?? new List<ArticleDto>();
    }

    public async Task<ArticleDto?> GetByIdAsync(Guid id)
    {
        return await _http.GetFromJsonAsync<ArticleDto>($"api/Articles/{id}");
    }

    public async Task<ArticleDto?> CreateAsync(Guid productId)
    {
        var req = new ArticleCreateRequest { ProductId = productId };

        var resp = await _http.PostAsJsonAsync("api/Articles", req);

        if (resp.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new InvalidOperationException("Not authorized.");

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"API error {(int)resp.StatusCode} {resp.ReasonPhrase}. Body: {body}"
            );
        }

        try
        {
            return await resp.Content.ReadFromJsonAsync<ArticleDto>();
        }
        catch
        {
            return null;
        }
    }

    public async Task GenerateAsync(Guid productId, int amount)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Amount must be greater than 0.");

        var resp = await _http.PostAsync($"api/Products/{productId}/articles/generate?amount={amount}", content: null);

        if (resp.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new InvalidOperationException("Not authorized.");

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(body) ? "Unable to generate articles." : body);
        }
    }

    public async Task UpdateStatusAsync(Guid articleId, string status)
    {
        var resp = await _http.PutAsync($"api/Articles/{articleId}/status?status={Uri.EscapeDataString(status)}", content: null);

        if (resp.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new InvalidOperationException("Not authorized.");

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(body) ? "Unable to update status." : body);
        }
    }

    public async Task DeleteAsync(Guid articleId)
    {
        var resp = await _http.DeleteAsync($"api/Articles/{articleId}");

        if (resp.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new InvalidOperationException("Not authorized.");

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(body) ? "Unable to delete article." : body);
        }
    }
}
