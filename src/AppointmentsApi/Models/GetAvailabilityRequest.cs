using System.ComponentModel.DataAnnotations;

namespace AppointmentsApi.Models
{
    public class GetAvailabilityRequest
    {
        [Required]
        public Guid ProviderId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? RequestDate { get; set; }
    }
}
