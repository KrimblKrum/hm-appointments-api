using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentsApi.Data.Entities
{
    [Table("Schedule")]
    public class ScheduleEntity
    {
        [Key]
        [Required]
        public Guid ScheduleId { get; set; }

        [Required]
        public Guid ProviderId { get; set; }

        [Required]
        public DateTime StartUtc { get; set; }

        [Required]
        public DateTime EndUtc { get; set; }
    }
}
