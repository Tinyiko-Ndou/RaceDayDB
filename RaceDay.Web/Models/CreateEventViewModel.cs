using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{
    public class CreateEventViewModel
    {
        [Required]
        public string EventName { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        [Required]
        public string EventType { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public string Location { get; set; } = "";

        [Required]
        public string City { get; set; } = "";

        [Required]
        public string Province { get; set; } = "";

        public string Status { get; set; } = "Upcoming";
    }
}