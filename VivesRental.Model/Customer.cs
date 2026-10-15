namespace VivesRental.Model;

public class Customer
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;

    public IList<Order> Orders { get; set; } = new List<Order>();
    public IList<ArticleReservation> ArticleReservations { get; set; } = new List<ArticleReservation>();
}