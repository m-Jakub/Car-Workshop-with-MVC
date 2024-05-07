using Microsoft.AspNetCore.Mvc;
using System;

namespace CarWorkshop.Controllers
{
    public class CalendarController : Controller
    {
        [HttpPost]
        public IActionResult AddAvailabilitySlot(DateTime date, int hour)
        {
            // Add logic to handle adding availability slot
            // Example: Save the date and hour to the database

            // Return a success response
            return Ok("Availability slot added successfully");
        }
    }
}
