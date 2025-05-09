using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payroll_Test_2.Pages.Models
{
    public class Bonus
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please select an employee.")]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }

        
    }

}
