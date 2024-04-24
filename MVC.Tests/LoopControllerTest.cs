using BusManagementSystem.Controllers;
using BusManagementSystem.Models;
using BusManagementSystem.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace MVC.Tests
{
    public class LoopControllerTests
    {
        private ILogger<LoopController> GetLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            return loggerFactory.CreateLogger<LoopController>();
        }

        private ILoopServiceInterface GetInMemoryService()
        {
            var options = new DbContextOptionsBuilder<BusContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb{Guid.NewGuid()}")
                .Options;

            var dbContext = new BusContext(options);
            return new LoopService(dbContext);
        }

        [Fact]
        public async Task TestIndexReturnsViewWithListOfLoops()
        {
            var loopService = GetInMemoryService();
            var loopController = new LoopController(loopService, GetLogger());

            var result = loopController.Index() as ViewResult;

            Assert.NotNull(result);
            Assert.IsAssignableFrom<List<Loop>>(result.Model);
            Assert.Empty((List<Loop>)result.Model);
        }
        
        [Theory]
        [InlineData("Green", true)]
        public async Task TestCreateReturnsRedirectToActionIndex(string loopName, bool expectRedirect)
        {
            var loopService = GetInMemoryService();
            var loopController = new LoopController(loopService, GetLogger());
            var loop = new Loop { Name = "Green" };

            var result = await loopController.Create(loop) as RedirectToActionResult;

            if (expectRedirect)
            {
                Assert.NotNull(result);
                Assert.Equal("Index", result.ActionName);
            }
            else
            {
                Assert.Null(result);
            }

            Assert.Equal(expectRedirect ? 1 : 0, (loopService.GetLoops()).Count);
        }

        [Fact]
        public async Task TestEditReturnsRedirectToActionIndexOnSuccess()
        {
            var loopService = GetInMemoryService();
            var loopController = new LoopController(loopService, GetLogger());
            var loop = new Loop { Name = "Green" };
            var loopId = await loopService.AddLoop(loop);
            loop.Name = "Purple";

            var result = await loopController.Edit(loopId, loop) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Single(loopService.GetLoops());
            Assert.Equal("Purple", (await loopService.GetLoop(loopId)).Name);
        }

        [Fact]
        public async Task TestDeleteReturnsRedirectToActionIndex()
        {
            var loopId = 1;
            var loopServiceMock = new Mock<ILoopServiceInterface>();
            loopServiceMock.Setup(service => service.DeleteLoop(loopId));
            var loggerMock = new Mock<ILogger<LoopController>>();
            var loopController = new LoopController(loopServiceMock.Object, loggerMock.Object);

            var result = await loopController.Delete(loopId) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            loopServiceMock.Verify(service => service.DeleteLoop(loopId), Times.Once);
        }
    }
}