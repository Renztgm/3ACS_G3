using System.ComponentModel.DataAnnotations;

namespace Payroll_Test_2.Pages.Models
{
    public class BonusCreate
    {
        [Required(ErrorMessage = "Please select an employee.")]
        public int EmployeeID { get; set; }  // Only the EmployeeID (no navigation property)

        [Required(ErrorMessage = "Amount is required.")]
        //[Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }
    }
}
