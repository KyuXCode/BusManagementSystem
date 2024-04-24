using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusManagementSystem.Models;
using BusManagementSystem.Service;

namespace BusManagementSystem.Controllers;

[Authorize(Policy = "ManagerOnly")]
public class StopController : Controller
{
    private readonly IStopServiceInterface _stopServiceInterface;
    private readonly ILogger<StopController> _logger;

    public StopController(IStopServiceInterface stopServiceInterface, ILogger<StopController> logger)
    {
        _stopServiceInterface = stopServiceInterface;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var stops = _stopServiceInterface.GetStops();
        return View(stops);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Latitude,Longitude")] Stop stop)
    {
        await _stopServiceInterface.AddStop(stop);
        _logger.LogInformation("Added stop with id {id} at {time}", stop.Id, DateTime.Now);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var stop = await _stopServiceInterface.GetStop(id);
        if (stop == null)
        {
            return NotFound();
        }

        return View(stop);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Latitude,Longitude")] Stop stop)
    {
        if (id != stop.Id)
        {
            return NotFound();
        }


        try
        {
            await _stopServiceInterface.UpdateStop(stop);
            _logger.LogInformation("Edited stop with id: {id} at {time}", stop.Id, DateTime.Now);
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
    public async Task<IActionResult> Delete(int stopId)
    {
        try
        {
            var stop = await _stopServiceInterface.GetStop(stopId);
            if (stop == null)
            {
                _logger.LogWarning("Stop with id: {id} not found for deletion at {time}.", stopId, DateTime.Now);
                return NotFound();
            }

            await _stopServiceInterface.DeleteStop(stopId);
            _logger.LogInformation("Deleted stop with id: {id} at {time}", stopId, DateTime.Now);
        }
        catch (Exception e)
        {
            _logger.LogError("Delete Stop failed with exception {exception} at {time}.", e.Message, DateTime.Now);
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}