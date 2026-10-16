using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class ResultRequest
    {
        [Required]
        public TimeSpan FinishTime { get; set; }

        [Range(1, int.MaxValue)]
        public int Position { get; set; }

        [MaxLength(50)]
        public string? Pace { get; set; }

        [MaxLength(50)]
        public string ResultStatus { get; set; } = "Finished";
    }
}