using System.ComponentModel.DataAnnotations;

namespace Bus_ticketingAPI.Entities
{
    public class Bus
    {
        [Key]
        public Guid BusId { get; set; } = Guid.NewGuid();

        public string Operator { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
        public string Model { get; set; }
        public int ModelYear { get; set; }
        public string DriverName { get; set; }
        //public double Rating { get; set; }

        // Stored as a list of strings
        public string Features { get; set; }

    }
}