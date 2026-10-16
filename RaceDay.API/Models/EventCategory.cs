using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class EventCategory
    {
        [Key]
        public int EventCategoryID { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        public decimal EntryFee { get; set; }

        public int? MaxParticipants { get; set; }

        public Event? Event { get; set; }

        public Category? Category { get; set; }

        public ICollection<Enrolment> Enrolments { get; set; }
            = new List<Enrolment>();
    }
}