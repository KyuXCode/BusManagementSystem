using BusManagementSystem.Controllers;
using BusManagementSystem.Models;
using BusManagementSystem.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MVC.Tests;

public class StopControllerTest
{
    private ILogger<StopController> GetLogger()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        return loggerFactory.CreateLogger<StopController>();
    }

    private IStopServiceInterface GetInMemoryService()
    {
        var options = new DbContextOptionsBuilder<BusContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb{Guid.NewGuid()}")
            .Options;

        var dbContext = new BusContext(options);
        return new StopService(dbContext);
    }

    [Fact]
    public async Task TestIndexReturnsViewWithListOfRoutes()
    {
        var stopService = GetInMemoryService();
        var stopController = new StopController(stopService, GetLogger());

        var result = stopController.Index() as ViewResult;

        Assert.NotNull(result);
        Assert.IsAssignableFrom<List<Stop>>(result.Model);
        Assert.Empty((List<Stop>)result.Model);
    }

    [Theory]
    [InlineData("AJ", 0, 0, true)]
    public async Task TestCreateReturnsRedirectToActionIndex(string stopName, double longitude, double latitude, bool expectRedirect)
    {
        var stopService = GetInMemoryService();
        var stopController = new StopController(stopService, GetLogger());
        var stop = new Stop { Name = stopName, Longitude = longitude, Latitude = latitude };

        IActionResult actionResult = await stopController.Create(stop);

        if (expectRedirect)
        {
            var result = Assert.IsType<RedirectToActionResult>(actionResult);
            Assert.Equal("Index", result.ActionName);
        }
        else
        {
            Assert.Null(actionResult);
        }

        Assert.Equal(expectRedirect ? 1 : 0, stopService.GetStops().Count);
    }

    [Fact]
    public async Task TestEditReturnsRedirectToActionIndexOnSuccess()
    {
        var stopService = GetInMemoryService();
        var stopController = new StopController(stopService, GetLogger());
        var stop = new Stop { Name = "AJ", Longitude = 0, Latitude = 0 };
        
        var stopId = await stopService.AddStop(stop);
        stop.Name = "North Dinning";

        var result = await stopController.Edit(stopId, stop) as RedirectToActionResult;

        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        Assert.Single(stopService.GetStops());
        Assert.Equal("North Dinning", (await stopService.GetStop(stopId)).Name);
    }
    
    [Fact]
    public async Task TestEditConfirmedMismatchedCode()
    {
        var stopService = GetInMemoryService();
        var stopController = new StopController(stopService, GetLogger());
        var stop = new Stop { Name = "AJ", Longitude = 0, Latitude = 0 };
        
        var stopId = await stopService.AddStop(stop);
        
        var mismatchedId = stopId * 10;

        var result = await stopController.Edit(mismatchedId, stop) as ActionResult;

        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task TestDeleteReturnsRedirectToActionIndexOnSuccess()
    {
        var stopService = GetInMemoryService();
        var stopController = new StopController(stopService, GetLogger());
        var stop = new Stop { Name = "AJ", Longitude = 0, Latitude = 0 };
        
        var stopId = await stopService.AddStop(stop);

        var result = await stopController.Delete(stopId) as RedirectToActionResult;

        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        Assert.Empty(stopService.GetStops());
    }
}