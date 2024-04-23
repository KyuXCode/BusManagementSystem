using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using BusManagementSystem.Models;
using BusManagementSystem.Service;

namespace MVC.Tests;

public class EntryRepoTests
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

    private BusContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<BusContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb-{Guid.NewGuid()}")
            .Options;

        return new BusContext(options);
    }

    [Fact]
    public async Task TestAddAndGetEntry()
    {
        var dbContext = GetDbContext();
        var service = new EntryService(dbContext);
        var newEntry = new Entry { Timestamp = Convert.ToDateTime("4/23/2024"), Boarded = 0, LeftBehind = 0, Driver = driver, Bus = bus, Loop = loop, Stop = stop};

        var entryId = await service.AddEntry(newEntry);
        var addedEntry = await service.GetEntry(entryId);

        Assert.NotNull(addedEntry);
        Assert.Equal(Convert.ToDateTime("4/23/2024"), addedEntry.Timestamp);
        Assert.Equal(0, addedEntry.Boarded);
        Assert.Equal(0, addedEntry.LeftBehind);
    }

    [Fact]
    public async Task TestGetEntries()
    {
        var dbContext = GetDbContext();
        var service = new EntryService(dbContext);
        var entry1 = new Entry { Timestamp = Convert.ToDateTime("4/22/2024"), Boarded = 0, LeftBehind = 0, Driver = driver, Bus = bus, Loop = loop, Stop = stop };
        var entry2 = new Entry { Timestamp = Convert.ToDateTime("4/23/2024"), Boarded = 1, LeftBehind = 1, Driver = driver, Bus = bus, Loop = loop, Stop = stop };

        await service.AddEntry(entry1);
        await service.AddEntry(entry2);

        var entries = await service.GetEntries();

        Assert.NotNull(entries);
        Assert.Equal(2, entries.Count);
        Assert.Contains(entries, entry => entry.Timestamp == Convert.ToDateTime("4/22/2024") && entry.Boarded == 0 && entry.LeftBehind == 0);
        Assert.Contains(entries, entry => entry.Timestamp == Convert.ToDateTime("4/23/2024") && entry.Boarded == 1 && entry.LeftBehind == 1);
    }

    [Fact]
    public async Task TestUpdateEntry()
    {
        var dbContext = GetDbContext();
        var service = new EntryService(dbContext);
        var newEntry = new Entry { Timestamp = Convert.ToDateTime("4/23/2024"), Boarded = 0, LeftBehind = 0, Driver = driver, Bus = bus, Loop = loop, Stop = stop };
        var entryId = await service.AddEntry(newEntry);
        newEntry.Id = entryId;
        newEntry.Boarded = 35;

        var updatedEntry = await service.UpdateEntry(newEntry);

        Assert.NotNull(updatedEntry);
        Assert.Equal(35, updatedEntry.Boarded);
    }

    [Fact]
    public async Task TestDeleteEntries()
    {
        var dbContext = GetDbContext();
        var service = new EntryService(dbContext);
        var entry1 = new Entry { Timestamp = Convert.ToDateTime("4/23/2024"), Boarded = 0, LeftBehind = 0, Driver = driver, Bus = bus, Loop = loop, Stop = stop };
        var entry2 = new Entry { Timestamp = Convert.ToDateTime("4/23/2024"), Boarded = 1, LeftBehind = 1, Driver = driver, Bus = bus, Loop = loop, Stop = stop };
        var entry1Id = await service.AddEntry(entry1);
        var entry2Id = await service.AddEntry(entry2);

        await service.DeleteEntries([entry1Id, entry2Id]);
        var remainingEntries = await service.GetEntries();

        Assert.Empty(remainingEntries);
    }
}