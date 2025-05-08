using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;
using Payroll_Test_2.Pages.Models;
using System.Text;


namespace Payroll_Test_2.Pages
{
    [Authorize]
    public class PayslipModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PayslipModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Payroll_Test_2.Pages.Models.Payroll payroll { get; set; }

        public string PayslipCsvContent { get; set; }

        public IActionResult OnGet(int PayrollID, int EmployeeID)
        {
            payroll = _context.Payroll
                .Include(p => p.Employee)
                .FirstOrDefault(p => p.PayrollID == PayrollID && p.EmployeeID == EmployeeID);

            if (payroll == null)
            {
                return RedirectToPage("/Error");
            }

            var csv = new StringBuilder();
            csv.AppendLine("                    The Company Name");
            csv.AppendLine("---------------------------------------------------------");
            csv.AppendLine($"Employee Name,{payroll.Employee.FirstName} {payroll.Employee.LastName}");
            csv.AppendLine($"Payroll Cycle,{payroll.PayrollCycle}");
            csv.AppendLine($"Start Date,{payroll.PayrollStartDate:yyyy-MM-dd}");
            csv.AppendLine($"End Date,{payroll.PayrollEndDate:yyyy-MM-dd}");
            csv.AppendLine("---------------------------------------------------------");
            csv.AppendLine("                       Gross Salary");
            csv.AppendLine("---------------------------------------------------------");
            csv.AppendLine($"Total Worked Hours,{payroll.TotalWorkedHours:0.00}");
            csv.AppendLine($"Overtime Hours,{payroll.OvertimeHours:0.00}");
            csv.AppendLine($"Night Differential Pay,{payroll.NightDifferentialPay:0.00}");
            csv.AppendLine($"Incentive,{payroll.Incentive:0.00}");
            csv.AppendLine($"Bonus,{payroll.Bonus:0.00}");
            csv.AppendLine($"Gross Salary,{payroll.GrossSalary:0.00}");
            csv.AppendLine("---------------------------------------------------------");
            csv.AppendLine("                       Deductions");
            csv.AppendLine("---------------------------------------------------------");
            csv.AppendLine($"SSS,{payroll.SSS:0.00}");
            csv.AppendLine($"Philhealth,{payroll.Philhealth:0.00}");
            csv.AppendLine($"Pagibig,{payroll.Pagibig:0.00}");
            csv.AppendLine($"HMO,{payroll.HMO:0.00}");
            csv.AppendLine($"Loan Deduction,{payroll.LoanDeduction:0.00}");
            csv.AppendLine($"Total Deductions,{payroll.TotalDeductions:0.00}");
            csv.AppendLine("---------------------------------------------------------");
            csv.AppendLine($"Net Salary,{payroll.NetSalary:0.00}");
            csv.AppendLine("---------------------------------------------------------");

            PayslipCsvContent = csv.ToString();
            return Page();
        }


        public IActionResult OnGetCsv(int PayrollID, int EmployeeID)
        {
            var payroll = _context.Payroll
                .Include(p => p.Employee)
                .FirstOrDefault(p => p.PayrollID == PayrollID && p.EmployeeID == EmployeeID);

            if (payroll == null)
            {
                return RedirectToPage("/Error");
            }

            var csv = new StringBuilder();

            csv.AppendLine("                    The Company Name");
            csv.AppendLine("---------------------------------------------------------");
            csv.AppendLine($"Employee Name,{payroll.Employee.FirstName} {payroll.Employee.LastName}");
            csv.AppendLine($"Payroll Cycle,{payroll.PayrollCycle}");
            csv.AppendLine($"Start Date,{payroll.PayrollStartDate:yyyy-MM-dd}");
            csv.AppendLine($"End Date,{payroll.PayrollEndDate:yyyy-MM-dd}");
            csv.AppendLine("---------------------------------------------------------");
            csv.AppendLine("                       Gross Salary");
            csv.AppendLine("---------------------------------------------------------");
            csv.AppendLine($"Total Worked Hours,{payroll.TotalWorkedHours:0.00}");
            csv.AppendLine($"Overtime Hours,{payroll.OvertimeHours:0.00}");
            csv.AppendLine($"Night Differential Pay,{payroll.NightDifferentialPay:0.00}");
            csv.AppendLine($"Incentive,{payroll.Incentive:0.00}");
            csv.AppendLine($"Bonus,{payroll.Bonus:0.00}");
            csv.AppendLine($"Gross Salary,{payroll.GrossSalary:0.00}");
            csv.AppendLine("---------------------------------------------------------");
            csv.AppendLine("                       Deductions");
            csv.AppendLine("---------------------------------------------------------");
            csv.AppendLine($"SSS,{payroll.SSS:0.00}");
            csv.AppendLine($"Philhealth,{payroll.Philhealth:0.00}");
            csv.AppendLine($"Pagibig,{payroll.Pagibig:0.00}");
            csv.AppendLine($"HMO,{payroll.HMO:0.00}");
            csv.AppendLine($"Loan Deduction,{payroll.LoanDeduction:0.00}");
            csv.AppendLine($"Total Deductions,{payroll.TotalDeductions:0.00}");
            csv.AppendLine("---------------------------------------------------------");
            csv.AppendLine($"Net Salary,{payroll.NetSalary:0.00}");
            csv.AppendLine("---------------------------------------------------------");

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"Payslip_{payroll.Employee.FirstName}_{payroll.Employee.LastName}_{payroll.PayrollID}.csv");
        }

    }
}
