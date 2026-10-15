using System.ComponentModel.DataAnnotations;

namespace VivesRental.Blazor.Models;

public class ArticleReservationCreateRequest
{
    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public Guid ArticleId { get; set; }

    [Required]
    public DateTime StartDate { get; set; } = DateTime.Today;

    public DateTime? EndDate { get; set; }
}
