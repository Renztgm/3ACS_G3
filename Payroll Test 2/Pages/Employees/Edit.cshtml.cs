using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;
using Payroll_Test_2.Pages.Models;

namespace Payroll_Test_2.Pages.Employees
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Employee = await _context.Employees
                .Include(e => e.Department) // Include navigation properties if needed
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (Employee == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var employeeInDb = await _context.Employees.FindAsync(Employee.EmployeeId);
            if (employeeInDb == null)
            {
                return NotFound();
            }

            // Update fields (Only update what is allowed!)
            employeeInDb.FirstName = Employee.FirstName;
            employeeInDb.LastName = Employee.LastName;
            employeeInDb.Email = Employee.Email;
            employeeInDb.PhoneNumber = Employee.PhoneNumber;
            employeeInDb.Salary = Employee.Salary;
            employeeInDb.SalaryType = Employee.SalaryType;
            employeeInDb.DepartmentId = Employee.DepartmentId;
            employeeInDb.EmploymentStatus = Employee.EmploymentStatus;
            employeeInDb.HireDate = Employee.HireDate;

            await _context.SaveChangesAsync();

            return RedirectToPage("/Employees/Index"); // Redirect after update
        }
    }

}
