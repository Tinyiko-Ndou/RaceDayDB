using System.ComponentModel.DataAnnotations;


namespace RaceDay.API.DTOs
{
    public class EventCategoryRequest
    {
        [Range(1, int.MaxValue)]
        public int CategoryID { get; set; }

        [Range(0, 1000000)]
        public decimal EntryFee { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxParticipants { get; set; }
    }
}
