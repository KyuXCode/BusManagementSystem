using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusManagementSystem.Models;

namespace BusManagementSystem.Controllers
{
    [Authorize(Policy = "ManagerOnly")]
    public class StopController : Controller
    {
        private readonly BusContext _context;

        public StopController(BusContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stops = await _context.Stops.ToListAsync();
            return View(stops);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StopId,Name,Latitude,Longitude")] Stop stop)
        {
            if (ModelState.IsValid)
            {
                _context.Stops.Add(stop);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var stop = await _context.Stops.FindAsync(id);
            if (stop == null)
            {
                return NotFound();
            }

            return View(stop);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int stopId, [Bind("StopId,Name,Latitude,Longitude")] Stop stop)
        {
            if (stopId != stop.StopId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var foundStop = _context.Stops.FirstOrDefault(b => b.StopId == stopId);
                if (foundStop != null)
                {
                    foundStop.Name = stop.Name;
                    foundStop.Latitude = stop.Latitude;
                    foundStop.Longitude = stop.Longitude;
                    _context.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            else
            {
                return View(stop);
            }
        }

        // public async Task<IActionResult> Delete(int id)
        // {
        //     var stop = await _context.Stops.FindAsync(id);
        //     if (stop == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     return View(stop);
        // }
        //
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(int id)
        // {
        //     var stop = await _context.Stops.FindAsync(id);
        //     _context.Stops.Remove(stop);
        //     await _context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }
    }
}