using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Payroll_Test_2.Pages.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System;
using System.Linq;
using Payroll_Test_2.Data;

namespace Payroll_Test_2.Pages
{
    [Authorize]
    public class PayrollModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PayrollModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string Month { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Cycle { get; set; }

        public List<Payroll_Test_2.Pages.Models.Payroll> PayrollList { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Payroll.Include(p => p.Employee).AsQueryable();

            if (!string.IsNullOrEmpty(Month) && DateTime.TryParse(Month + "-01", out DateTime selectedMonth))
            {
                query = query.Where(p => p.PayrollStartDate.Month == selectedMonth.Month &&
                                         p.PayrollStartDate.Year == selectedMonth.Year);
            }

            if (!string.IsNullOrEmpty(Cycle))
            {
                query = query.Where(p => p.PayrollCycle == Cycle);
            }

            PayrollList = await query.ToListAsync();

            foreach (var payroll in PayrollList)
            {
                if (payroll.Employee == null)
                {
                    payroll.Employee = new Employee { FirstName = "Unknown", LastName = "" };
                }
            }
        }

        public async Task<IActionResult> OnGetDownloadCsvAsync()
        {
            var query = _context.Payroll.Include(p => p.Employee).AsQueryable();

            if (!string.IsNullOrEmpty(Month) && DateTime.TryParse(Month + "-01", out DateTime selectedMonth))
            {
                query = query.Where(p => p.PayrollStartDate.Month == selectedMonth.Month &&
                                         p.PayrollStartDate.Year == selectedMonth.Year);
            }

            if (!string.IsNullOrEmpty(Cycle))
            {
                query = query.Where(p => p.PayrollCycle == Cycle);
            }

            var payrolls = await query.ToListAsync();

            foreach (var payroll in payrolls)
            {
                if (payroll.Employee == null)
                {
                    payroll.Employee = new Employee { FirstName = "Unknown", LastName = "" };
                }
            }

            var csv = new StringBuilder();
            csv.AppendLine("PayrollID,EmployeeID,EmployeeName,PayrollCycle,StartDate,EndDate,TotalWorkedHours,OvertimeHours,Incentive,Bonus,GrossSalary,NetSalary,TotalDeductions,SSS,Pagibig,Philhealth,TIN,HMO,LoanDeduction");

            foreach (var p in payrolls)
            {
                string line = string.Join(",", new[]
                {
                    p.PayrollID.ToString(),
                    p.EmployeeID.ToString(),
                    Escape($"{p.Employee?.FirstName} {p.Employee?.LastName}" ?? "N/A"),
                    Escape(p.PayrollCycle),
                    p.PayrollStartDate.ToString("yyyy-MM-dd"),
                    p.PayrollEndDate.ToString("yyyy-MM-dd"),
                    p.TotalWorkedHours?.ToString() ?? "",
                    p.OvertimeHours?.ToString() ?? "",
                    p.Incentive?.ToString() ?? "",
                    p.Bonus?.ToString() ?? "",
                    p.GrossSalary?.ToString() ?? "",
                    p.NetSalary?.ToString() ?? "",
                    p.TotalDeductions?.ToString() ?? "",
                    p.SSS?.ToString() ?? "",
                    p.Pagibig?.ToString() ?? "",
                    p.Philhealth?.ToString() ?? "",
                    p.TIN?.ToString() ?? "",
                    p.HMO?.ToString() ?? "",
                    p.LoanDeduction?.ToString() ?? ""
                });

                csv.AppendLine(line);
            }

            string fileName = "payroll_export.csv";
            var first = payrolls.FirstOrDefault();
            if (first != null)
            {
                var formattedStartDate = first.PayrollStartDate.ToString("yyyyMMdd");
                var formattedEndDate = first.PayrollEndDate.ToString("yyyyMMdd");
                fileName = $"payroll_{formattedStartDate}_to_{formattedEndDate}_{first.PayrollCycle}.csv";
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", fileName);
        }

        // Add this method to handle delete functionality
        public async Task<IActionResult> OnPostDeleteAsync(int payrollId)
        {
            var payroll = await _context.Payroll.FindAsync(payrollId);

            if (payroll != null)
            {
                // Check if there are any related records that need to be deleted first
                // For example, if you have PayrollDetails table with foreign key:
                /*
                var payrollDetails = await _context.PayrollDetails
                    .Where(pd => pd.PayrollID == payrollId)
                    .ToListAsync();
                    
                if (payrollDetails.Any())
                {
                    _context.PayrollDetails.RemoveRange(payrollDetails);
                }
                */

                _context.Payroll.Remove(payroll);
                await _context.SaveChangesAsync();

                // Add success message to TempData if you want to show it
                TempData["SuccessMessage"] = "Payroll record successfully deleted.";
            }
            else
            {
                // Add error message to TempData if you want to show it
                TempData["ErrorMessage"] = "Payroll record not found.";
            }

            // Redirect back to the same page with the same filters
            return RedirectToPage("./Payroll", new { Month, Cycle });
        }


        private string Escape(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }
            return value;
        }
    }
}
