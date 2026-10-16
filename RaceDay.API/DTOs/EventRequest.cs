using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class EventRequest
    {
        [Required]
        [MaxLength(150)]
        public string EventName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string EventType { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        public string? City { get; set; }

        public string? Province { get; set; }

        public string Status { get; set; } = "Open";
    }
}