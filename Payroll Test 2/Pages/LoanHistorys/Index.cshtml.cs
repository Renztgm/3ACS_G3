using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;
using Payroll_Test_2.Pages.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Payroll_Test_2.Pages.LoanHistorys
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<LoanHistory> LoanHistory { get; set; }

        public IList<Loans> Loans { get; set; }

        public async Task OnGetAsync()
        {
            // Fetch LoanHistory along with related Loan data
            LoanHistory = await _context.LoanHistory
                .Include(lh => lh.Loan)
                .ToListAsync();
            // Debugging - log or inspect data
            foreach (var history in LoanHistory)
            {
                if (history.Loan == null)
                {
                    // Log or inspect to check why Loan is null
                    Debug.WriteLine($"Loan is null for LoanHistoryID {history.LoanHistoryID}");
                }
                else
                {
                    Debug.WriteLine($"LoanHistoryID {history.LoanHistoryID} {history.LoanID}");
                }
            }
        }

    }
}
