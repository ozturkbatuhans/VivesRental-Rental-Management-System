namespace VivesRental.Blazor.Models;

public class ArticleDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int Status { get; set; }

    public string StatusName => Status switch
    {
        0 => "Available",
        1 => "Rented",
        2 => "Reserved",
        _ => $"Status({Status})"
    };
}
