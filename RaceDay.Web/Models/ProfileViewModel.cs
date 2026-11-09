using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{
    public class ProfileViewModel
    {
        public int UserID { get; set; }

        [Required]
        public string FirstName { get; set; } = "";

        [Required]
        public string LastName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        public string? PhoneNumber { get; set; }

        public string Role { get; set; } = "";
    }
}