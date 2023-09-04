using AppointmentsApi.Controllers;
using AppointmentsApi.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using AppointmentsApi.Data.Entities;
using AppointmentsApi.Data;

namespace AppointmentsApi.UnitTests.ControllerTests
{
    public class ScheduleControllerTests
    {
        public class CreateSchedule
        {
            [Fact]
            public void ShouldAllowProviderToSubmitAvailableTime()
            {
                // arrange
                var request = new Models.CreateScheduleRequest
                {
                    ProviderId = Guid.NewGuid(),
                    StartUtc = DateTime.Parse("2023-09-02T13:00:00Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                    EndUtc = DateTime.Parse("2023-09-02T21:00:00Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                };

                var dbSet = new FakeDbSet<ScheduleEntity>();
                var dbContext = new Mock<IAppointmentsDbContext>();
                dbContext.SetupGet(i => i.Schedules).Returns(dbSet);
                dbContext.Setup(i => i.SaveChanges()).Returns(1);

                var controller = new ScheduleController(dbContext.Object);

                // act
                var results = controller.Create(request);

                // assert
                Assert.IsType<OkObjectResult>(results);
                Assert.Collection(dbSet.InnerItems, i =>
                {
                    Assert.NotEqual(Guid.Empty, i.ScheduleId);
                    Assert.Equal(i.ProviderId, request.ProviderId);
                    Assert.Equal(i.StartUtc, request.StartUtc);
                    Assert.Equal(i.EndUtc, request.EndUtc);
                });
                dbContext.Verify(i => i.SaveChanges(), Times.Once);
            }

            [Fact]
            public void ShouldReturnBadRequestWhenModelIsInvalid()
            {
                // arrange
                var request = new Models.CreateScheduleRequest
                {
                    ProviderId = Guid.NewGuid(),
                    StartUtc = DateTime.Parse("2023-09-03T13:00:00Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                    EndUtc = DateTime.Parse("2023-09-02T21:00:00Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                };
                var dbContext = new Mock<IAppointmentsDbContext>();

                var controller = new ScheduleController(dbContext.Object);
                controller.ModelState.AddModelError("ProviderId", "ProviderId is required.");

                // act
                var results = controller.Create(request);

                // assert 
                Assert.IsType<BadRequestResult>(results);
            }

            [Fact]
            public void ShouldReturnBadRequestWhenStartUtcIsGreaterThanEndUtc()
            {
                // arrange
                var request = new Models.CreateScheduleRequest
                {
                    ProviderId = Guid.NewGuid(),
                    StartUtc = DateTime.Parse("2023-09-03T13:00:00Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                    EndUtc = DateTime.Parse("2023-09-02T21:00:00Z", null, System.Globalization.DateTimeStyles.RoundtripKind),
                };
                var dbContext = new Mock<IAppointmentsDbContext>();

                var controller = new ScheduleController(dbContext.Object);

                // act
                var results = controller.Create(request);

                // assert 
                Assert.IsType<BadRequestObjectResult>(results);
            }
        }
    }
}
