using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash {  get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber {  get; set; }

        [Required]
        [MaxLength(20)]
        public string Role {  get; set; } = string.Empty;

        public ICollection<Event> OrganisedEvents {  get; set; }
        = new List<Event>();

       public ICollection<Enrolment> Enrolments { get; set; }
        = new List<Enrolment>();


    }
}
