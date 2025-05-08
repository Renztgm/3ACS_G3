using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;
using Payroll_Test_2.Pages.Models;  // Update this if needed to match your actual namespace
using System.Threading.Tasks;

namespace Payroll_Test_2.Pages
{
    public class TaxSettingsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TaxSettingsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Deductions DeductionSettings { get; set; }

        public async Task OnGetAsync()
        {
            // Fetch the deduction settings - assuming there's only one record
            // If there are multiple records, you might need to specify which one to get
            DeductionSettings = await _context.Deductions.FirstOrDefaultAsync();

            // If no record exists yet, create a new one with default values
            if (DeductionSettings == null)
            {
                DeductionSettings = new Deductions
                {
                    SSS = 0.05m,
                    Philhealth = 0.025m,
                    Pagibig = 0.02m,
                    HMO = 1100m
                };
                _context.Deductions.Add(DeductionSettings);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Find the existing record
            var existingDeductions = await _context.Deductions.FindAsync(DeductionSettings.DeductionID);

            if (existingDeductions != null)
            {
                // Update the properties
                existingDeductions.SSS = DeductionSettings.SSS;
                existingDeductions.Philhealth = DeductionSettings.Philhealth;
                existingDeductions.Pagibig = DeductionSettings.Pagibig;
                existingDeductions.HMO = DeductionSettings.HMO;

                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Deduction settings updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["ErrorMessage"] = "Error updating deduction settings. Please try again.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Deduction settings not found.";
            }

            return RedirectToPage();
        }
    }
}