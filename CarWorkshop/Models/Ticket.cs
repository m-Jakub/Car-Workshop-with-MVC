using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [DisplayName("Assigned Employee")]
        public string ?EmployeeName { get; set; } = "Not assigned"; // Name of the employee assigned to repair the vehicle

        // List of CalendarEvent IDs for scheduled times
         public List<int> CalendarEventIds { get; set; } = new List<int>();

        // Track the state of the ticket, e.g., created, in progress, done, closed
        [Required]
        public string State { get; set; } = "Created";

        // Optional properties
        [DisplayName("Estimate Description")]
        public string? EstimateDescription { get; set; }

        [DisplayName("Expected Cost")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ExpectedCost { get; set; }

        [DisplayName("Estimate Accepted")]
        public bool? EstimateAccepted { get; set; } = false;

        // List of parts bought for the repair (optional)
        public List<Part>? PartsBought { get; set; }

        [DisplayName("Price Paid")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePaid { get; set; } // Price paid by the client
    }

    public class Part
    {
        public int PartId { get; set; } // Primary key
        public string Name { get; set; }
        public double Amount { get; set; }
        
        [DisplayName("Unit Price")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }

        [DisplayName("Total Price")]
        public decimal TotalPrice => (decimal)Amount * UnitPrice; // Automatically calculate total price

        [Required]
        public int TicketId { get; set; } // Foreign key
    }
}
