using System.ComponentModel.DataAnnotations;

namespace Bus_ticketing_Backend.DTOs
{
    public class CityDto
    {
        public string Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public int StationCount { get; set; }
    }

    public class CreateCityDto
    {
        [Required]
        public string Id { get; set; } //"LOC_AMN"

        [Required]
        public string NameEn { get; set; }

        [Required]
        public string NameAr { get; set; }
    }

    public class UpdateCityDto
    {
        [Required]
        public string NameEn { get; set; }

        [Required]
        public string NameAr { get; set; }
    }
}