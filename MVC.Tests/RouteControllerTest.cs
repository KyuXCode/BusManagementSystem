// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using Moq;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using BusManagementSystem.Controllers;
// using BusManagementSystem.Models;
// using BusManagementSystem.Service;
// using Xunit;
//
// namespace BusManagementSystem.Tests
// {
//     public class RouteControllerTests
//     {
//         [Fact]
//         public async Task TestCreate()
//         {
//             var mockRouteService = new Mock<IRouteServiceInterface>();
//             var mockLogger = new Mock<ILogger<RouteController>>();
//             var controller = new RouteController(mockRouteService.Object, mockLogger.Object);
//             var route = new Route { Id = 1, Order = 1, StopId = 1, LoopId = 1 };
//
//             var result = await controller.Create(route, route.LoopId) as RedirectToActionResult;
//
//             Assert.NotNull(result);
//             Assert.Equal("Index", result.ActionName);
//             Assert.Equal(route.LoopId, result.RouteValues["loopId"]);
//             mockRouteService.Verify(s => s.AddRoute(route), Times.Once);
//         }
//
//         [Fact]
//         public async Task TestDelete()
//         {
//             var mockRouteService = new Mock<IRouteServiceInterface>();
//             var mockLogger = new Mock<ILogger<RouteController>>();
//             var controller = new RouteController(mockRouteService.Object, mockLogger.Object);
//             var routeId = 1;
//             var loopId = 1;
//             var route = new Route { Id = routeId, LoopId = loopId }; 
//
//             mockRouteService.Setup(s => s.GetRoute(routeId)).ReturnsAsync(route);
//
//             var result = await controller.Delete(routeId) as RedirectResult;
//
//             Assert.NotNull(result);
//             Assert.Equal($"/Route/loop/{loopId}", result.Url);
//             mockRouteService.Verify(s => s.DeleteRoutes(routeId), Times.Once);
//             mockLogger.Verify(l => l.LogInformation(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>()), Times.Once);
//         }
//
//         [Fact]
//         public async Task TestMoveUp()
//         {
//             var mockRouteService = new Mock<IRouteServiceInterface>();
//             var mockLogger = new Mock<ILogger<RouteController>>();
//             var controller = new RouteController(mockRouteService.Object, mockLogger.Object);
//             var routeId = 1;
//             var loopId = 1;
//
//             var result = await controller.MoveUp(routeId, loopId) as RedirectToActionResult;
//
//             Assert.NotNull(result);
//             Assert.Equal("Index", result.ActionName);
//             Assert.Equal(loopId, result.RouteValues["loopId"]);
//             mockRouteService.Verify(s => s.MoveRouteUp(routeId), Times.Once);
//         }
//
//         [Fact]
//         public async Task TestMoveDown()
//         {
//             var mockRouteService = new Mock<IRouteServiceInterface>();
//             var mockLogger = new Mock<ILogger<RouteController>>();
//             var controller = new RouteController(mockRouteService.Object, mockLogger.Object);
//             var routeId = 1;
//             var loopId = 1;
//
//             var result = await controller.MoveDown(routeId, loopId) as RedirectToActionResult;
//
//             Assert.NotNull(result);
//             Assert.Equal("Index", result.ActionName);
//             Assert.Equal(loopId, result.RouteValues["loopId"]);
//             mockRouteService.Verify(s => s.MoveRouteDown(routeId), Times.Once);
//         }
//     }
// }
