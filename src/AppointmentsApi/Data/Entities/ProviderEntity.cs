using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentsApi.Data.Entities
{
    [Table("Provider")]
    public class ProviderEntity
    {
        [Key]
        [Required]
        public Guid ProviderId { get; set; }

        [Required]
        [MinLength(1)]
        public string? Name { get; set; }
    }
}
