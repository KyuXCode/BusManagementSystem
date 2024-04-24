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
        var busService = GetInMemoryService();
        var busController = new BusController(busService, GetLogger());

        var result = busController.Index() as ViewResult;

        Assert.NotNull(result);
        Assert.IsAssignableFrom<List<Bus>>(result.Model);
        Assert.Empty((List<Bus>)result.Model);
    }

    [Theory]
    [InlineData(222, true)]
    public async Task TestCreateReturnsRedirectToActionIndex(int busNumber, bool expectRedirect)
    {
        var busService = GetInMemoryService();
        var busController = new BusController(busService, GetLogger());
        var bus = new Bus { BusNumber = busNumber };

        var result = await busController.Create(bus) as RedirectToActionResult;

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
        var busService = GetInMemoryService();
        var busController = new BusController(busService, GetLogger());
        var bus = new Bus { BusNumber = 123 };
        var busId = await busService.AddBus(bus);
        bus.BusNumber = 222;

        var result = await busController.Edit(busId, bus) as RedirectToActionResult;

        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        Assert.Single(busService.GetBuses());
        Assert.Equal(222, (await busService.GetBus(busId)).BusNumber);
    }

    [Fact]
    public async Task TestEditConfirmedMismatchedCode()
    {
        var busService = GetInMemoryService();
        var busController = new BusController(busService, GetLogger());
        var bus = new Bus { BusNumber = 123 };
        var busId = await busService.AddBus(bus);
        var mismatchedId = busId * 10;

        var result = await busController.Edit(mismatchedId, bus) as ActionResult;

        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task TestDeleteReturnsRedirectToActionIndexOnSuccess()
    {
        var busService = GetInMemoryService();
        var busController = new BusController(busService, GetLogger());
        var bus = new Bus { BusNumber = 123 };
        var busId = await busService.AddBus(bus);

        var result = await busController.Delete(busId) as RedirectToActionResult;

        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        Assert.Empty(busService.GetBuses());
    }
}