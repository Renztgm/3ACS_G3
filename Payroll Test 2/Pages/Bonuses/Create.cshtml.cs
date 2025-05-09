using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Payroll_Test_2.Data;
using Payroll_Test_2.Pages.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payroll_Test_2.Pages.Bonuses
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Constructor to initialize the DbContext
        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BonusCreate BonusCreate { get; set; }  // Using the BonusCreate model for binding

        [BindProperty]
        public List<SelectListItem> EmployeeOptions { get; set; }  // Employee dropdown options

        // OnGet method loads the employee options
        public IActionResult OnGet()
        {
            LoadEmployeeOptions();
            return Page();
        }

        // OnPostAsync method handles the form submission and saves the data
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                LoadEmployeeOptions();  // Reload employee options if validation fails
                return Page();
            }

            try
            {
                // Map BonusCreate to Bonus model before saving to the database
                var bonus = new Bonus
                {
                    EmployeeID = BonusCreate.EmployeeID,
                    Amount = BonusCreate.Amount,
                    Date = BonusCreate.Date,
                    Description = BonusCreate.Description
                };

                // Add the new bonus to the database
                _context.Bonuses.Add(bonus);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Bonus created successfully!";
                return RedirectToPage("/BonusIndexPage");
            }
            catch (System.Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the bonus.";
                LoadEmployeeOptions();
                return Page();
            }
        }

        // Load employee options for the dropdown
        private void LoadEmployeeOptions()
        {
            EmployeeOptions = _context.Employees
                .Select(e => new SelectListItem
                {
                    Value = e.EmployeeId.ToString(),
                    Text = $"{e.FirstName} {e.LastName}"
                })
                .ToList();
        }
    }
}
