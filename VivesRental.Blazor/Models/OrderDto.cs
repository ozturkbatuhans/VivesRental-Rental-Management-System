public class OrderDto
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }
    public string CustomerFirstName { get; set; } = "";
    public string CustomerLastName { get; set; } = "";
    public string CustomerEmail { get; set; } = "";
    public string CustomerPhoneNumber { get; set; } = "";

    public DateTime CreatedAt { get; set; }
    public DateTime? ReturnedAt { get; set; }

    public int NumberOfOrderLines { get; set; }
}
