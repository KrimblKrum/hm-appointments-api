using AppointmentsApi.Controllers;
using AppointmentsApi.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using AppointmentsApi.Data.Entities;
using AppointmentsApi.Data;
using AppointmentsApi.Services;

namespace AppointmentsApi.UnitTests.ControllerTests
{
    public class AppointmentControllerTests
    {
        public class GetAvailability
        {
            [Fact]
            public void ShouldReturnAListOfAvailableAppointments()
            {
                // arrange
                var request = new Models.GetAvailabilityRequest
                {
                    ProviderId = Guid.NewGuid(),
                    RequestDate = DateTime.UtcNow
                };
                var expected = new List<TimeSpan>
                {
                    TimeSpan.Parse("08:00"),
                    TimeSpan.Parse("08:15"),
                    TimeSpan.Parse("08:30"),
                    TimeSpan.Parse("08:45"),
                };

                var service = new Mock<IAppointmentService>();
                service.Setup(i => i.GetAvailability(request.ProviderId, request.RequestDate.GetValueOrDefault().Date)).Returns(expected);

                var controller = new AppointmentController(service.Object);

                // act
                var results = controller.GetAvailability(request);

                // assert 
                Assert.IsType<OkObjectResult>(results);
                Assert.Equal(expected, ((OkObjectResult)results).Value);
            }

            [Fact]
            public void ShouldReturnBadRequestWhenModelIsInvalid()
            {
                // arrange
                var request = new Models.GetAvailabilityRequest
                {
                    ProviderId = Guid.NewGuid(),
                    RequestDate = DateTime.UtcNow
                };

                var service = new Mock<IAppointmentService>();

                var controller = new AppointmentController(service.Object);
                controller.ModelState.AddModelError("ProviderId", "ProviderId is required.");

                // act
                var results = controller.GetAvailability(request);

                // assert 
                Assert.IsType<BadRequestResult>(results);
            }
        }
        public class Create
        {
            [Fact]
            public void ShouldReturnAListOfAvailableAppointments()
            {
                // arrange
                var request = new Models.CreateAppointmentRequest
                {
                    ProviderId = Guid.NewGuid(),
                    ClientId = Guid.NewGuid(),
                    StartUtc = DateTime.UtcNow.AddDays(1),
                    EndUtc = DateTime.UtcNow.AddDays(1).AddMinutes(15),
                };
                var expected = new AppointmentEntity
                {
                    AppointmentId = Guid.NewGuid(),
                    ProviderId = request.ProviderId,
                    ClientId = request.ClientId,
                    StartUtc = request.StartUtc,
                    EndUtc = request.EndUtc,
                };

                var service = new Mock<IAppointmentService>();
                service.Setup(i => i.CreateAppointment(request)).Returns(expected);

                var controller = new AppointmentController(service.Object);

                // act
                var results = controller.Create(request);

                // assert 
                Assert.IsType<OkObjectResult>(results);
                Assert.Equal(expected, ((OkObjectResult)results).Value);
            }

            [Fact]
            public void ShouldReturnBadRequestWhenModelIsInvalid()
            {
                // arrange
                var request = new Models.CreateAppointmentRequest
                {
                    ClientId = Guid.NewGuid(),
                    StartUtc = DateTime.UtcNow.AddDays(1),
                    EndUtc = DateTime.UtcNow.AddDays(1).AddMinutes(15),
                };

                var service = new Mock<IAppointmentService>();

                var controller = new AppointmentController(service.Object);
                controller.ModelState.AddModelError("ProviderId", "ProviderId is required.");

                // act
                var results = controller.Create(request);

                // assert 
                Assert.IsType<BadRequestResult>(results);
            }

            [Fact]
            public void ShouldReturnBadRequestWhenStartAndEndDateDoNotMatch()
            {
                // arrange
                var request = new Models.CreateAppointmentRequest
                {
                    ClientId = Guid.NewGuid(),
                    StartUtc = DateTime.UtcNow.AddDays(1),
                    EndUtc = DateTime.UtcNow.AddDays(2),
                };

                var service = new Mock<IAppointmentService>();

                var controller = new AppointmentController(service.Object);

                // act
                var results = controller.Create(request);

                // assert 
                Assert.IsType<BadRequestObjectResult>(results);
            }

            [Fact]
            public void ShouldReturnBadRequestWhenStartDateIsCurrentDate()
            {
                // arrange
                var request = new Models.CreateAppointmentRequest
                {
                    ClientId = Guid.NewGuid(),
                    StartUtc = DateTime.UtcNow,
                    EndUtc = DateTime.UtcNow,
                };

                var service = new Mock<IAppointmentService>();

                var controller = new AppointmentController(service.Object);

                // act
                var results = controller.Create(request);

                // assert 
                Assert.IsType<BadRequestObjectResult>(results);
            }
        }
        public class Confirm
        {
            [Fact]
            public void ShouldReturnAListOfAvailableAppointments()
            {
                // arrange
                var appointmentId = Guid.NewGuid();

                var service = new Mock<IAppointmentService>();
                service.Setup(i => i.ConfirmAppointment(appointmentId)).Returns(true);

                var controller = new AppointmentController(service.Object);

                // act
                var results = controller.Confirm(appointmentId);

                // assert 
                Assert.IsType<OkObjectResult>(results);
            }

            [Fact]
            public void ShouldReturnBadRequestWhenAppointmentIdIsInvalid()
            {
                // arrange
                var appointmentId = Guid.Empty;

                var service = new Mock<IAppointmentService>();

                var controller = new AppointmentController(service.Object);
                controller.ModelState.AddModelError("appointmentId", "appointmentId is required.");

                // act
                var results = controller.Confirm(appointmentId);

                // assert 
                Assert.IsType<BadRequestObjectResult>(results);
            }

            [Fact]
            public void ShouldReturnNotFoundWhenAppointmentIdIsNotFound()
            {
                // arrange
                var appointmentId = Guid.NewGuid();

                var service = new Mock<IAppointmentService>();

                var controller = new AppointmentController(service.Object);
                service.Setup(i => i.ConfirmAppointment(appointmentId)).Returns(false);

                // act
                var results = controller.Confirm(appointmentId);

                // assert 
                Assert.IsType<NotFoundObjectResult>(results);
            }
        }
    }
}
