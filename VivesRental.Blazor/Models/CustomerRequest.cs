using System.ComponentModel.DataAnnotations;

namespace VivesRental.Blazor.Models;

public class CustomerRequest
{
    [Required]
    public string FirstName { get; set; } = "";

    [Required]
    public string LastName { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string PhoneNumber { get; set; } = "";
}
