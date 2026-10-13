using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Result
    {
        [Key]
        public int ResultID { get; set; }

        [Required]
        public int EnrolmentID { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? Position { get; set; }

        public decimal? Pace { get; set; }

        [MaxLength(30)]
        public string ResultStatus { get; set; } = "Pending";

        public Enrolment? Enrolment { get; set; }
    }
}