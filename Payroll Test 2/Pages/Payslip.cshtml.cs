using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Pages.Data;


namespace Payroll_Test_2.Pages
{
    public class PayslipModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PayslipModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Payroll_Test_2.Pages.Models.Payroll Payroll { get; set; }

        public void OnGet(int PayrollID, int EmployeeID)
        {
            Payroll = _context.Payroll
                .Include(p => p.Employee)
                //.Include(p => p.Deductions)
                .FirstOrDefault(p => p.PayrollID == PayrollID && p.EmployeeID == EmployeeID);

            if (Payroll == null)
            {
                // Handle error, could not find the payslip.
                RedirectToPage("/Error");
            }
        }
    }
}
