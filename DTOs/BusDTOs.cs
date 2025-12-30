using System.ComponentModel.DataAnnotations;

namespace Bus_ticketing_Backend.DTOs
{
    public class BusDto
    {
        public Guid BusId { get; set; }
        public string Operator { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
        public string Model { get; set; }
        public int ModelYear { get; set; }
        public string DriverName { get; set; }
        public string Features { get; set; }
    }

    public class CreateBusDto
    {
        [Required]
        public string Operator { get; set; }

        [Required]
        public string Type { get; set; }

        [Range(1, 50)]
        public int Capacity { get; set; }

        public string Model { get; set; }
        public int ModelYear { get; set; }
        public string DriverName { get; set; }
        public string Features { get; set; }
    }

    public class UpdateBusDto
    {
        public string Operator { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
        public string Model { get; set; }
        public int ModelYear { get; set; }
        public string DriverName { get; set; }
        public string Features { get; set; }
    }
}