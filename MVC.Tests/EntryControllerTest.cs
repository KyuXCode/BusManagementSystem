using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BusManagementSystem.Controllers;
using BusManagementSystem.Models;
using BusManagementSystem.Service;
using Microsoft.AspNetCore.Http;
using Moq;

namespace MVC.Tests;

public class EntryControllerTests
{
    Driver driver = new Driver()
    {
        FirstName = "Test",
        LastName = "Test",
        Email = "test@bsu.edu",
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

    private ILogger<EntryController> GetLogger()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        return loggerFactory.CreateLogger<EntryController>();
    }

    private BusContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<BusContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb{Guid.NewGuid()}")
            .Options;

        return new BusContext(options);
    }

    private IEntryServiceInterface GetInMemoryEntryService(BusContext dbContext)
    {
        return new EntryService(dbContext);
    }

    private ILoopServiceInterface GetInMemoryLoopService(BusContext dbContext)
    {
        return new LoopService(dbContext);
    }

    private IStopServiceInterface GetInMemoryStopService(BusContext dbContext)
    {
        return new StopService(dbContext);
    }

    private IBusServiceInterface GetInMemoryBusService(BusContext dbContext)
    {
        return new BusService(dbContext);
    }

    [Fact]
    public async Task TestIndex()
    {
        var dbContext = GetInMemoryDbContext();
        var loopService = GetInMemoryLoopService(dbContext);
        var stopService = GetInMemoryStopService(dbContext);
        var entryService = GetInMemoryEntryService(dbContext);
        var busService = GetInMemoryBusService(dbContext);

        var entryController = new EntryController(
            entryService, loopService, stopService, busService,
            new FakeUserManager(), GetLogger()
        );
        var result = await entryController.Index() as ViewResult;

        var model = result.ViewData.Model as List<Entry>;

        Assert.NotNull(model);
        Assert.Empty(model);
    }

    [Fact]
    public async Task TestEditConfirmed()
    {
        var dbContext = GetInMemoryDbContext();
        var loopService = GetInMemoryLoopService(dbContext);
        var stopService = GetInMemoryStopService(dbContext);
        var entryService = GetInMemoryEntryService(dbContext);
        var busService = GetInMemoryBusService(dbContext);
        var entryController = new EntryController(
            entryService, loopService, stopService, busService,
            new FakeUserManager(), GetLogger()
        );

        var entry = new Entry
        {
            Timestamp = Convert.ToDateTime("4/22/2024"), Boarded = 0, LeftBehind = 0, Driver = driver, Bus = bus,
            Loop = loop, Stop = stop
        };
        var entryId = await entryService.AddEntry(entry);

        var actionResult = await entryController.EditConfirmed(entryId, entry) as RedirectToActionResult;

        var result = entryService.GetEntry(entryId);

        Assert.NotNull(actionResult);
        Assert.Equal("Index", actionResult.ActionName);
        Assert.Single(entryService.GetEntries().Result);
        Assert.True(result.Result.Equals(entry));
    }

    [Fact]
    public async Task TestEditConfirmedMismatchedCode()
    {
        var dbContext = GetInMemoryDbContext();
        var loopService = GetInMemoryLoopService(dbContext);
        var stopService = GetInMemoryStopService(dbContext);
        var entryService = GetInMemoryEntryService(dbContext);
        var busService = GetInMemoryBusService(dbContext);
        var entryController = new EntryController(
            entryService, loopService, stopService, busService,
            new FakeUserManager(), GetLogger()
        );
        var entry = new Entry
        {
            Timestamp = Convert.ToDateTime("4/22/2024"), Boarded = 0, LeftBehind = 0, Driver = driver, Bus = bus,
            Loop = loop, Stop = stop
        };
        var entryId = await entryService.AddEntry(entry);
        var mismatchedId = entryId * 10;

        var actionResult = await entryController.EditConfirmed(mismatchedId, entry) as ActionResult;

        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Fact]
    public async Task TestDeleteConfirmed_WithValidIds()
    {
        var dbContext = GetInMemoryDbContext();
        var loopService = GetInMemoryLoopService(dbContext);
        var stopService = GetInMemoryStopService(dbContext);
        var entryServiceMock = new Mock<IEntryServiceInterface>();
        var busService = GetInMemoryBusService(dbContext);
        var userManagerMock = new Mock<FakeUserManager>();

        var entryController = new EntryController(
            entryServiceMock.Object, loopService, stopService, busService,
            userManagerMock.Object, GetLogger()
        );

        var ids = new List<int> { 1, 2, 3 }; 
        var idsString = string.Join(",", ids);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Query = new QueryCollection(
            new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "ids", idsString }
            });

        entryController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        var result = await entryController.DeleteConfirmed();

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);

        entryServiceMock.Verify(x => x.DeleteEntries(ids), Times.Once);
    }
}