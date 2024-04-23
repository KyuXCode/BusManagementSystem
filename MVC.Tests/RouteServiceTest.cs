using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using BusManagementSystem.Models;
using BusManagementSystem.Service;

namespace MVC.Tests;

public class RouteServiceTests  
{
    Stop stop = new Stop()
    {
        Name = "Test",
        Latitude = 0.0,
        Longitude = 0.0
    };

    Loop loop = new Loop()
    {
        Name = "Test"
    };

    private BusContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<BusContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb-{Guid.NewGuid()}")
            .Options;

        return new BusContext(options);
    }

    ILoopServiceInterface loopServiceInterface;


    [Fact]
    public async Task TestAddAndGetRoute()
    {
        var dbContext = GetDbContext();
        var service = new RouteService(dbContext, loopServiceInterface);
        var newRoute = new Route { Order = 1, Stop = stop, Loop = loop };

        var routeId = await service.AddRoute(newRoute);
        var addedRoute = await service.GetRoute(routeId);

        Assert.NotNull(addedRoute);
        Assert.Equal(1, addedRoute.Order);
    }

    [Fact]
    public async Task TestGetRoutes()
    {
        var dbContext = GetDbContext();
        var service = new RouteService(dbContext, loopServiceInterface);
        var route1 = new Route { Order = 1, Stop = stop, Loop = loop };
        var route2 = new Route { Order = 2, Stop = stop, Loop = loop };

        await service.AddRoute(route1);
        await service.AddRoute(route2);

        var routes = service.GetRoutes(); 

        Assert.NotNull(routes);
        Assert.Equal(2, routes.Count);
    }


    [Fact]
    public async Task TestUpdateRoute()
    {
        var dbContext = GetDbContext();
        var service = new RouteService(dbContext, loopServiceInterface);
        var newRoute = new Route { Order = 1, Stop = stop, Loop = loop };
        var routeId = await service.AddRoute(newRoute);
        newRoute.Id = routeId;
        newRoute.Order = 420;

        var updatedRoute = await service.UpdateRoute(newRoute);

        Assert.NotNull(updatedRoute);
        Assert.Equal(420, updatedRoute.Order);
    }

    [Fact]
    public async Task TestDeleteRoutes()
    {
        var dbContext = GetDbContext();
        var service = new RouteService(dbContext, loopServiceInterface);
        var route = new Route { Order = 1, Stop = stop, Loop = loop };
        var routeId = await service.AddRoute(route);

        await service.DeleteRoutes(routeId);
        var remainingRoutes = service.GetRoutes();

        Assert.Empty(remainingRoutes);
    }
    
    [Fact]
        public async Task TestMoveRouteUp()
        {
            var dbContext = GetDbContext();
            var service = new RouteService(dbContext, loopServiceInterface);

            var loop = new Loop { Name = "Test Loop" };
            await dbContext.Loops.AddAsync(loop);
            await dbContext.SaveChangesAsync();

            var route1 = new Route { Order = 1, LoopId = loop.Id };
            var route2 = new Route { Order = 2, LoopId = loop.Id };
            await dbContext.Routes.AddRangeAsync(route1, route2);
            await dbContext.SaveChangesAsync();

            
            await service.MoveRouteUp(route2.Id);

            var updatedRoute1 = await dbContext.Routes.FindAsync(route1.Id);
            var updatedRoute2 = await dbContext.Routes.FindAsync(route2.Id);

            Assert.Equal(2, updatedRoute1.Order);
            Assert.Equal(1, updatedRoute2.Order);
        }

        [Fact]
        public async Task TestMoveRouteDown()
        {
            var dbContext = GetDbContext();
            var service = new RouteService(dbContext, loopServiceInterface);

            var loop = new Loop { Name = "Test Loop" };
            await dbContext.Loops.AddAsync(loop);
            await dbContext.SaveChangesAsync();

            var route1 = new Route { Order = 1, LoopId = loop.Id };
            var route2 = new Route { Order = 2, LoopId = loop.Id };
            await dbContext.Routes.AddRangeAsync(route1, route2);
            await dbContext.SaveChangesAsync();

            
            await service.MoveRouteDown(route1.Id);

            var updatedRoute1 = await dbContext.Routes.FindAsync(route1.Id);
            var updatedRoute2 = await dbContext.Routes.FindAsync(route2.Id);

            Assert.Equal(2, updatedRoute1.Order);
            Assert.Equal(1, updatedRoute2.Order);
        }
}