using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Pages;
namespace Payroll_Test_2.Pages.Payroll
{
    public class EditPayrollModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditPayrollModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Payroll_Test_2.Pages.Models.Payroll Payroll { get; set; }


        public List<SelectListItem> EmployeeList { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Payroll = await _context.Payroll.FindAsync(id);

            if (Payroll == null)
            {
                return NotFound();
            }

            EmployeeList = await _context.Employees
                .Select(e => new SelectListItem
                {
                    Value = e.EmployeeId.ToString(),
                    Text = e.FirstName + " " + e.LastName
                })
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingPayroll = await _context.Payroll.FindAsync(Payroll.PayrollID);
            if (existingPayroll == null)
            {
                ErrorMessage = "Payroll record not found.";
                return Page();
            }

            existingPayroll.EmployeeID = Payroll.EmployeeID;
            existingPayroll.GrossSalary = Payroll.GrossSalary;
            existingPayroll.TotalHoursWorked = Payroll.TotalHoursWorked;
            existingPayroll.OvertimeHours = Payroll.OvertimeHours;
            existingPayroll.OvertimePay = Payroll.OvertimePay;
            existingPayroll.Deductions = Payroll.Deductions;
            existingPayroll.Bonuses = Payroll.Bonuses;
            existingPayroll.PayrollEndDate = Payroll.PayrollEndDate;
            existingPayroll.NetSalary = Payroll.NetSalary;

            await _context.SaveChangesAsync();

            return RedirectToPage("/Payroll/PayrollList");
        }
    }

}
