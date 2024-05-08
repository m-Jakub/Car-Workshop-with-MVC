using System;
using System.ComponentModel.DataAnnotations;

namespace CarWorkshop.Models
{
    public class CalendarEvent
    {
        public int CalendarEventId { get; set; }

        [Required]
        public string? EmployeeId { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; } // Day of the week

        [Required]
        public int Hour { get; set; } // Hour of the day (0-23)

        [Required]
        public string? AvailabilityStatus { get; set; }

        [Required]
        public AppUser? Employee { get; set; }
    }
}
