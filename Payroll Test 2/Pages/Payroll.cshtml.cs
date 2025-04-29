using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;   
using System.Threading.Tasks;
using Payroll_Test_2.Pages.Models;
using Payroll_Test_2.Pages.Data;

namespace Payroll_Test_2.Pages
{
    public class PayrollModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PayrollModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Payroll_Test_2.Pages.Models.Payroll> PayrollList { get; set; } // Ensure correct reference

        public async Task OnGetAsync()
        {
            PayrollList = await _context.Payroll
                .Include(p => p.Employee)
                .ToListAsync();

            foreach (var payroll in PayrollList)
            {
                if (payroll.Employee == null)
                {
                    // Optional: log or mark it
                    // e.g., payroll.Employee = new Employee { FirstName = "Unknown" };
                    payroll.Employee = new Employee { FirstName = "Unknown" };
                    
                }
            }
        }

    }
}
