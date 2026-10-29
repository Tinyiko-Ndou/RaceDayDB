using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; } = "";

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = "";

        [Required]
        public string Role { get; set; } = "Participant";
    }
}
