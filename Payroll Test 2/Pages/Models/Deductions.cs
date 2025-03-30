using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Test_2.Pages.Models
{
    public class Deductions
    {
        [Key]
        public int DeductionID { get; set; } // Primary Key

        [ForeignKey("Employee")]
        public int EmployeeID { get; set; } // Employee Relation
        public Employee Employee { get; set; } // Employee Object

        [Column(TypeName = "decimal(18,2)")]
        public decimal SSS { get; set; } // Social Security System Contribution

        [Column(TypeName = "decimal(18,2)")]
        public decimal Pagibig { get; set; } // Pagibig Fund Contribution

        [Column(TypeName = "decimal(18,2)")]
        public decimal TIN { get; set; } // Tax Identification Number (Withholding Tax)

        [Column(TypeName = "decimal(18,2)")]
        public decimal Philhealth { get; set; } // PhilHealth Contribution

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal TotalDeduction { get; set; } // Ensure this exists
    }
}
