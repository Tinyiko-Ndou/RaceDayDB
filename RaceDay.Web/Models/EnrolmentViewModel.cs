namespace RaceDay.Web.Models
{
    public class EnrolmentViewModel
    {
        public int EnrolmentID { get; set; }

        public int UserID { get; set; }

        public int EventID { get; set; }

        public int EventCategoryID { get; set; }

        public DateTime EnrolmentDate { get; set; }

        public string Status { get; set; } = "";

        public string EventName { get; set; } = "";

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = "";
    }
}