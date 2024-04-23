using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusManagementSystem.Models;
using BusManagementSystem.Service;

namespace BusManagementSystem.Controllers;

[Authorize(Policy = "ManagerOnly")]
public class BusController : Controller
{
    private readonly IBusServiceInterface _busServiceInterface;
    private readonly ILogger<BusController> _logger;

    public BusController(IBusServiceInterface busServiceInterface, ILogger<BusController> logger)
    {
        _busServiceInterface = busServiceInterface;
        _logger = logger;
    }

    // GET: /Bus
    public IActionResult Index()
    {
        var buses = _busServiceInterface.GetBuses();
        return View(buses);
    }

    //POST: 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,BusNumber")] Bus bus)
    {
        await _busServiceInterface.AddBus(bus);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var bus = await _busServiceInterface.GetBus(id);

        if (bus == null)
        {
            return NotFound();
        }

        return View(bus);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id, BusNumber")] Bus bus)
    {
        if (id != bus.Id)
        {
            _logger.LogWarning("Bus with id {busId} not found at {time}.", bus.Id, DateTime.Now);
            return NotFound();
        }


        try
        {
            await _busServiceInterface.UpdateBus(bus);
            _logger.LogInformation("Edited bus with id {id} at {time}", bus.Id, DateTime.Now);
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            _logger.LogError("Edit Failed with exception {exception} at {time}.", e.Message, DateTime.Now);
            return NotFound();
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int busId)
    {
        try
        {
            await _busServiceInterface.DeleteBus(busId);
            _logger.LogInformation("Deleted buss with ids {busId} at {time}", busId, DateTime.Now);
        }
        catch (Exception e)
        {
            _logger.LogError("Delete Bus failed with exception {exception} at {time}.", e.Message, DateTime.Now);
            return NotFound();
        }

        return RedirectToAction("Index");
    }
}