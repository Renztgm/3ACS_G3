using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Payroll_Test_2;
using Payroll_Test_2.Data;
using Payroll_Test_2.Pages.Models;

namespace Payroll_Test_2.Pages.LoansLoc
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            // You can add code here to populate dropdowns for employees if needed
            // ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "FullName");
            return Page();
        }

        [BindProperty]
        public Models.Loans Loan { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || Loan == null)
            {
                return Page();
            }

            _context.Loans.Add(Loan);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}