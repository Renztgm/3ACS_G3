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
            .Include(p => p.Employee)  // Include related Employee details
            .ToListAsync();
        }
    }
}
