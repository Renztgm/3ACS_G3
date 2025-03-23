using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Pages.Models; // Ensure correct namespace
using Payroll_Test_2.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payroll_Test_2.Pages
{
    public class PayrollModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PayrollModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Payroll_Test_2.Pages.Models.Payroll> PayrollList { get; set; } = new List<Payroll_Test_2.Pages.Models.Payroll>();

        public async Task OnGetAsync()
        {
            PayrollList = await _context.Payroll
                .Include(p => p.Employee) // Assuming Payroll has an Employee relation
                .ToListAsync();
        }
    }
}
