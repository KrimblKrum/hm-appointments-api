using AppointmentsApi.Data;
using AppointmentsApi.Data.Entities;
using AppointmentsApi.Models;

namespace AppointmentsApi.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentsDbContext _dbContext;

        private readonly TimeSpan TIME_SLOT_INTERVAL = TimeSpan.FromMinutes(15);
        private readonly TimeSpan MAX_RESERVATION_HOLD = TimeSpan.FromMinutes(30);

        public AppointmentService(IAppointmentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public AppointmentEntity? CreateAppointment(CreateAppointmentRequest model)
        {
            // TODO: Replace with mapper?
            var entity = new AppointmentEntity
            {
                AppointmentId = Guid.NewGuid(),
                ClientId = model.ClientId,
                ProviderId = model.ProviderId,
                StartUtc = model.StartUtc,
                EndUtc = model.EndUtc,
            };

            _dbContext.Appointments?.Add(entity);

            if(_dbContext.SaveChanges() != 1) return null;

            return entity;
        }

        public bool ConfirmAppointment(Guid appointmentId)
        {
            var record = _dbContext.Appointments?.FirstOrDefault(i => i.AppointmentId == appointmentId);

            if (record == null) return false;

            record.IsConfirmed = true;

            return _dbContext.SaveChanges() == 1;
        }

        public IEnumerable<TimeSpan> GetAvailability(Guid providerId, DateTime requestUtc)
        {
            var daySchedule = _dbContext.Schedules?
                .FirstOrDefault(i => i.ProviderId == providerId && IsSameDate(i.StartUtc, requestUtc));

            if (daySchedule == null)
            {
                return new TimeSpan[] { };
            }

            var allTimeSlots = GetTimeSlots(daySchedule.StartUtc.TimeOfDay, daySchedule.EndUtc.TimeOfDay, TIME_SLOT_INTERVAL);

            var booked = _dbContext.Appointments?
                .Where(i =>
                    i.ProviderId == providerId &&
                    IsSameDate(i.StartUtc, requestUtc) &&
                    (!i.IsConfirmed || i.CreatedUtc.Add(MAX_RESERVATION_HOLD) > DateTime.UtcNow)
                )
                .Select(i => i.StartUtc.TimeOfDay);

            var available = from slot in allTimeSlots
                            join appt in booked on slot equals appt into grp
                            from apptSlot in grp.DefaultIfEmpty()
                            where apptSlot == TimeSpan.Zero
                            select slot;

            return available;
        }

        private bool IsSameDate(DateTime value1, DateTime value2)
        {
            return value1.ToShortDateString() == value2.ToShortDateString();
        }

        private TimeSpan[] GetTimeSlots(TimeSpan startTime, TimeSpan endTime, TimeSpan interval)
        {
            if (startTime > endTime) throw new AppointmentApiException($"{nameof(startTime)} must be less than {nameof(endTime)}.");

            var rVal = new List<TimeSpan>();
            var curTime = startTime;

            while (curTime < endTime)
            {
                rVal.Add(curTime);
                curTime = curTime.Add(interval);
            }

            return rVal.ToArray();
        }
    }
}
