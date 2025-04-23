using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Payroll_Test_2.Pages.Models
{
    [Table("Loans")] // Maps to the database table "positions"
    public class Loans
    {
        [Key]
        [Column("LoanID")]
        public int LoanID { get; set; }
    }
}
