namespace AppointmentsApi.Models
{
    public class CreateScheduleRequest
    {
        public Guid ProviderId { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
    }
}
