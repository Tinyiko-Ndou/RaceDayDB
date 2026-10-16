using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Enrolment
    {
        [Key]
        public int EnrolmentID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required]
        public int EventCategoryID { get; set; }

        public DateTime EnrolmentDate { get; set; }
            = DateTime.UtcNow;

        [MaxLength(30)]
        public string Status { get; set; } = "Registered";

        public User? User { get; set; }

        public Event? Event { get; set; }

        public EventCategory? EventCategory { get; set; }

        public Result? Result { get; set; }
    }
}