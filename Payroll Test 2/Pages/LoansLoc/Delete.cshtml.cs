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
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Loans Loan { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Loans == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans.FirstOrDefaultAsync(m => m.LoanID == id);

            if (loan == null)
            {
                return NotFound();
            }
            else
            {
                Loan = loan;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Loans == null)
            {
                return NotFound();
            }
            var loan = await _context.Loans.FindAsync(id);

            if (loan != null)
            {
                Loan = loan;
                _context.Loans.Remove(Loan);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}