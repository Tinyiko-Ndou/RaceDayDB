using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class CategoryRequest
    {
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
