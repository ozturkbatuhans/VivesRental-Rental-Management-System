namespace VivesRental.Blazor.Models;

public class ArticleReservationDto
{
    public Guid Id { get; set; }

    public Guid ArticleId { get; set; }
    public int ArticleStatus { get; set; }
    public string ProductName { get; set; } = "";

    public Guid CustomerId { get; set; }
    public string CustomerFirstName { get; set; } = "";
    public string CustomerLastName { get; set; } = "";

    public DateTime FromDateTime { get; set; }
    public DateTime UntilDateTime { get; set; }

    public string CustomerName => $"{CustomerFirstName} {CustomerLastName}";
    public string ArticleName => $"{ProductName} - {ArticleId}";
}
