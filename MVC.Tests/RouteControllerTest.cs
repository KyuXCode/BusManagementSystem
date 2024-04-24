using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using BusManagementSystem.Controllers;
using BusManagementSystem.Models;
using BusManagementSystem.Service;
using Microsoft.EntityFrameworkCore;

namespace BusManagementSystem.Tests
{
    public class RouteControllerTests
    {
        private ILogger<RouteController> GetLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            return loggerFactory.CreateLogger<RouteController>();
        }

        private ILoopServiceInterface GetInMemoryLoopService()
        {
            var options = new DbContextOptionsBuilder<BusContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb{Guid.NewGuid()}")
                .Options;

            var dbContext = new BusContext(options);
            return new LoopService(dbContext);
        }

        private IRouteServiceInterface GetInMemoryService()
        {
            var options = new DbContextOptionsBuilder<BusContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb{Guid.NewGuid()}")
                .Options;

            var dbContext = new BusContext(options);
            return new RouteService(dbContext, GetInMemoryLoopService());
        }

        [Fact]
        public async Task TestIndexReturnsViewWithListOfRoutes()
        {
            var loopService = GetInMemoryLoopService();
            var routeService = GetInMemoryService();
            var routeController = new RouteController(routeService, GetLogger());
            var loop = new Loop { Name = "Green" };
            var loopId = await loopService.AddLoop(loop);


            var result = routeController.Index(loopId) as ViewResult;
            var model = result.Model as IEnumerable<Route>;

            Assert.NotNull(result);
            Assert.NotNull(model);
            Assert.Empty(model);
        }

        [Theory]
        [InlineData(1, 2, 1, true)]
        public async Task TestCreateReturnsRedirectToActionIndex(int loopId, int stopId, int order, bool expectRedirect)
        {
            var loopService = GetInMemoryLoopService();
            var routeService = GetInMemoryService();
            var routeController = new RouteController(routeService, GetLogger());

            int initialRouteCount = routeService.GetRoutes().Count;

            var route = new Route { LoopId = loopId, StopId = stopId, Order = order };

            IActionResult actionResult = await routeController.Create(route, loopId);

            if (expectRedirect)
            {
                var result = Assert.IsType<RedirectResult>(actionResult);
                Assert.Equal($"/Route/loop/{loopId}", result.Url);
            }
            else
            {
                Assert.Null(actionResult);
            }

            int finalRouteCount = routeService.GetRoutes().Count;

            if (expectRedirect)
            {
                Assert.Equal(initialRouteCount + 1, finalRouteCount);
            }
            else
            {
                Assert.Equal(initialRouteCount, finalRouteCount);
            }
        }


        //
        // [Fact]
        // public async Task TestDelete()
        // {
        //     var mockRouteService = new Mock<IRouteServiceInterface>();
        //     var mockLogger = new Mock<ILogger<RouteController>>();
        //     var controller = new RouteController(mockRouteService.Object, mockLogger.Object);
        //     var routeId = 1;
        //     var loopId = 1;
        //     var route = new Route { Id = routeId, LoopId = loopId }; 
        //
        //     mockRouteService.Setup(s => s.GetRoute(routeId)).ReturnsAsync(route);
        //
        //     var result = await controller.Delete(routeId) as RedirectResult;
        //
        //     Assert.NotNull(result);
        //     Assert.Equal($"/Route/loop/{loopId}", result.Url);
        //     mockRouteService.Verify(s => s.DeleteRoutes(routeId), Times.Once);
        //     mockLogger.Verify(l => l.LogInformation(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Once);
        // }
        
        
        [Fact]
        public async Task TestMoveUp()
        {
            var mockRouteService = new Mock<IRouteServiceInterface>();
            var mockLogger = new Mock<ILogger<RouteController>>();
            var controller = new RouteController(mockRouteService.Object, mockLogger.Object);
            var routeId = 1;
            var loopId = 1;
        
            var result = await controller.MoveUp(routeId, loopId) as RedirectToActionResult;
        
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(loopId, result.RouteValues["loopId"]);
            mockRouteService.Verify(s => s.MoveRouteUp(routeId), Times.Once);
        }
        
        [Fact]
        public async Task TestMoveDown()
        {
            var mockRouteService = new Mock<IRouteServiceInterface>();
            var mockLogger = new Mock<ILogger<RouteController>>();
            var controller = new RouteController(mockRouteService.Object, mockLogger.Object);
            var routeId = 1;
            var loopId = 1;
        
            var result = await controller.MoveDown(routeId, loopId) as RedirectToActionResult;
        
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal(loopId, result.RouteValues["loopId"]);
            mockRouteService.Verify(s => s.MoveRouteDown(routeId), Times.Once);
        }
    }
}