using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusManagementSystem.Models;
using BusManagementSystem.Service;
using Microsoft.EntityFrameworkCore;

namespace BusManagementSystem.Controllers;

[Authorize(Policy = "ManagerOnly")]
public class BusController : Controller
{
    private readonly BusContext _context;

    public BusController(BusContext context)
    {
        _context = context;
    }

    // GET: /Bus
    public IActionResult Index()
    {
        var buses = _context.Buses.ToList();
        return View(buses);
    }

    //POST: 

    [HttpPost]
    public async Task<IActionResult> Create([Bind("Id,BusNumber")] Bus bus)
    {
        if (ModelState.IsValid)
        {
            _context.Buses.Add(bus);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return RedirectToAction("Index");
    }


    // //Update
    // [HttpPost, ActionName("Edit")]
    // public async Task<IActionResult> Edit(Bus bus)
    // {
    //     var foundBus = await _context.Buses.FindAsync(bus.BusId);
    //
    //     if (foundBus == null)
    //     {
    //         throw new Exception("Bus not found");
    //     }
    //
    //     foundBus.BusNumber = bus.BusNumber;
    //
    //     _context.Entry(foundBus).State = EntityState.Modified;
    //
    //     await _context.SaveChangesAsync();
    //
    //     return RedirectToAction("Index");
    // }

    public IActionResult Edit(int id)
    {
        var bus = _context.Buses.FirstOrDefault(b => b.BusId == id);
        if (bus == null)
        {
            return NotFound();
        }

        return View(bus);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int busId, [Bind("BusId, BusNumber")] Bus bus)
    {
        if (ModelState.IsValid)
        {
            var foundBus = _context.Buses.FirstOrDefault(b => b.BusId == busId);
            if (foundBus != null)
            {
                foundBus.BusNumber = bus.BusNumber;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        else
        {
            return View(bus);
        }
    }
    

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int busId)
    {
        var selectedBus = await _context.Buses.FindAsync(busId);
        if (selectedBus == null)
        {
            return NotFound();
        }

        _context.Buses.Remove(selectedBus);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }   
}