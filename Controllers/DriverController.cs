using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusManagementSystem.Service;
using BusManagementSystem.ViewModels;
using BusManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BusManagementSystem.Controllers;

[Authorize]
public class DriverController : Controller
{
    private readonly ILoopServiceInterface _loopServiceInterface;
    private readonly IRouteServiceInterface _routeServiceInterface;
    private readonly IBusServiceInterface _busServiceInterface;

    private readonly UserManager<Driver> _userManager;
    private readonly SignInManager<Driver> _signInManager;
    private readonly ILogger<DriverController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DriverController(ILoopServiceInterface loopServiceInterface, IRouteServiceInterface routeServiceInterface,
        IBusServiceInterface busServiceInterface,
        UserManager<Driver> userManager, SignInManager<Driver> signInManager, ILogger<DriverController> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _loopServiceInterface = loopServiceInterface;
        _routeServiceInterface = routeServiceInterface;
        _busServiceInterface = busServiceInterface;
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> SelectBusLoop()
    {
        ViewData["BusId"] = new SelectList(_busServiceInterface.GetBuses(), "Id", "BusNumber");
        ViewData["LoopId"] = new SelectList(_loopServiceInterface.GetLoops(), "Id", "Name");
        ViewData["IsActive"] = User.HasClaim("IsActive", "true");
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "ActiveOnly")]
    public async Task<IActionResult> StartDriving(int BusId, int LoopId)
    {
        Bus selectedBus = await _busServiceInterface.GetBus(BusId);
        Loop selectedLoop = await _loopServiceInterface.GetLoop(LoopId);
        _logger.LogInformation("Bus id {id} started driving on loop {loop} at {time}", BusId, selectedLoop.Name,
            DateTime.Now);
        return RedirectToAction("EntryCreator", new { BusId = selectedBus.Id, LoopId = selectedLoop.Id });
    }


    [Authorize(Policy = "ActiveOnly")]
    public async Task<IActionResult> EntryCreator(int BusId, int LoopId)
    {
        Bus selectedBus = await _busServiceInterface.GetBus(BusId);
        Loop selectedLoop = await _loopServiceInterface.GetLoop(LoopId);
        _routeServiceInterface.GetRoutes();

        EntryCreatorViewModel entryCreatorViewModel = new EntryCreatorViewModel
        {
            Bus = selectedBus,
            Loop = selectedLoop,
            Entry = new Entry(),
            BusId = BusId,
            LoopId = LoopId,
        };

        return View(entryCreatorViewModel);
    }

    [Authorize(Policy = "ManagerOnly")]
    public async Task<IActionResult> ApproveDriver(string userId)
    {
        _logger.LogInformation("Approve Driver called for user {id} at {time}.", userId, DateTime.Now);
        // Check if user exists
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            // Handle the case where the user doesn't exist
            _logger.LogWarning("Approve Driver called on user that does not exist.", DateTime.Now);
            return NotFound();
        }

        // Add the approval claim to the user
        var result = await _userManager.AddClaimAsync(user, new Claim("IsActive", "true"));
        _logger.LogInformation("User with id {userId} approved at {time}.", userId, DateTime.Now);

        if (!result.Succeeded)
        {
            // Handle the case where the claim couldn't be added
            return BadRequest();
        }

        // If the claim was added successfully, update the user
        await _userManager.UpdateAsync(user);
        var identity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

        if (identity != null)
        {
            identity.AddClaim(new Claim("IsActive", "true"));
            // Redirect to the Driver Index action method
            return RedirectToAction("Dashboard", "Home");
        }

        // Sign out the user
        await _signInManager.SignOutAsync();
        // Redirect to the Login page
        return RedirectToAction("Login", "User");
    }
}