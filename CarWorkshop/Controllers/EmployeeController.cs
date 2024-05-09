using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarWorkshop.Models;
using CarWorkshop.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CarWorkshop.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Ticket
        public async Task<IActionResult> AvailableTickets()
        {
            return View(await _context.Ticket.ToListAsync());
        }

        // POST: Employee/AcceptTicket/5
        [HttpPost]
        public async Task<IActionResult> AcceptTicket(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string? employeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ticket = await _context.Ticket.FindAsync(id);

            if (ticket != null && ticket.State == "Created")
            {
                return RedirectToAction("Calendar", "Employee", new { chooseSlotsOnly = true, ticketId = id });
            }
            if (ticket != null && ticket.State != "Created")
            {
                return RedirectToAction("AvailableTickets");
            }

            return NotFound();
        }

        // GET: Employee/Calendar
        public IActionResult Calendar(bool chooseSlotsOnly = false, int? ticketId = null)
        {
            // Retrieve logged-in employee ID
            string? employeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["EmployeeId"] = employeeId;

            if (ticketId.HasValue)
            {
                ViewData["TicketId"] = ticketId.Value;
            }

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
            if (!chooseSlotsOnly)
            {
                return View("~/Views/Calendar/Calendar.cshtml", eventsByDayAndHour);
            }
            else
            {
                return View("~/Views/Calendar/ChooseCalendarSlots.cshtml", eventsByDayAndHour);
            }
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

        [HttpPost]
        public async Task<IActionResult> SubmitSelectedEvents([FromBody] SubmitEventsRequest request)
        {
            // Validate the request
            if (request == null || request.EventIds == null || request.EventIds.Count == 0)
            {
                return BadRequest(new { success = false, error = "No events selected or invalid request" });
            }

            // Retrieve the employee ID from the authenticated user's claims
            string? employeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Update the events that match the specified EventIds and belong to the authenticated employee
            var eventsToUpdate = await _context.CalendarEvents
                .Where(e => request.EventIds.Contains(e.CalendarEventId) && e.EmployeeId == employeeId)
                .ToListAsync();

            // If no events were found, return an error
            if (eventsToUpdate.Count == 0)
            {
                return NotFound(new { success = false, error = "No matching events found" });
            }

            // Update each event's status and ticket ID
            foreach (var calendarEvent in eventsToUpdate)
            {
                calendarEvent.AvailabilityStatus = "Busy";
                calendarEvent.TicketId = request.TicketId;
            }

            // Update the ticket
            var ticket = await _context.Ticket.FindAsync(request.TicketId);
            if (ticket != null)
            {
                ticket.State = "In Progress";
            }

            ticket.EmployeeId = employeeId;
            ticket.EmployeeName = User.Identity.Name;
            ticket.CalendarEventIds = request.EventIds;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a JSON response indicating success
            return Ok(new { success = true });
        }


    }

    public class SubmitEventsRequest
    {
        public int TicketId { get; set; }
        public List<int> EventIds { get; set; } // List of event IDs to update
    }


}