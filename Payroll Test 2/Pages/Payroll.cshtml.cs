using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;   
using System.Threading.Tasks;
using Payroll_Test_2.Pages.Models;
using Payroll_Test_2.Pages.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;


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

        public List<Payroll_Test_2.Pages.Models.Payroll> PayrollList { get; set; } // Ensure correct reference

        public async Task OnGetAsync()
        {

            PayrollList = await _context.Payroll
                .Include(p => p.Employee)
                .ToListAsync();

            foreach (var payroll in PayrollList)
            {
                if (payroll.Employee == null)
                {
                    // Optional: log or mark it
                    // e.g., payroll.Employee = new Employee { FirstName = "Unknown" };
                    payroll.Employee = new Employee { FirstName = "Unknown" };
                    
                }
            
            }
        }


        public async Task<IActionResult> OnGetDownloadCsvAsync()
        {
            var payrolls = await _context.Payroll
                .Include(p => p.Employee) // if you want employee data
                .ToListAsync();

            var csv = new StringBuilder();

            // Add CSV header
            csv.AppendLine("PayrollID,EmployeeID,EmployeeName,PayrollCycle,StartDate,EndDate,TotalWorkedHours,OvertimeHours,Incentive,Bonus,GrossSalary,NetSalary,TotalDeductions,SSS,Pagibig,Philhealth,TIN,HMO,LoanDeduction");

            foreach (var p in payrolls)
            {
                string line = string.Join(",", new[]
                {
            p.PayrollID.ToString(),
            p.EmployeeID.ToString(),
            Escape(p.Employee?.FirstName +" "+p.Employee?.LastName ?? "N/A"), // assuming Employee has FullName
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

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "payroll_export.csv");
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
