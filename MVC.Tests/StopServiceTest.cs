using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using BusManagementSystem.Models;
using BusManagementSystem.Service;

namespace MVC.Tests;

public class StopServiceTests
{
    private BusContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<BusContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb-{Guid.NewGuid()}")
            .Options;

        return new BusContext(options);
    }

    [Fact]
    public async Task TestAddAndGetStop()
    {
        var dbContext = GetDbContext();
        var repository = new StopService(dbContext);
        var newStop = new Stop { Name = "Test Stop", Latitude = 0.0, Longitude = 0.0 };

        var stopId = await repository.AddStop(newStop);
        var addedStop = await repository.GetStop(stopId);

        Assert.NotNull(addedStop);
        Assert.Equal("Test Stop", addedStop.Name);
        Assert.Equal(0.0, addedStop.Latitude);
        Assert.Equal(0.0, addedStop.Longitude);
    }

    [Fact]
    public async Task TestGetStop()
    {
        var dbContext = GetDbContext();
        var repository = new StopService(dbContext);
        var stop1 = new Stop { Name = "Stop 1", Latitude = 0.0, Longitude = 0.0 };
        var stop2 = new Stop { Name = "Stop 2", Latitude = 1.0, Longitude = 1.0 };

        await repository.AddStop(stop1);
        await repository.AddStop(stop2);

        var stops = repository.GetStops();

        Assert.NotNull(stops);
        Assert.Equal(2, stops.Count);
        Assert.Contains(stops, stop => stop.Name == "Stop 1" && stop.Latitude == 0.0 && stop.Longitude == 0.0);
        Assert.Contains(stops, stop => stop.Name == "Stop 2" && stop.Latitude == 1.0 && stop.Longitude == 1.0);
    }

    [Fact]
    public async Task TestUpdateStop()
    {
        var dbContext = GetDbContext();
        var repository = new StopService(dbContext);
        var newStop = new Stop { Name = "Test Stop", Latitude = 0.0, Longitude = 0.0 };
        var stopId = await repository.AddStop(newStop);
        newStop.Id = stopId;
        newStop.Latitude = 123.456;

        var updatedStop = await repository.UpdateStop(newStop);

        Assert.NotNull(updatedStop);
        Assert.Equal(123.456, updatedStop.Latitude);
    }

    [Fact]
    public async Task TestDeleteStop()
    {
        var dbContext = GetDbContext();
        var repository = new StopService(dbContext);
        var stop = new Stop { Name = "Stop 1", Latitude = 0.0, Longitude = 0.0 };
        var stopId = await repository.AddStop(stop);

        await repository.DeleteStop(stopId);
        var remainingStops = repository.GetStops();

        Assert.Empty(remainingStops);
    }
}