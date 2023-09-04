using AppointmentsApi.Controllers;
using AppointmentsApi.UnitTests.Shared;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using AppointmentsApi.Data.Entities;
using AppointmentsApi.Data;

namespace AppointmentsApi.UnitTests.ControllerTests
{
    public class ClientControllerTests
    {
        public class Create
        {
            [Fact]
            public void ShouldCreateDbRecordAndReturnNewClient()
            {
                // arrange
                var request = new Models.CreateClientRequest
                {
                    Name = "Dr. Jekyll"
                };
                var dbSet = new FakeDbSet<ClientEntity>();
                var dbContext = new Mock<IAppointmentsDbContext>();
                dbContext.SetupGet(i => i.Clients).Returns(dbSet);
                dbContext.Setup(i => i.SaveChanges()).Returns(1);

                var controller = new ClientController(dbContext.Object);

                // act
                var results = controller.Create(request);

                // assert 
                Assert.IsType<OkObjectResult>(results);
                Assert.Collection(dbSet.InnerItems, i =>
                {
                    Assert.NotEqual(Guid.Empty, i.ClientId);
                    Assert.Equal(i.Name, request.Name);
                });
                dbContext.Verify(i => i.SaveChanges(), Times.Once);
            }

            [Fact]
            public void ShouldReturnBadRequestWhenModelIsInvalid()
            {
                // arrange
                var request = new Models.CreateClientRequest
                {
                    Name = ""
                };
                var dbContext = new Mock<IAppointmentsDbContext>();

                var controller = new ClientController(dbContext.Object);
                controller.ModelState.AddModelError("Name", "Name is required.");

                // act
                var results = controller.Create(request);

                // assert 
                Assert.IsType<BadRequestResult>(results);
            }
        }
    }
}
