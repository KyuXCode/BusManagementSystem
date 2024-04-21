using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BusManagementSystem.Models;
using BusManagementSystem.Service;
using BusManagementSystem.ViewModels;

namespace BusManagementSystem.Controllers
{
    [Authorize]
    public class EntryController : Controller
    {
        private readonly IEntryServiceInterface _entryServiceInterface;
        private readonly ILoopServiceInterface _loopServiceInterface;
        private readonly IBusServiceInterface _busServiceInterface;
        private readonly IStopServiceInterface _stopServiceInterface;
        private readonly UserManager<Driver> _userManager;
        private readonly ILogger<EntryController> _logger;

        public EntryController(IEntryServiceInterface entryServiceInterface, ILoopServiceInterface loopServiceInterface,
            IStopServiceInterface stopServiceInterface, IBusServiceInterface busServiceInterface,
            UserManager<Driver> userManager, ILogger<EntryController> logger)
        {
            _entryServiceInterface = entryServiceInterface;
            _loopServiceInterface = loopServiceInterface;
            _busServiceInterface = busServiceInterface;
            _stopServiceInterface = stopServiceInterface;
            _userManager = userManager;
            _logger = logger;
        }

        [Route("Entry")]
        public async Task<IActionResult> Index()
        {
            var entries = await _entryServiceInterface.GetEntries();
            return View(entries);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EntryCreatorViewModel entryCreatorViewModel)
        {
            entryCreatorViewModel.Bus = await _busServiceInterface.GetBus(entryCreatorViewModel.BusId);
            entryCreatorViewModel.Loop = await _loopServiceInterface.GetLoop(entryCreatorViewModel.LoopId);

            Entry entry = entryCreatorViewModel.Entry;
            entry.Stop = await _stopServiceInterface.GetStop(entryCreatorViewModel.SelectedStopId);
            
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            entry.Driver = await _userManager.FindByIdAsync(userId);

            entry.DriverId = userId;

            entry.Bus = entryCreatorViewModel.Bus;
            entry.Loop = entryCreatorViewModel.Loop;
            entry.Timestamp = DateTime.Now;

            ModelState.Clear();
            TryValidateModel(entryCreatorViewModel.Entry);

            _logger.LogInformation("Entry {id} created by {driver} on bus {bus} for stop {stop} at {time}",
                entry.Id, entry.Driver.FirstName + " " + entry.Driver.LastName, entry.Bus.BusNumber, entry.Stop.Id,
                DateTime.Now);
            await _entryServiceInterface.AddEntry(entry);
            
            return RedirectToAction("EntryCreator", "Driver", new { BusId = entry.Bus.Id, LoopId = entry.Loop.Id });
        }


        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditConfirmed(int id,
            [Bind("Id,Timestamp,Boarded,LeftBehind,Driver,Bus,Loop,Stop")]
            Entry Entry)
        {
            if (id != Entry.Id)
            {
                _logger.LogWarning("Entry with id {id} not found at {time}.", id, DateTime.Now);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _entryServiceInterface.UpdateEntry(Entry);
                    _logger.LogInformation("Updated entry {id} at {time}.", id, DateTime.Now);
                }
                catch (Exception e)
                {
                    _logger.LogWarning("Failed to update entry with exception {exception} at {time}.", e.Message,
                        DateTime.Now);
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _entryServiceInterface.DeleteEntry(id);
                _logger.LogInformation("Deleted entries with id {id} at {time}", id, DateTime.Now);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Delete Bus Failed with exception {exception} at {time}.", e.Message, DateTime.Now);
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}