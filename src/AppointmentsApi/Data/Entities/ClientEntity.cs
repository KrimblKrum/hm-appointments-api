using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentsApi.Data.Entities
{
    [Table("Client")]
    public class ClientEntity
    {
        [Key]
        [Required]
        public Guid ClientId { get; set; }

        [Required]
        [MinLength(1)]
        public string? Name { get; set; }

        [Required]
        [RegularExpression("/^([a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,})$/", ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }
    }
}
