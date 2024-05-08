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
        // Retrieve logged-in employee's ID
        string? employeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
        // Retrieve logged-in employee ID
        string? employeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewData["EmployeeId"] = employeeId;

        // Calculate start and end dates of the current week
        DateTime startDate = DateTime.Today.StartOfWeek(DayOfWeek.Monday);
        DateTime endDate = startDate.AddDays(6);

        // Query the database for events based on day of the week and employee ID
        var events = _context.CalendarEvents
            .Where(e => e.EmployeeId == employeeId)
            .ToList();

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

        return View("~/Views/Calendar/Calendar.cshtml", eventsByDayAndHour);
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
            CalendarEvent newEvent = new CalendarEvent
            {
                DayOfWeek = dayOfWeek,
                Hour = hour,
                AvailabilityStatus = newStatus,
                EmployeeId = employeeId,
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

