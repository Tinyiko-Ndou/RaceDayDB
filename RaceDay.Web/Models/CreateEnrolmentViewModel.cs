using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.Models
{
    public class CreateEnrolmentViewModel
    {
        [Required]
        public int EventID { get; set; }

        [Required]
        public int EventCategoryID { get; set; }
    }
}