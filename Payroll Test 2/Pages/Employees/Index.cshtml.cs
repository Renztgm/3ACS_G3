using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;
using Payroll_Test_2.Pages.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payroll_Test_2.Pages.Employees
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Employee> Employees { get; set; }

        public async Task OnGetAsync()
        {
            Employees = await _context.Employees
                .Include(e => e.Department) // Include navigation properties if needed
                .ToListAsync();
        }
    }
}

