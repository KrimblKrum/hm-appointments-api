namespace AppointmentsApi.Models
{
    public class CreateAppointmentRequest
    {
        public Guid ClientId { get; set; }
        public Guid ProviderId { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
    }
}
