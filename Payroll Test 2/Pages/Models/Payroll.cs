using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Test_2.Pages.Models
{
    public class Payroll
    {
        [Key]
        public int PayrollID { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; } // Relationship

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasicSalary { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalHoursWorked { get; set; } // ✅ Added

        [Column(TypeName = "decimal(18,2)")]
        public decimal OvertimeHours { get; set; } // ✅ Added

        [Column(TypeName = "decimal(18,2)")]
        public decimal OvertimePay { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Deductions { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Bonuses { get; set; } // ✅ Added if needed

        [Column(TypeName = "decimal(18,2)")]
        public decimal NetSalary { get; set; }

        public DateTime PayrollDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
