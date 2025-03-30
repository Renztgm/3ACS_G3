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
        public decimal GrossSalary { get; set; }

        [Column("TotalHoursWorked")]
        public int TotalHoursWorked { get; set; }

        [Column("OvertimeHours")]
        public int OvertimeHours { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OvertimePay { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Bonuses { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal NetSalary { get; set; }

        public DateTime PayrollEndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("Deductions")]
        public int DeductionID { get; set; }
        public Deductions Deductions { get; set; } // Relationship to deductions table

        // Computed Field for Total Deduction (Not Stored in DB)
        [NotMapped] // Marked as not stored in DB
        public decimal TotalDeduction
        {
            get
            {
                return (Deductions?.SSS ?? 0) + (Deductions?.Pagibig ?? 0) +
                       (Deductions?.TIN ?? 0) + (Deductions?.Philhealth ?? 0);
            }
        }
    }
}
