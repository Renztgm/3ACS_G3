using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2;
using Payroll_Test_2.Data;
using Payroll_Test_2.Pages.Models;

namespace Payroll_Test_2.Pages.LoansLoc
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Loans> Loans { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Loans != null)
            {
                Loans = await _context.Loans.ToListAsync();
            }
        }
    }
}