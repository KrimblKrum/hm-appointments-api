using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentsApi.Data.Entities
{
    [Table("Appointment")]
    public class AppointmentEntity
    {
        [Key]
        [Required]
        public Guid AppointmentId { get; set; }

        [Required]
        public Guid ClientId { get; set; }

        [Required]
        public Guid ProviderId { get; set; }

        [Required]
        public DateTime StartUtc { get; set; }

        [Required]
        public DateTime EndUtc { get; set; }

        [Required]
        public bool IsConfirmed { get; set; } = false;

        [Required]
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }
}
