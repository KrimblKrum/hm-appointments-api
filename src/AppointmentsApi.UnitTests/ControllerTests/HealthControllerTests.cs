using Xunit;
using AppointmentsApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentsApi.UnitTests.ControllerTests
{
    public class HealthControllerTests
    {
        public class Get
        {
            [Fact]
            public void ShouldReturnOkResult()
            {
                // arrange
                var controller = new HealthController();

                // act
                var results = controller.Get();

                // assert
                Assert.IsType<OkObjectResult>(results);
            }
        }
    }
}