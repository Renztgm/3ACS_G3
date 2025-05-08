using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;
using Payroll_Test_2.Pages.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Payroll_Test_2.Pages.LoanHistorys
{
    public class MakePaymentModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MakePaymentModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoanHistory NewPayment { get; set; }

        public Loans Loan { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Loan = await _context.Loans.FirstOrDefaultAsync(l => l.LoanID == id);

            //if (Loan == null)
            //    return NotFound();

            // Initialize NewPayment with LoanID
            NewPayment = new LoanHistory
            {
                LoanID = id
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["StatusMessage"] = "Error: Invalid form submission.";
                return Page();
            }

            var now = DateTime.Now;

            // Retrieve the loan
            var loan = await _context.Loans.FirstOrDefaultAsync(l => l.LoanID == NewPayment.LoanID);

            if (loan == null)
            {
                TempData["StatusMessage"] = "Error: Loan not found.";
                return Page();
            }

            if (NewPayment.LoanAmount <= 0)
            {
                TempData["StatusMessage"] = "Error: Payment amount must be greater than zero.";
                return Page();
            }

            // Apply payment to loan
            loan.PaidLoan += NewPayment.LoanAmount;

            if (loan.PaidLoan >= loan.LoanAmount)
            {
                loan.LoanStatus = "Paid";
            }

            // Record payment history
            NewPayment.DateIssued = now;
            _context.LoanHistory.Add(NewPayment);

            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Payment submitted successfully.";
            return RedirectToPage("/LoanHistorys/Index", new { loanId = loan.LoanID });
        }
    }
}
