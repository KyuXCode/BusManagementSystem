using Microsoft.EntityFrameworkCore;
using BusManagementSystem.Models;
using BusManagementSystem.Service;

namespace MVC.Tests;

public class BusServiceTests
{
    private BusContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<BusContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb{Guid.NewGuid()}")
            .Options;

        return new BusContext(options);
    }
    
    [Fact]
    public async Task TestAddAndGetBus()
    {
        var dbContext = GetDbContext();
        var service = new BusService(dbContext);
        var newBus = new Bus { BusNumber = 123 };

        var busId = await service.AddBus(newBus);
        var addedBus = await service.GetBus(busId);

        Assert.NotNull(addedBus);
        Assert.Equal(123, addedBus.BusNumber);
    }

    [Fact]
    public async Task TestGetBuses()
    {
        var dbContext = GetDbContext();
        var service = new BusService(dbContext);
        var bus1 = new Bus { BusNumber = 123 };
        var bus2 = new Bus { BusNumber = 456 };

        await service.AddBus(bus1);
        await service.AddBus(bus2);

        var buses = service.GetBuses();

        Assert.NotNull(buses);
        Assert.Equal(2, buses.Count);
        Assert.Contains(buses, bus => bus.BusNumber == 123);
        Assert.Contains(buses, bus => bus.BusNumber == 456);
    }

    [Fact]
    public async Task TestUpdateBus()
    {
        var dbContext = GetDbContext();
        var service = new BusService(dbContext);
        var newBus = new Bus { BusNumber = 123 };
        var busId = await service.AddBus(newBus);
        newBus.Id = busId;
        newBus.BusNumber = 456;

        var updatedBus = await service.UpdateBus(newBus);

        Assert.NotNull(updatedBus);
        Assert.Equal(456, updatedBus.BusNumber);
    }
    
    [Fact]
    public async void TestDeleteBuses()
    {
        var dbContext = GetDbContext();
        var service = new BusService(dbContext);
        var bus = new Bus { BusNumber = 123 };
        var busId = await service.AddBus(bus);

        await service.DeleteBus(busId);
        var remainingBuses = service.GetBuses();

        Assert.Empty(remainingBuses);
    }
}