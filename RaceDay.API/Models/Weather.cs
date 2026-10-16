using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Weather
    {
        [Key]
        public int WeatherID { get; set; }

        [Required]
        public int EventID { get; set; }

        public DateTime RecordedAt { get; set; }

        public decimal? TemperatureC { get; set; }

        public decimal? FeelsLikeC { get; set; }

        public int? Humidity { get; set; }

        public decimal? WindSpeedKPH { get; set; }

        [MaxLength(100)]
        public string? Conditions { get; set; }

        public Event? Event { get; set; }
    }
}
