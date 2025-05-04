using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Test_2.Pages.Models
{
    public class Deductions
    {
        [Key]
        public int DeductionID { get; set; } // Primary Key
        [Column("SSS")]
        public decimal SSS { get; set; }
        [Column("PAGIBIG")]
        public decimal Pagibig { get; set; }
        [Column("PHILHEALTH")]
        public decimal Philhealth { get; set; }
        [Column("HMO")]
        public decimal HMO { get; set; }

        
    }
}
