using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
        public Loans Loan { get; set; } = default!;

        // On GET request to retrieve the loan
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Loan = await _context.Loans.FirstOrDefaultAsync(m => m.LoanID == id);

            if (Loan == null)
            {
                return NotFound();
            }

            return Page();
        }

        // On POST request to delete the loan
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the loan to delete
            Loan = await _context.Loans.FindAsync(id);

            if (Loan == null)
            {
                return NotFound();
            }

            // Remove the loan from the context
            _context.Loans.Remove(Loan);
            await _context.SaveChangesAsync();

            // Redirect to the index page after deletion
            return RedirectToPage("./Index");
        }
    }
}
