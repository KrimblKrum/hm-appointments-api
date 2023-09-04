using AppointmentsApi.Data.Entities;
using AppointmentsApi.Models;

namespace AppointmentsApi.Services
{
    public interface IAppointmentService
    {
        bool ConfirmAppointment(Guid appointmentId);
        AppointmentEntity? CreateAppointment(CreateAppointmentRequest model);
        IEnumerable<TimeSpan> GetAvailability(Guid providerId, DateTime requestUtc);
    }
}