using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarWorkshop.Models
{
    public class Ticket
    {
        public int TicketId { get; set; } // Primary key

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        [Display(Name = "Registration ID")]
        public string RegistrationId { get; set; }

        [Required]
        public string Description { get; set; }

        public string ?EmployeeId { get; set; } // Employee assigned to repair the vehicle

        // List of CalendarEvent IDs for scheduled times
         public List<int> CalendarEventIds { get; set; } = new List<int>();

        // Track the state of the ticket, e.g., created, in progress, done, closed
        [Required]
        public string State { get; set; } = "Created";

        // Optional properties
        public string? EstimateDescription { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ExpectedCost { get; set; }
        public bool? EstimateAccepted { get; set; }

        // List of parts bought for the repair (optional)
        public List<Part>? PartsBought { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePaid { get; set; } // Price paid by the client
    }

    public class Part
    {
        public int PartId { get; set; } // Primary key
        public string Name { get; set; }
        public double Amount { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => (decimal)Amount * UnitPrice; // Automatically calculate total price
    }
}
