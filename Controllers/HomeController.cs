using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using BusManagementSystem.Models;
using BusManagementSystem.Service;
using BusManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BusManagementSystem.Controllers;

[Authorize]
// [Authorize(Policy = "ActiveOnly")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDriverServiceInterface _driverServiceInterface;
    private readonly UserManager<Driver> _userManager;

    public HomeController(ILogger<HomeController> logger, IDriverServiceInterface driverServiceInterface,
        UserManager<Driver> userManager)
    {
        _logger = logger;
        _driverServiceInterface = driverServiceInterface;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Dashboard()
    {
        var drivers = _driverServiceInterface.GetDrivers();

        var activeUsers = await _userManager.GetUsersForClaimAsync(new Claim("IsActive", "true"));
        var activeUserIds = activeUsers.Select(u => u.Id).ToList(); // Fetch and store IDs
        
        drivers = drivers.Where(d => !d.IsManager && !activeUserIds.Contains(d.Id)).ToList();

        return View(drivers);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}