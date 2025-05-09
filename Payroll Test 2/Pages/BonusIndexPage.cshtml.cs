using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;
using Payroll_Test_2.Pages.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payroll_Test_2.Pages
{
    public class BonusIndexPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public BonusIndexPageModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Bonus> Bonuses { get; set; }

        public async Task OnGetAsync()
        {
            // Fetch bonuses and include the related Employee data
            Bonuses = await _context.Bonuses
                .Include(b => b.Employee)
                .OrderByDescending(b => b.Date)
                .ToListAsync();
        }
    }
}