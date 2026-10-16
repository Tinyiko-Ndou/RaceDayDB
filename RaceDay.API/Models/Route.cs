using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Route
    {
        [Key]
        public int RouteID { get; set; }

        [Required]
        public int EventID { get; set; }

        public decimal DistanceKM { get; set; }

        public decimal? ElevationGain { get; set; }

        [MaxLength(200)]
        public string? StartLocation { get; set; }

        [MaxLength(200)]
        public string? FinishLocation { get; set; }

        public string? RouteDescription { get; set; }

        [MaxLength(500)]
        public string? MapURL { get; set; }

        public Event? Event { get; set; }
    }
}