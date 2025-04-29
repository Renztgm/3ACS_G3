    using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
            public Employee Employee { get; set; } // Relationship to Employee table

            [Column("PayrollCycle")]
            public string PayrollCycle { get; set; } // e.g., "Monthly", "Bi-Weekly"

            [Column("PayrollStartDate")]
            public DateTime PayrollStartDate { get; set; } // Start date of the payroll period
            [Column("PayrollEndDate")]
            public DateTime PayrollEndDate { get; set; } // End date of the payroll period

            [Column("TotalWorkedHours")]
            public decimal? TotalWorkedHours { get; set; } // Total hours worked in the period
            [Column("OvertimeHours")]
            public decimal? OvertimeHours { get; set; } // Overtime hours worked

            [Column("Incentive")]
            public decimal? Incentive { get; set; } // Any additional incentives

            [Column("Bonus")]
            public decimal? Bonus { get; set; } // Any bonuses received

            [Column("CreatedAt")]
            public DateTime CreatedAt { get; set; }

            [Column("UpdatedAt")]
            public DateTime UpdatedAt { get; set; }

            [Column("LeaveID")]
            public int? LeaveID { get; set; }

            [Column("Leave")]
            public decimal Leave { get; set; }

            [Column("NetSalary")]
            public decimal? NetSalary { get; set; }

            [Column("GrossSalary")]
            public decimal? GrossSalary { get; set; }

            [Column("TotalDeduction")]
            public decimal? TotalDeductions { get; set; }

            [Column("SSS")]
            public decimal? SSS { get; set; }

            [Column("Pagibig")]
            public decimal? Pagibig { get; set; }

            [Column("Philhealth")]
            public decimal? Philhealth { get; set; }

            [Column("TIN")]
            public decimal? TIN { get; set; }
        
            [Column("HMO")]
            public decimal? HMO { get; set; }

            [Column("LoanDeduction")]
            public decimal? LoanDeduction { get; set; }

            [Column("LoanID")]
            public int? LoanID { get; set; }

            [Column("AttendanceID")]
            public int AttendanceID { get; set; }
            [Column("DeductionID")]
            public int? DeductionID { get; set; }
        }
    }
