using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BusManagementSystem.Controllers;
using BusManagementSystem.Models;
using BusManagementSystem.Service;
using Microsoft.EntityFrameworkCore;

namespace MVC.Tests;

public class BusControllerTests
{
    private ILogger<BusController> GetLogger()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        return loggerFactory.CreateLogger<BusController>();
    }

    private IBusServiceInterface GetInMemoryService()
    {
        var options = new DbContextOptionsBuilder<BusContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb{Guid.NewGuid()}")
            .Options;

        var dbContext = new BusContext(options);
        return new BusService(dbContext);
    }

    [Fact]
    public async Task TestIndexReturnsViewWithListOfBuses()
    {
        // Arrange
        var busService = GetInMemoryService();
        var busController = new BusController(busService, GetLogger());

        // Act
        var result = busController.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<List<Bus>>(result.Model);
        Assert.Empty((List<Bus>)result.Model);
    }

    [Theory]
    [InlineData(222, true)]
    public async Task TestCreateReturnsRedirectToActionIndex(int busNumber, bool expectRedirect)
    {
        // Arrange
        var busService = GetInMemoryService();
        var busController = new BusController(busService, GetLogger());
        var bus = new Bus { BusNumber = busNumber };

        // Act
        var result = await busController.Create(bus) as RedirectToActionResult;

        // Assert
        if (expectRedirect)
        {
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }
        else
        {
            Assert.Null(result);
        }

        Assert.Equal(expectRedirect ? 1 : 0, (busService.GetBuses()).Count);
    }

    [Fact]
    public async Task TestEditReturnsRedirectToActionIndexOnSuccess()
    {
        // Arrange
        var busService = GetInMemoryService();
        var busController = new BusController(busService, GetLogger());
        var bus = new Bus { BusNumber = 123 };
        var busId = await busService.AddBus(bus);

        // Act
        var result = await busController.Edit(busId, bus) as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        Assert.Single(busService.GetBuses());
        Assert.Equal(123, (await busService.GetBus(busId)).BusNumber);
    }

    [Fact]
    public async Task TestEditConfirmedMismatchedCode()
    {
        // Arrange
        var busService = GetInMemoryService();
        var busController = new BusController(busService, GetLogger());
        var bus = new Bus { BusNumber = 123 };
        var busId = await busService.AddBus(bus);
        var mismatchedId = busId * 10;

        // Act
        var result = await busController.Edit(mismatchedId, bus) as ActionResult;

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}