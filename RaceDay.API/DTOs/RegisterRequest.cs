using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class RegisterRequest
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
