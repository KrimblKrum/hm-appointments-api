using AppointmentsApi.Data.Entities;
using AppointmentsApi.Data;
using AppointmentsApi.UnitTests.Shared;
using Moq;
using Xunit;
using AppointmentsApi.Services;
using AppointmentsApi.Models;

namespace AppointmentsApi.UnitTests.ServicesTests
{
    public class AppointmentServiceTests
    {
        public class GetAvailability
        {
            [Fact]
            public void ShouldReturnAListOfAvailableAppointments()
            {
                // arrange
                var providerId = Guid.Parse("a0000000-0000-0000-0000-000000000001");
                var requestDate = DateTime.Parse("2023-09-02");

                var expected = new List<TimeSpan>
                {
                    TimeSpan.Parse("08:00"),
                    TimeSpan.Parse("08:15"),
                };
                var schdDbSet = new FakeDbSet<ScheduleEntity>();
                schdDbSet.AddRange(
                    new ScheduleEntity
                    {
                        ScheduleId = Guid.NewGuid(),
                        ProviderId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                        StartUtc = DateTime.Parse("2023-09-02T08:00Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                        EndUtc = DateTime.Parse("2023-09-02T09:00Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                    }
                );
                var apptDbSet = new FakeDbSet<AppointmentEntity>();
                apptDbSet.AddRange(
                    new AppointmentEntity
                    {
                        AppointmentId = Guid.NewGuid(),
                        ProviderId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                        ClientId = Guid.Parse("b0000000-0000-0000-0000-000000000001"),
                        StartUtc = DateTime.Parse("2023-09-02T08:00Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                        EndUtc = DateTime.Parse("2023-09-02T08:14Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                    },
                    new AppointmentEntity
                    {
                        AppointmentId = Guid.NewGuid(),
                        ProviderId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                        ClientId = Guid.Parse("b0000000-0000-0000-0000-000000000002"),
                        StartUtc = DateTime.Parse("2023-09-02T08:30Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                        EndUtc = DateTime.Parse("2023-09-02T08:44Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                    }
                );
                var dbContext = new Mock<IAppointmentsDbContext>();
                dbContext.SetupGet(i => i.Appointments).Returns(apptDbSet);
                dbContext.SetupGet(i => i.Schedules).Returns(schdDbSet);

                var service = new AppointmentService(dbContext.Object);

                // act
                var results = service.GetAvailability(providerId, requestDate);

                // assert 
                Assert.Collection(results,
                    i => Assert.Equal(TimeSpan.Parse("08:15"), i),
                    i => Assert.Equal(TimeSpan.Parse("08:45"), i)
                );
            }

            [Fact]
            public void ShouldReturnNullWhenSaveFails()
            {
                // arrange
                var request = new Models.CreateAppointmentRequest
                {
                    ClientId = Guid.NewGuid(),
                    ProviderId = Guid.NewGuid(),
                    StartUtc = DateTime.UtcNow,
                    EndUtc = DateTime.UtcNow.AddMinutes(15),
                };
                var dbContext = new Mock<IAppointmentsDbContext>();

                var service = new AppointmentService(dbContext.Object);

                // act
                var results = service.CreateAppointment(request);

                // assert 
                Assert.Null(results);
            }

            [Fact]
            public void ShouldReturnAnEmptyListWhenNoAppointmentsAreAvailable()
            {
                // arrange
                var providerId = Guid.NewGuid();
                var requestDate = DateTime.UtcNow.Date;

                var expected = new List<TimeSpan>();

                var dbSet = new FakeDbSet<AppointmentEntity>();
                var dbContext = new Mock<IAppointmentsDbContext>();
                dbContext.SetupGet(i => i.Appointments).Returns(dbSet);

                var service = new AppointmentService(dbContext.Object);

                // act
                var results = service.GetAvailability(providerId, requestDate);

                // assert
                Assert.Equal(expected, results);
            }
        }

        public class CreateAppointment
        {
            [Fact]
            public void ShouldReturnNewEntityWhenAppointmentIsCreated()
            {
                // arrange
                var appointmentRequest = new CreateAppointmentRequest
                {
                    ProviderId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                    ClientId = Guid.Parse("b0000000-0000-0000-0000-000000000001"),
                    StartUtc = DateTime.Parse("2023-09-02T08:00Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                    EndUtc = DateTime.Parse("2023-09-02T08:14Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                };

                var apptDbSet = new FakeDbSet<AppointmentEntity>();
                var dbContext = new Mock<IAppointmentsDbContext>();
                dbContext.SetupGet(i => i.Appointments).Returns(apptDbSet);
                dbContext.Setup(i => i.SaveChanges()).Returns(1);

                var service = new AppointmentService(dbContext.Object);

                // act
                var results = service.CreateAppointment(appointmentRequest);

                // assert 
                Assert.NotEqual(Guid.Empty, results?.AppointmentId);
                Assert.Equivalent(new AppointmentEntity
                {
                    AppointmentId = results?.AppointmentId ?? Guid.Empty,
                    ProviderId = appointmentRequest.ProviderId,
                    ClientId = appointmentRequest.ClientId,
                    StartUtc = appointmentRequest.StartUtc,
                    EndUtc = appointmentRequest.EndUtc,
                    CreatedUtc = results?.CreatedUtc ?? DateTime.MinValue,
                }, results);
            }

            [Fact]
            public void ShouldReturnNullWhenAppointmentIsNotSaved()
            {
                // arrange
                var appointmentRequest = new CreateAppointmentRequest
                {
                    ProviderId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                    ClientId = Guid.Parse("b0000000-0000-0000-0000-000000000001"),
                    StartUtc = DateTime.Parse("2023-09-02T08:00Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                    EndUtc = DateTime.Parse("2023-09-02T08:14Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                };

                var apptDbSet = new FakeDbSet<AppointmentEntity>();
                var dbContext = new Mock<IAppointmentsDbContext>();
                dbContext.SetupGet(i => i.Appointments).Returns(apptDbSet);

                var service = new AppointmentService(dbContext.Object);

                // act
                var results = service.CreateAppointment(appointmentRequest);

                // assert 
                Assert.Null(results);
            }
        }

        public class ConfirmAppointment
        {
            [Fact]
            public void ShouldReturnTrueWhenAppointmentIsConfirmed()
            {
                // arrange
                var appointmentId = Guid.NewGuid();

                var apptDbSet = new FakeDbSet<AppointmentEntity>();
                apptDbSet.AddRange(
                    new AppointmentEntity
                    {
                        AppointmentId = appointmentId,
                        ProviderId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                        ClientId = Guid.Parse("b0000000-0000-0000-0000-000000000001"),
                        StartUtc = DateTime.Parse("2023-09-02T08:00Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                        EndUtc = DateTime.Parse("2023-09-02T08:14Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                    }
                );
                var dbContext = new Mock<IAppointmentsDbContext>();
                dbContext.SetupGet(i => i.Appointments).Returns(apptDbSet);
                dbContext.Setup(i => i.SaveChanges()).Returns(1);

                var service = new AppointmentService(dbContext.Object);

                // act
                var results = service.ConfirmAppointment(appointmentId);

                // assert 
                Assert.True(results);
            }

            [Fact]
            public void ShouldReturnFalseWhenAppointmentIsNotFound()
            {
                // arrange
                var appointmentId = Guid.NewGuid();

                var apptDbSet = new FakeDbSet<AppointmentEntity>();
                var dbContext = new Mock<IAppointmentsDbContext>();
                dbContext.SetupGet(i => i.Appointments).Returns(apptDbSet);

                var service = new AppointmentService(dbContext.Object);

                // act
                var results = service.ConfirmAppointment(appointmentId);

                // assert 
                Assert.False(results);
            }

            [Fact]
            public void ShouldReturnFalseWhenAppointmentIsNotSaved()
            {
                // arrange
                var appointmentId = Guid.NewGuid();

                var apptDbSet = new FakeDbSet<AppointmentEntity>();
                apptDbSet.AddRange(
                    new AppointmentEntity
                    {
                        AppointmentId = appointmentId,
                        ProviderId = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                        ClientId = Guid.Parse("b0000000-0000-0000-0000-000000000001"),
                        StartUtc = DateTime.Parse("2023-09-02T08:00Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                        EndUtc = DateTime.Parse("2023-09-02T08:14Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                    }
                );
                var dbContext = new Mock<IAppointmentsDbContext>();
                dbContext.SetupGet(i => i.Appointments).Returns(apptDbSet);

                var service = new AppointmentService(dbContext.Object);

                // act
                var results = service.ConfirmAppointment(appointmentId);

                // assert 
                Assert.False(results);
            }
        }
    }
}
