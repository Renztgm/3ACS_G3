﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Test_2.Pages.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceID { get; set; } // Primary Key

        [Required]
        public int EmployeeID { get; set; } // Foreign Key to Employee

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } // The attendance date

        [DataType(DataType.DateTime)]
        public DateTime? CheckInTime { get; set; } // Nullable in case of absence

        [DataType(DataType.DateTime)]
        public DateTime? LunchStartTime { get; set; } // ✅ New: Lunch break start time

        [DataType(DataType.DateTime)]
        public DateTime? LunchEndTime { get; set; } // ✅ New: Lunch break end time

        [DataType(DataType.DateTime)]
        public DateTime? CheckOutTime { get; set; } // Nullable in case of absence

        public decimal? WorkHours { get; set; } // Nullable in case of absence (Manually Converted in Query)

        public decimal? LunchDuration =>
            (LunchStartTime.HasValue && LunchEndTime.HasValue)
                ? (decimal)(LunchEndTime.Value - LunchStartTime.Value).TotalHours
                : (decimal?)null; // ✅ Optional: Auto-calculate lunch duration

        [Required]
        public string Status { get; set; } // Present, Absent, Late, etc.

        public string? Remarks { get; set; } // Optional comments

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Auto-sets when created

        public DateTime? UpdatedAt { get; set; } // Nullable, updated only when modified

        // Foreign Key Relationship
        [ForeignKey("EmployeeID")]
        public Employee Employee { get; set; }
    }
}
