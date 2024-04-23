using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using BusManagementSystem.Controllers;
using BusManagementSystem.Models;
using BusManagementSystem.Service;
using BusManagementSystem.ViewModels;

namespace MVC.Tests;

public class DriverControllerTests
{
    Driver driver = new Driver()
    {
        FirstName = "Test",
        LastName = "Test",
        IsManager = false,
        IsActive = true
    };

    Bus bus = new Bus()
    {
        BusNumber = 1
    };

    Route route = new Route()
    {
        Order = 1
    };

    Loop loop = new Loop()
    {
        Name = "Test"
    };

    Stop stop = new Stop()
    {
        Name = "Test",
        Latitude = 0.0,
        Longitude = 0.0
    };

    private ILogger<DriverController> GetLogger()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        return loggerFactory.CreateLogger<DriverController>();
    }

    private BusContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<BusContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb{Guid.NewGuid()}")
            .Options;

        return new BusContext(options);
    }

    private ILoopServiceInterface GetInMemoryLoopService(BusContext dbContext)
    {
        return new LoopService(dbContext);
    }

    private IBusServiceInterface GetInMemoryBusService(BusContext dbContext)
    {
        return new BusService(dbContext);
    }

    private IRouteServiceInterface GetInMemoryRouteService(BusContext dbContext,
        ILoopServiceInterface loopServiceInterface)
    {
        return new RouteService(dbContext, loopServiceInterface);
    }

    private IDriverServiceInterface GetInMemoryDriverService(BusContext dbContext)
    {
        return new DriverService(dbContext);
    }

    [Fact]
    public async Task TestStartDriving()
    {
        var dbContext = GetInMemoryDbContext();
        var driverService = GetInMemoryDriverService(dbContext);
        var loopService = GetInMemoryLoopService(dbContext);
        var routeService = GetInMemoryRouteService(dbContext, loopService);
        var busService = GetInMemoryBusService(dbContext);
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
        var driverController = new DriverController(
            loopService, routeService, busService,
            new FakeUserManager(), new FakeSignInManager(),
            GetLogger(),
            mockHttpContextAccessor.Object);
        var busId = await busService.AddBus(bus);
        var loopId = await loopService.AddLoop(loop);

        var actionResult = await driverController.StartDriving(busId, loopId) as RedirectToActionResult;

        Assert.NotNull(actionResult);
        Assert.Equal("EntryCreator", actionResult.ActionName);
    }

    [Fact]
    public async Task TestEntryCreator()
    {
        var dbContext = GetInMemoryDbContext();
        var driverService = GetInMemoryDriverService(dbContext);
        var loopService = GetInMemoryLoopService(dbContext);
        var routeService = GetInMemoryRouteService(dbContext, loopService);
        var busService = GetInMemoryBusService(dbContext);
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
        var driverController = new DriverController(
            loopService, routeService, busService,
            new FakeUserManager(), new FakeSignInManager(),
            GetLogger(),
            mockHttpContextAccessor.Object);
        var busId = await busService.AddBus(bus);
        var loopId = await loopService.AddLoop(loop);

        var actionResult = await driverController.EntryCreator(busId, loopId) as ViewResult;

        var model = actionResult.ViewData.Model as EntryCreatorViewModel;

        Assert.NotNull(actionResult);
        Assert.True(model.BusId == busId && model.LoopId == loopId);
    }
}