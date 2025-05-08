using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;
using Payroll_Test_2.Pages.Models;

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
        public Models.Payroll Payroll { get; set; }

        public string EmployeeFullName { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Payroll = await _context.Payroll
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(p => p.PayrollID == id);

            if (Payroll == null)
            {
                return NotFound();
            }

            // Get the employee's full name
            EmployeeFullName = Payroll.Employee.FirstName + " " + Payroll.Employee.LastName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Re-fetch employee name for display if validation fails
                var employee = await _context.Employees.FindAsync(Payroll.EmployeeID);
                if (employee != null)
                {
                    EmployeeFullName = employee.FirstName + " " + employee.LastName;
                }
                return Page();
            }

            // Update the UpdatedAt timestamp
            Payroll.UpdatedAt = DateTime.Now;

            // Only update the fields that were modified (excluding EmployeeID and other IDs)
            var existingPayroll = await _context.Payroll.FindAsync(Payroll.PayrollID);

            if (existingPayroll == null)
            {
                return NotFound();
            }

            // Update payroll cycle and dates
            existingPayroll.PayrollCycle = Payroll.PayrollCycle;
            existingPayroll.PayrollStartDate = Payroll.PayrollStartDate;
            existingPayroll.PayrollEndDate = Payroll.PayrollEndDate;

            // Update work information
            existingPayroll.TotalWorkedHours = Payroll.TotalWorkedHours;
            existingPayroll.OvertimeHours = Payroll.OvertimeHours;

            // Update compensation
            existingPayroll.GrossSalary = Payroll.GrossSalary;
            existingPayroll.NetSalary = Payroll.NetSalary;
            existingPayroll.Incentive = Payroll.Incentive;
            existingPayroll.Bonus = Payroll.Bonus;

            // Update deductions
            existingPayroll.TotalDeductions = Payroll.TotalDeductions;
            existingPayroll.SSS = Payroll.SSS;
            existingPayroll.Pagibig = Payroll.Pagibig;
            existingPayroll.Philhealth = Payroll.Philhealth;
            existingPayroll.TIN = Payroll.TIN;
            existingPayroll.HMO = Payroll.HMO;
            existingPayroll.LoanDeduction = Payroll.LoanDeduction;
            existingPayroll.Leave = Payroll.Leave;

            // Update timestamp
            existingPayroll.UpdatedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("/Payroll");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayrollExists(Payroll.PayrollID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool PayrollExists(int id)
        {
            return _context.Payroll.Any(e => e.PayrollID == id);
        }
    }
}