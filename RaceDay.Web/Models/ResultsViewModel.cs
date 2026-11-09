namespace RaceDay.Web.Models
{
    public class ResultViewModel
    {
        public int ResultID { get; set; }

        public int EnrolmentID { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        public string? Pace { get; set; }

        public string ResultStatus { get; set; } = "";

        public string EventName { get; set; } = "";
    }
}