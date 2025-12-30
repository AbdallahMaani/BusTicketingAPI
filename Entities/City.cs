using System.ComponentModel.DataAnnotations;

namespace Bus_ticketingAPI.Models
{
    public class City
    {
        [Key]
        public string Id { get; set; } // e.g., "LOC_AMN"

        [Required]
        public string NameEn { get; set; }

        [Required]
        public string NameAr { get; set; }

        // Navigation Property
        public ICollection<Station> Areas { get; set; }
    }
}