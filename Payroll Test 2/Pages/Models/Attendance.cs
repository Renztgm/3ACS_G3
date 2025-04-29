using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Test_2.Pages.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceID { get; set; } // Primary Key
        public int EmployeeID { get; set; } // Foreign Key to Employee

        
        [DataType(DataType.DateTime)]
        [Column("AttendanceDate")]
        public DateTime? Date { get; set; } // The attendance date

        [DataType(DataType.DateTime)]
        [Column("TimeIn")]
        public DateTime? CheckInTime { get; set; } // Nullable in case of absence

        [DataType(DataType.DateTime)]
        public DateTime? LunchStartTime { get; set; } // ✅ New: Lunch break start time

        [DataType(DataType.DateTime)]
        public DateTime? LunchEndTime { get; set; } // ✅ New: Lunch break end time

        [DataType(DataType.DateTime)]
        [Column("TimeOut")]
        public DateTime? CheckOutTime { get; set; } // Nullable in case of absence

        //public decimal? WorkHours { get; set; } // Nullable in case of absence (Manually Converted in Query)

        public decimal? LunchDuration =>
            (LunchStartTime.HasValue && LunchEndTime.HasValue)
                ? (decimal)(LunchEndTime.Value - LunchStartTime.Value).TotalHours
                : (decimal?)null;

        
        public string Status { get; set; } // Present, Absent, Late, etc.

        //public string? Remarks { get; set; } // Optional comments

        
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Auto-sets when created

        public DateTime? UpdatedAt { get; set; } // Nullable, updated only when modified
        [Column("SchedTimeIn")]
        public DateTime? ScheduledIn { get; set; } // Scheduled check-in time
        [Column("SchedTimeOut")]
        public DateTime? ScheduledOut { get; set; } // Scheduled check-out time

        // Foreign Key Relationship
        [ForeignKey("EmployeeID")]
        public Employee Employee { get; set; }

        [NotMapped]
        public double DailyOvertimeHours
        {
            get
            {
                if (!CheckInTime.HasValue || !CheckOutTime.HasValue || !ScheduledIn.HasValue || !ScheduledOut.HasValue)
                    return 0;

                var actualWorked = (CheckOutTime.Value - CheckInTime.Value).TotalHours;
                var scheduledWorked = (ScheduledOut.Value - ScheduledIn.Value).TotalHours;

                var lunch = (LunchStartTime.HasValue && LunchEndTime.HasValue)
                    ? (LunchEndTime.Value - LunchStartTime.Value).TotalHours
                    : 0;

                var netWorked = actualWorked - lunch;
                var overtime = netWorked - scheduledWorked;

                return overtime > 0 ? overtime : 0;
            }
        }

    }
}
