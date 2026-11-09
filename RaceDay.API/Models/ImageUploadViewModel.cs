using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{
    public class ImageUploadViewModel
    {
        [Required]
        public IFormFile? File { get; set; }
    }
}