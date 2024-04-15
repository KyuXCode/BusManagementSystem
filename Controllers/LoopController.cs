using BusManagementSystem.Models;
using BusManagementSystem.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusManagementSystem.Controllers;

[Authorize(Policy = "ManagerOnly")]
public class LoopController : Controller
{
    private readonly ILoopServiceInterface _loopServiceInterface;
    private readonly ILogger<LoopController> _logger;

    public LoopController(ILoopServiceInterface loopServiceInterface, ILogger<LoopController> logger)
    {
        _loopServiceInterface = loopServiceInterface;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var loops = _loopServiceInterface.GetLoops();
        return View(loops);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id, Name")] Loop loop)
    {
        try
        {
            await _loopServiceInterface.AddLoop(loop);
            _logger.LogInformation("Added loop with id {id} at {time}", loop.Id, DateTime.Now);
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            _logger.LogError("Create failed with exception {exception} at {time}", e.Message, DateTime.Now);
            return NotFound();
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var loop = await _loopServiceInterface.GetLoop(id);
        if (loop == null)
        {
            return NotFound();
        }

        return View(loop);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id, Name")] Loop loop)
    {
        if (id != loop.Id)
        {
            _logger.LogWarning("Loop with id {loopId} not found at {time}", loop.Id, DateTime.Now);
            return RedirectToAction("Index");
        }


        try
        {
            await _loopServiceInterface.UpdateLoop(loop);
            _logger.LogInformation("Edited loop with id {id} at {time}", loop.Id, DateTime.Now);
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            _logger.LogError("Edit failed with exception {exception} at {time}", e.Message, DateTime.Now);
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int loopId)
    {
        try
        {
            await _loopServiceInterface.DeleteLoop(loopId);
            _logger.LogInformation("Deleted loop with ids {loopId} at {time}", loopId, DateTime.Now);
        }
        catch (Exception e)
        {
            _logger.LogError("Delete loop failed with exception {exception} at {time}.", e.Message, DateTime.Now);
            return NotFound();
        }

        return RedirectToAction("Index");
    }
}