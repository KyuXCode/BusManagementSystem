using Microsoft.EntityFrameworkCore;
using BusManagementSystem.Models;
using BusManagementSystem.Service;

namespace MVC.Tests;

public class LoopRepoTests
{
    private BusContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<BusContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb-{Guid.NewGuid()}")
            .Options;

        return new BusContext(options);
    }

    [Fact]
    public async Task TestAddAndGetLoop()
    {
        var dbContext = GetDbContext();
        var repository = new LoopService(dbContext);
        var newLoop = new Loop { Name = "Test Loop" };

        var loopId = await repository.AddLoop(newLoop);
        var addedLoop = await repository.GetLoop(loopId);

        Assert.NotNull(addedLoop);
        Assert.Equal("Test Loop", addedLoop.Name);
    }

    [Fact]
    public async Task TestGetLoops()
    {
        var dbContext = GetDbContext();
        var repository = new LoopService(dbContext);
        var loop1 = new Loop { Name = "Loop 1" };
        var loop2 = new Loop { Name = "Loop 2" };

        await repository.AddLoop(loop1);
        await repository.AddLoop(loop2);

        var loops = repository.GetLoops();

        Assert.NotNull(loops);
        Assert.Equal(2, loops.Count);
        Assert.Contains(loops, loop => loop.Name == "Loop 1");
        Assert.Contains(loops, loop => loop.Name == "Loop 2");
    }

    [Fact]
    public async Task TestUpdateLoop()
    {
        var dbContext = GetDbContext();
        var repository = new LoopService(dbContext);
        var newLoop = new Loop { Name = "Test Loop" };
        var loopId = await repository.AddLoop(newLoop);
        newLoop.Id = loopId;
        newLoop.Name = "Updated Loop";

        var updatedLoop = await repository.UpdateLoop(newLoop);

        Assert.NotNull(updatedLoop);
        Assert.Equal("Updated Loop", updatedLoop.Name);
    }

    [Fact]
    public async Task TestDeleteLoops()
    {
        var dbContext = GetDbContext();
        var repository = new LoopService(dbContext);
        var loop = new Loop { Name = "Loop 1" };
        var loopId = await repository.AddLoop(loop);

        await repository.DeleteLoop(loopId);
        var remainingLoops = repository.GetLoops();

        Assert.Empty(remainingLoops);
    }
}