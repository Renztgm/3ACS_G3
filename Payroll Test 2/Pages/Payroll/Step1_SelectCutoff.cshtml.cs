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
        public string Cycle { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Cycle))
            {
                ModelState.AddModelError("Cycle", "Please select a cutoff cycle.");
                return Page();
            }
            // Ensure EndDate is after StartDate
            if (EndDate <= StartDate)
            {
                ModelState.AddModelError("", "End date must be after start date.");
                return Page(); // Return the same page with the error message
            }
            System.Diagnostics.Debug.WriteLine($"✅ Submitting {Cycle} ");
            // Pass the dates and cycle to the next page via query parameters
            return RedirectToPage("Step2_CalculateWorkedHours", new { StartDate = StartDate, EndDate = EndDate, Cycle = Cycle });
        }

    }
}

