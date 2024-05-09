using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarWorkshop.Data;
using CarWorkshop.Models;

namespace CarWorkshop.Controllers
{
    public class PartController : Controller
    {
        private readonly AppDbContext _context;

        public PartController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Part
        public async Task<IActionResult> Index(int? ticketId)
        {
            if (ticketId == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket.FindAsync(ticketId);
            if (ticket == null)
            {
                return NotFound();
            }

            var parts = await _context.Part.Where(p => p.TicketId == ticketId).ToListAsync();
            ViewData["TicketId"] = ticketId;

            return View(parts);
        }

        // GET: Part/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var part = await _context.Part
                .FirstOrDefaultAsync(m => m.PartId == id);
            if (part == null)
            {
                return NotFound();
            }

            return View(part);
        }

        // GET: Part/Create
        public IActionResult Create(int? ticketId)
        {
            if (ticketId == null)
            {
                // Handle the case when ticketId is null
                return NotFound();
            }

            // Pass the ticketId to the view using ViewData
            ViewData["TicketId"] = ticketId;

            return View();
        }

        // POST: Part/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Amount,UnitPrice,TicketId")] Part part)
        {
            if (ModelState.IsValid)
            {
                _context.Add(part);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { ticketId = part.TicketId });
            }
            return View(part);
        }


        // GET: Part/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var part = await _context.Part.FindAsync(id);
            if (part == null)
            {
                return NotFound();
            }

            return View(part);
        }

        // POST: Part/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PartId,Name,Amount,UnitPrice")] Part part)
        {
            if (id != part.PartId)
            {
                return NotFound();
            }
            
            var ticketId = _context.Part.Where(p => p.PartId == id).Select(p => p.TicketId).FirstOrDefault();
            part.TicketId = ticketId;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(part);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PartExists(part.PartId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
                return RedirectToAction(nameof(Index), new { ticketId = ticketId });
            }
            return View(part);
        }

        // GET: Part/Delete/5
        public async Task<IActionResult> Delete(int? ticketId, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var part = await _context.Part
                .FirstOrDefaultAsync(m => m.PartId == id);

            if (part == null)
            {
                return NotFound();
            }

            ViewData["TicketId"] = ticketId;

            return View(part);
        }

        // POST: Part/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var part = await _context.Part.FindAsync(id);
            int TicketId = part.TicketId;
            if (part != null)
            {
                _context.Part.Remove(part);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { ticketId = TicketId });
        }

        private bool PartExists(int id)
        {
            return _context.Part.Any(e => e.PartId == id);
        }
    }
}
