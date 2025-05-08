using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Test_2.Pages.Models
{
    public class LoanHistory
    {
        [Key]
        [Column("LoanHistoryID")]
        public int LoanHistoryID { get; set; }
        [Column("LoanID")]
        public int LoanID { get; set; }
        [Column("LoanAmount")]
        public decimal LoanAmount { get; set; }
        [Column("DateIssued")]
        public DateTime DateIssued { get; set; }

        [BindProperty]
        [ValidateNever]
        public Loans? Loan { get; set; }  // This is the navigation property
    }
}
