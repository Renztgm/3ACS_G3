// Pages/TestPayroll.cshtml.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Pages.Data;
using Payroll_Test_2.Pages.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using static TestPayrollModel;

[Authorize]
[IgnoreAntiforgeryToken]
public class TestPayrollModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public TestPayrollModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public DateTime StartDate { get; set; }

    [BindProperty]
    public DateTime EndDate { get; set; }

    [BindProperty]
    public string Cycle { get; set; }

    public class PayrollRequest
    {
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string cycle { get; set; }
    }

    public async Task<JsonResult> OnPostGetWorkedHoursAsync([FromBody] PayrollRequest request)
    {

        var leaveStatuses = new[]
            {
                "Sick Leave", "Paid Time Off", "Voluntary Time Off", "Marriage Leave",
                "Maternity Time Off", "Paternity Time Off", "Bereavement Leave", "Compensatory Leave", "Leave", "Half Day"
            };
        Debug.WriteLine($"Received: startDate={request?.startDate}, endDate={request?.endDate}, cycle={request?.cycle}");

        var allDeductions = await _context.Deductions
            .ToListAsync();

        decimal totalSSS = 0;
        decimal totalPagibig = 0;
        decimal totalPhilhealth = 0;
        decimal totalHMO = 0;

        
        // Process the deductions
        foreach (var deduction in allDeductions)
        {
            totalSSS += deduction.SSS;
            totalPagibig += deduction.Pagibig;
            totalPhilhealth += deduction.Philhealth;
            totalHMO += deduction.HMO;

            // Do something with each deduction (e.g., processing, calculating, etc.)
        }
        var dateFormat = "yyyy-MM-dd";
        if (!DateTime.TryParseExact(request.startDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out var startDate) ||
            !DateTime.TryParseExact(request.endDate, dateFormat, null, System.Globalization.DateTimeStyles.None, out var endDate))
        {
            return new JsonResult(new { error = "Invalid date format" }) { StatusCode = 400 };
        }

        var records = _context.Attendance
    .Include(a => a.Employee)
    .Where(a => a.Date >= startDate && a.Date <= endDate )
    .AsEnumerable()
    .GroupBy(a => new { a.EmployeeID, a.Employee.FirstName, a.Employee.LastName, a.Employee.Salary, a.Employee.SalaryType }) // <-- ADD Salary, SalaryType
    .Select(g => {
        var totalWorkedHours = g.Sum(a =>
            a.Status == "Present" && a.CheckInTime.HasValue && a.CheckOutTime.HasValue && a.Status == "Present"
                ? (decimal)((a.CheckOutTime.Value - a.CheckInTime.Value).TotalHours
                    - ((a.LunchStartTime.HasValue && a.LunchEndTime.HasValue)
                        ? (a.LunchEndTime.Value - a.LunchStartTime.Value).TotalHours
                        : 0))
                : 0);

        foreach (var a in g)
        {
            Debug.WriteLine($"ScheduledIn: {a.ScheduledIn}, ScheduledOut: {a.ScheduledOut}");
        }

        var scheduledWorkHours = g
            .Where(a => a.ScheduledIn != null && a.ScheduledOut != null && a.ScheduledIn != DateTime.MinValue && a.ScheduledOut != DateTime.MinValue)
            .Sum(a =>
            {
                Debug.WriteLine($"SchedIn {a.ScheduledIn}");  // Log the ScheduledIn value
                return (decimal)((a.ScheduledOut.Value - a.ScheduledIn.Value).TotalHours - 1); // Subtract 1 hour
            });

        Debug.WriteLine($"ScheduledWorkHours: {scheduledWorkHours}");


        var overtimeHours = g.Sum(a =>
        {
            // Calculate worked hours for the day
            var workedHours = (a.Status == "Present" && a.CheckInTime.HasValue && a.CheckOutTime.HasValue)
                ? (decimal)((a.CheckOutTime.Value - a.CheckInTime.Value).TotalHours
                    - ((a.LunchStartTime.HasValue && a.LunchEndTime.HasValue)
                        ? (a.LunchEndTime.Value - a.LunchStartTime.Value).TotalHours
                        : 0))
                : 0;

            // Calculate scheduled work hours for the day
            var scheduledWorkHours = (a.ScheduledIn != null && a.ScheduledOut != null && a.ScheduledIn != DateTime.MinValue && a.ScheduledOut != DateTime.MinValue)
                ? (decimal)((a.ScheduledOut.Value - a.ScheduledIn.Value).TotalHours - 1) // Subtract 1 hour for breaks
                : 0;

            // Calculate overtime for the day if worked hours exceed scheduled hours
            var overtime = workedHours > scheduledWorkHours ? workedHours - scheduledWorkHours : 0;

            // Apply 25% premium to overtime hours (1.25 represents 100% + 25%)
            return overtime * 1.25m;
        });

        // --- Adjust Gross Salary based on selected Cycle (NOT based on SalaryType) ---
        var grossSalary = g.Key.Salary;

        if (request.cycle == "Weekly")
            grossSalary = grossSalary / 4m;
        else if (request.cycle == "Bi-Weekly")
            grossSalary = grossSalary / 2m;
        else if (request.cycle == "Monthly" || request.cycle == "Project-Based")
            grossSalary = grossSalary; // No change


        // Assume grossSalary is already adjusted based on Cycle above
        // But for deduction calculation, we need the original Monthly salary

        var monthlySalary = g.Key.Salary; // g.Key.Salary is stored Monthly

        decimal divider = 1m;
        if (request.cycle == "Weekly")
            divider = 4m;
        else if (request.cycle == "Bi-Weekly")
            divider = 2m;
        else if (request.cycle == "Monthly" || request.cycle == "Project-Based")
            divider = 1m;

        // Define your deduction percentages
        decimal hmoFixedAmount = totalHMO;  // Fixed amount, not percentage
        decimal sssRate = totalSSS;       // 5%
        decimal pagibigRate = totalPagibig;   // 2%
        decimal philhealthRate = totalPhilhealth; // 2.5%
        decimal tinRate = 0.10m;       // 10%

        Debug.WriteLine($"SSS {sssRate} Pagibig {pagibigRate} Philhealth {philhealthRate} HMO {hmoFixedAmount}");
        // Deductions
        var sssDeduction = (monthlySalary * sssRate) / divider;
        var pagibigDeduction = (monthlySalary * pagibigRate) / divider;
        var philhealthDeduction = (monthlySalary * philhealthRate) / divider;

        // Total Mandatory Contributions
        var totalMandatoryDeductions = sssDeduction + pagibigDeduction + philhealthDeduction;

        // Taxable Income
        var taxableIncome = monthlySalary - totalMandatoryDeductions;

        // HMO (optional fixed deduction)
        var hmoDeduction = hmoFixedAmount / divider; // if you have this
            
        // Compute Tin Deduction based on Taxable Income
        decimal ComputeTinDeduction(decimal taxableIncome)
        {
            tinRate = taxableIncome <= 20833.33m ? 0m :
                      taxableIncome <= 33333.33m ? (taxableIncome - 20833.33m) * 0.15m :
                      taxableIncome <= 66666.67m ? 1875m + (taxableIncome - 33333.33m) * 0.20m :
                      taxableIncome <= 166666.67m ? 8541.67m + (taxableIncome - 66666.67m) * 0.25m :
                      taxableIncome <= 666666.67m ? 33541.67m + (taxableIncome - 166666.67m) * 0.30m :
                      183541.67m + (taxableIncome - 666666.67m) * 0.35m;

            var tinDeduction = tinRate / divider;
            Debug.WriteLine($"tinRate: {tinRate}");
            Debug.WriteLine($"tinRate: {divider}");

            return tinDeduction;
        }

        // Now get the TIN Deduction using the Taxable Income
        var tinDeduction = ComputeTinDeduction(taxableIncome);

        var loan = _context.Loans
            .FirstOrDefault(l => l.EmployeeID == g.Key.EmployeeID && l.LoanStatus == "Approved");

        decimal loanDeduction = loan != null
            ? (loan.LoanAmount - loan.PaidLoan) / 12m  // Example: divide remaining loan by 12 months for deduction
            : 0;

        Debug.WriteLine($"Loan Amount {loanDeduction}");


        // Total Deductions
        var totalDeductions = sssDeduction + pagibigDeduction + philhealthDeduction + tinDeduction + hmoDeduction + loanDeduction;

        // Net Salary after deductions
        var adjustedGrossSalary = scheduledWorkHours > 0 ? grossSalary * (totalWorkedHours / scheduledWorkHours) :0;


        var totalBonus = _context.Bonuses
                       .Where(b => b.EmployeeID == g.Key.EmployeeID && b.Date >= startDate && b.Date <= endDate)
                       .Sum(b => (decimal?)b.Amount) ?? 0;

        var overtimePay = overtimeHours * (grossSalary / scheduledWorkHours); // Hourly rate * Overtime hours

        var netSalary = totalBonus > 0
            ? totalBonus + (adjustedGrossSalary - totalDeductions) + overtimePay
            : adjustedGrossSalary - totalDeductions + overtimePay;

            
        return new
        {
            EmployeeID = g.Key.EmployeeID,
            FullName = g.Key.FirstName + " " + g.Key.LastName,
            TotalWorkedHours = totalWorkedHours,
            ScheduledWorkHours = scheduledWorkHours,
            OvertimeHours = overtimePay,
            Leaves = _context.Attendance
                .Count(a => a.EmployeeID == g.Key.EmployeeID && a.Date >= startDate && a.Date <= endDate && leaveStatuses.Contains(a.Status)),
            Bonus = totalBonus,
            GrossSalary = grossSalary,
            SSS = sssDeduction,
            Pagibig = pagibigDeduction,
            Philhealth = philhealthDeduction,
            TIN = tinDeduction,
            HMO = hmoDeduction,
            LoanDeduction = loanDeduction,
            TotalDeductions = totalDeductions,
            NetSalary = netSalary
        };



    })
    .ToList();

        return new JsonResult(records); // ✅ don't forget this
    }
    public async Task<IActionResult> OnPostSavePayrollAsync([FromBody] List<SavePayrollRequest> payrollRequests)
    {
        try
        {
            if (payrollRequests == null || !payrollRequests.Any())
                return new JsonResult(new { success = false, message = "No payroll data received." });

            var now = DateTime.Now;

            var payrollEntries = payrollRequests.Select(r => new Payroll
            {
                EmployeeID = r.EmployeeID,
                AttendanceID = 6,
                LoanID = r.LoanID,
                DeductionID = r.DeductionID,
                PayrollCycle = r.PayrollCycle,
                PayrollStartDate = r.PayrollStartDate,
                PayrollEndDate = r.PayrollEndDate,
                TotalWorkedHours = r.TotalWorkedHours,
                OvertimeHours = r.OvertimeHours,
                Incentive = r.Incentive,
                Bonus = r.Bonus,
                CreatedAt = now,
                UpdatedAt = now,
                LeaveID = r.LeaveID,

                GrossSalary = r.GrossSalary,
                NetSalary = r.NetSalary,
                TotalDeductions = r.TotalDeductions,
                SSS = r.SSS,
                Pagibig = r.Pagibig,
                Philhealth = r.Philhealth,
                TIN = r.TIN,
                HMO = r.HMO,
                LoanDeduction = r.LoanDeduction
            }).ToList();

            _context.Payroll.AddRange(payrollEntries); // Make sure this is Payrolls
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Payroll saved successfully!" });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { success = false, message = "Error saving payroll: " + ex.Message });
        }
    }


    public class SavePayrollRequest
    {
        public int EmployeeID { get; set; }
        public int? AttendanceID { get; set; }
        public int? LoanID { get; set; }
        public int? DeductionID { get; set; }
        public string PayrollCycle { get; set; }
        public DateTime PayrollStartDate { get; set; }
        public DateTime PayrollEndDate { get; set; }
        public decimal? TotalWorkedHours { get; set; }
        public decimal? OvertimeHours { get; set; }
        public decimal? Incentive { get; set; }
        public decimal? Bonus { get; set; }
        public int? LeaveID { get; set; }
        public decimal? GrossSalary { get; set; }
        public decimal? NetSalary { get; set; }
        public decimal? TotalDeductions { get; set; }
        public decimal? SSS { get; set; }
        public decimal? Pagibig { get; set; }
        public decimal? Philhealth { get; set; }
        public decimal? TIN { get; set; }
        public decimal? HMO { get; set; }
        public decimal? LoanDeduction { get; set; }
    }



}
