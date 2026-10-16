using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        public int OrganizerID { get; set; }

        [Required, MaxLength(150)]
        public string EventName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required, MaxLength(20)]
        public string EventType { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        [Required, MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Province { get; set; }

        [MaxLength(30)]
        public string Status { get; set; } = "Upcoming";

        public User? Organiser { get; set; }

        public ICollection<EventCategory> EventCategories { get; set; }
            = new List<EventCategory>();

        public ICollection<Enrolment> Enrolments { get; set; }
            = new List<Enrolment>();

        public Route? Route { get; set; }

        public ICollection<Weather> WeatherRecords { get; set; }
            = new List<Weather>();
    }
}