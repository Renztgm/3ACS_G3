using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Payroll_Test_2.Data;
using Payroll_Test_2.Pages.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payroll_Test_2.Pages
{
    public class BonusPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public BonusPageModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Bonus Bonus { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public List<SelectListItem> EmployeeSelectList { get; set; }

        public IActionResult OnGet()
        {
            Bonus = new Bonus { Date = DateTime.Today };
            PopulateEmployeeDropdown();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Bonus.EmployeeID <= 0)
            {
                ModelState.AddModelError("Bonus.EmployeeID", "The Employee field is required.");
            }

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please correct the errors below before submitting.";
                PopulateEmployeeDropdown();
                return Page();
            }

            try
            {
                if (Bonus.Date == DateTime.MinValue)
                {
                    Bonus.Date = DateTime.Today;
                }

                _context.Bonuses.Add(Bonus);
                await _context.SaveChangesAsync();

                SuccessMessage = "Bonus was successfully added!";
                return RedirectToPage("./BonusIndexPage");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                PopulateEmployeeDropdown();
                return Page();
            }
        }

        private void PopulateEmployeeDropdown()
        {
            EmployeeSelectList = _context.Employees
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .Select(e => new SelectListItem
                {
                    Value = e.EmployeeId.ToString(),
                    Text = $"{e.FirstName} {e.LastName}"
                })
                .ToList();

            EmployeeSelectList.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "-- Select Employee --"
            });
        }
    }
}
