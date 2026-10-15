namespace VivesRental.Model;

public class ArticleReservation
{
    public Guid Id { get; set; }
    public Guid ArticleId { get; set; }
    public Article Article { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public DateTime FromDateTime { get; set; }
    public DateTime UntilDateTime { get; set; }
}