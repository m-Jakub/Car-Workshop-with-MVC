using System.ComponentModel.DataAnnotations;

namespace CarWorkshop.Models
{
	public class CalendarEvent
	{
		public int CalendarEventId { get; set; }
		
		[Required]
		public string? EmployeeId { get; set; }

		[Required]
		public DateTime Date { get; set; }

		[Required]
		public string? AvailabilityStatus { get; set; }

		[Required]
		public AppUser? Employee { get; set; }
	}
}
