using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarWorkshop.Models;
using CarWorkshop.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;


[Authorize(Roles = "Employee")]
public class EmployeeController : Controller
{
    private readonly AppDbContext _context;

    public EmployeeController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetEventsForDate(DayOfWeek dayOfWeek)
    {
        // Query the database for CalendarEvent instances occurring on the specified day of the week
        var eventsForDate = _context.CalendarEvents
            .Where(e => e.DayOfWeek == dayOfWeek) // Filter by day of the week
            .ToList(); // Execute the query and retrieve the list

        // Return the list as a JSON response
        return Json(eventsForDate);
    }



    // GET: Employee/Calendar
    public IActionResult Calendar()
    {
        // Calculate the start and end dates of the current week
        DateTime startDate = DateTime.Today.StartOfWeek(DayOfWeek.Monday);
        DateTime endDate = startDate.AddDays(6);

        // Query the database for CalendarEvent instances occurring within the specified date range
        var events = _context.CalendarEvents
            .Where(e => e.DayOfWeek >= startDate.DayOfWeek && e.DayOfWeek <= endDate.DayOfWeek)
            .ToList();

        // Group the events by day of the week and hour of the day
        // Group the events by day of the week and hour of the day
        var eventsByDayAndHour = events
            .GroupBy(e => new { e.DayOfWeek, e.Hour }) // Group by DayOfWeek and Hour
            .GroupBy(g => g.Key.DayOfWeek, g => g)
            .ToDictionary(
                outerGroup => outerGroup.Key, // Key for the outer dictionary (DayOfWeek)
                outerGroup => outerGroup.ToDictionary(
                    innerGroup => innerGroup.Key.Hour, // Key for the inner dictionary (Hour)
                    innerGroup => innerGroup.ToList() // Convert each subgroup to a list of events
                )
            );


        // Return the model to the view
        return View("~/Views/Calendar/Calendar.cshtml", eventsByDayAndHour);
    }





    // POST: Employee/Calendar
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

    [HttpPost]
    public IActionResult ChangeStatus(int eventId, string newStatus, DayOfWeek dayOfWeek, int hour)
    {
        // Retrieve the employee ID from the authenticated user's claims
        string? employeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Retrieve the event from the database based on day of the week and hour
        var calendarEvent = _context.CalendarEvents
            .FirstOrDefault(e => e.CalendarEventId == eventId && e.DayOfWeek == dayOfWeek && e.Hour == hour);

        if (calendarEvent != null)
        {
            // Update the event's status
            calendarEvent.AvailabilityStatus = newStatus;

            // Save the changes to the database
            _context.SaveChanges();

            // Return a JSON response indicating success
            return Json(new { success = true });
        }
        else
        {
            // If the event was not found, create a new event
            var newEvent = new CalendarEvent
            {
                DayOfWeek = dayOfWeek,
                Hour = hour,
                AvailabilityStatus = newStatus,
                EmployeeId = employeeId,
                // Set other required properties
            };

            // Add the new event to the database
            _context.CalendarEvents.Add(newEvent);

            // Save the changes to the database
            _context.SaveChanges();

            // Return a JSON response indicating success
            return Json(new { success = true });
        }
    }



}

