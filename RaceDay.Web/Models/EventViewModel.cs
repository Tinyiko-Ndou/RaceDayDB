namespace RaceDay.Web.Models
{
    public class EventViewModel
    {
        public int EventID { get; set; }

        public int OrganizerID { get; set; }

        public string EventName { get; set; } = "";

        public string Description { get; set; } = "";

        public string EventType { get; set; } = "";

        public DateTime EventDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public string Location { get; set; } = "";

        public string City { get; set; } = "";

        public string Province { get; set; } = "";

        public string Status { get; set; } = "";
    }
}