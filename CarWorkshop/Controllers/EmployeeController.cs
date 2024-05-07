using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarWorkshop.Models;
using CarWorkshop.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;


[Authorize(Roles = "Employee")]
public class EmployeeController : Controller
{
    private readonly AppDbContext _context;

    public EmployeeController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Employee/Calendar
    public IActionResult Calendar()
    {
        // Retrieve events for the current week (for demonstration purposes)
        DateTime startDate = DateTime.Today.StartOfWeek(DayOfWeek.Monday);
        DateTime endDate = startDate.AddDays(6);

        var events = _context.CalendarEvents
            .Where(e => e.Date >= startDate && e.Date <= endDate)
            .ToList();

        // Group events by date
        var eventsByDate = events.GroupBy(e => e.Date.Date)
            .ToDictionary(g => g.Key, g => g.ToList());

        return View("~/Views/Calendar/Calendar.cshtml", eventsByDate);

    }

    // POST: Employee/UpdateCalendar
    [HttpPost]
    public IActionResult Calendar(List<CalendarEvent> updatedEvents)
    {
        if (ModelState.IsValid)
        {
            // Update events in the database
            foreach (var updatedEvent in updatedEvents)
            {
                var existingEvent = _context.CalendarEvents.Find(updatedEvent.CalendarEventId);
                if (existingEvent != null)
                {
                    existingEvent.AvailabilityStatus = updatedEvent.AvailabilityStatus;
                    _context.Update(existingEvent);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Calendar");
        }

        // If model state is not valid, return to the calendar view with errors
        return View("Calendar", updatedEvents);
    }
}

