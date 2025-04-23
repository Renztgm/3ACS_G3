using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Payroll_Test_2.Pages.Payroll
{
    public class Step1_SelectCutoffModel : PageModel
    {
        [BindProperty]
        public DateTime StartDate { get; set; }

        [BindProperty]
        public DateTime EndDate { get; set; }

        [BindProperty]
        public string CutoffType { get; set; } // Weekly, Bi-Weekly, Monthly

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (EndDate <= StartDate)
            {
                ModelState.AddModelError("", "End date must be after start date.");
                return Page();
            }

            return RedirectToPage("Step2_CalculateWorkedHours", new { startDate = StartDate, endDate = EndDate, cutoffType = CutoffType });
        }
    }
}

