// Pages/TestPayroll.cshtml.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;
using Payroll_Test_2.Pages.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using static Payroll_Test_2.Pages.AttendanceMonthModel;
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
        .Where(a => a.Date >= startDate && a.Date <= endDate)
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

            // Calculate night differential hours (10pm - 6am)
            var nightDifferentialHours = g.Sum(a => {
                if (a.Status != "Present" || !a.CheckInTime.HasValue || !a.CheckOutTime.HasValue)
                    return 0m;



                // Define night shift hours (10 PM to 6 AM)
                var nightShiftStart = new TimeSpan(22, 0, 0); // 10 PM
                var nightShiftEnd = new TimeSpan(6, 0, 0);    // 6 AM

                var checkInTime = a.CheckInTime.Value;
                var checkOutTime = a.CheckOutTime.Value;

                // Get just the time part for check-in and check-out
                var checkInTimeOfDay = checkInTime.TimeOfDay;
                var checkOutTimeOfDay = checkOutTime.TimeOfDay;

                decimal nightHoursND = 0;

                // If check-in and check-out are on the same day
                if (checkInTime.Date == checkOutTime.Date)
                {
                    // Case 1: Both check-in and check-out are within the night shift
                    if ((checkInTimeOfDay >= nightShiftStart || checkInTimeOfDay <= nightShiftEnd) &&
                        (checkOutTimeOfDay >= nightShiftStart || checkOutTimeOfDay <= nightShiftEnd))
                    {
                        // Handle the case where check-in is before midnight and check-out is after midnight
                        if (checkInTimeOfDay >= nightShiftStart && checkOutTimeOfDay <= nightShiftEnd)
                        {
                            nightHoursND = (decimal)((checkOutTime - checkInTime).TotalHours);
                        }
                        // Handle the case where check-in is before 10 PM and check-out is before 6 AM
                        else if (checkInTimeOfDay < nightShiftStart && checkOutTimeOfDay <= nightShiftEnd)
                        {
                            var nightStart = checkInTime.Date.Add(nightShiftStart);
                            nightHoursND = (decimal)((checkOutTime - nightStart).TotalHours);
                        }
                        // Handle the case where check-in is after 10 PM and check-out is after 6 AM
                        else if (checkInTimeOfDay >= nightShiftStart && checkOutTimeOfDay > nightShiftEnd)
                        {
                            var nightEnd = checkOutTime.Date.Add(nightShiftEnd);
                            nightHoursND = (decimal)((nightEnd - checkInTime).TotalHours);
                        }
                    }
                    // Case 2: Check-in before night shift, check-out during/after night shift
                    else if (checkInTimeOfDay < nightShiftStart && (checkOutTimeOfDay >= nightShiftStart || checkOutTimeOfDay <= nightShiftEnd))
                    {
                        var nightStart = checkInTime.Date.Add(nightShiftStart);
                        if (checkOutTimeOfDay <= nightShiftEnd)
                        {
                            nightHoursND = (decimal)((checkOutTime - nightStart).TotalHours);
                        }
                        else
                        {
                            var nightEnd = checkOutTime.Date.Add(nightShiftEnd);
                            nightHoursND = (decimal)((nightEnd - nightStart).TotalHours);
                        }
                    }
                    // Case 3: Check-in during night shift, check-out after night shift
                    else if ((checkInTimeOfDay >= nightShiftStart || checkInTimeOfDay <= nightShiftEnd) && checkOutTimeOfDay > nightShiftEnd)
                    {
                        var nightEnd = checkInTime.Date.Add(nightShiftEnd);
                        if (checkInTimeOfDay >= nightShiftStart)
                        {
                            nightHoursND = (decimal)((nightEnd - checkInTime).TotalHours);
                        }
                        else
                        {
                            nightHoursND = (decimal)((nightEnd - checkInTime.Date).TotalHours);
                        }
                    }
                }
                // If check-in and check-out span multiple days
                else
                {
                    // Calculate night hours for each day spanned
                    var currentDay = checkInTime;
                    while (currentDay.Date <= checkOutTime.Date)
                    {
                        if (currentDay.Date == checkInTime.Date)
                        {
                            // First day: from check-in to midnight or 6 AM, whichever comes first
                            if (checkInTimeOfDay < nightShiftStart)
                            {
                                var nightStart = currentDay.Date.Add(nightShiftStart);
                                var dayEnd = currentDay.Date.AddDays(1);
                                var span = dayEnd - nightStart;
                                if (span.TotalHours > 0)
                                    nightHoursND += (decimal)span.TotalHours;
                            }
                            else if (checkInTimeOfDay >= nightShiftStart)
                            {
                                var dayEnd = currentDay.Date.AddDays(1);
                                nightHoursND += (decimal)((dayEnd - checkInTime).TotalHours);
                            }
                        }
                        else if (currentDay.Date == checkOutTime.Date)
                        {
                            // Last day: from midnight to check-out or 6 AM, whichever comes last
                            if (checkOutTimeOfDay <= nightShiftEnd)
                            {
                                nightHoursND += (decimal)(checkOutTimeOfDay.TotalHours);
                            }
                            else
                            {
                                nightHoursND += (decimal)(nightShiftEnd.TotalHours);
                            }
                        }
                        else
                        {
                            // Full night for days in between
                            nightHoursND += (decimal)((nightShiftEnd.TotalHours + (24 - nightShiftStart.TotalHours)));
                        }

                        currentDay = currentDay.AddDays(1);
                    }
                }

                // Account for lunch break during night hours if applicable
                if (a.LunchStartTime.HasValue && a.LunchEndTime.HasValue)
                {
                    var lunchStartTime = a.LunchStartTime.Value;
                    var lunchEndTime = a.LunchEndTime.Value;
                    var lunchStartTimeOfDay = lunchStartTime.TimeOfDay;
                    var lunchEndTimeOfDay = lunchEndTime.TimeOfDay;

                    decimal lunchNightHours = 0;

                    // Check if lunch break overlaps with night shift
                    if ((lunchStartTimeOfDay >= nightShiftStart || lunchStartTimeOfDay <= nightShiftEnd) &&
                        (lunchEndTimeOfDay >= nightShiftStart || lunchEndTimeOfDay <= nightShiftEnd))
                    {
                        lunchNightHours = (decimal)((lunchEndTime - lunchStartTime).TotalHours);
                        nightHoursND -= lunchNightHours;
                    }
                }

                return Math.Max(0, nightHoursND); // Ensure we don't have negative hours
            });

            // Night differential pay (10% of regular rate for night hours)
            var nightDifferentialRate = 0.10m; // 10%

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

            var loans = _context.Loans
                .Where(l => l.EmployeeID == g.Key.EmployeeID && l.LoanStatus == "Approved")
                .ToList();

            decimal loanDeductionwithoutdiveder = loans.Sum(l => {
                // Calculate total payment with 3% interest
                decimal totalInterest = l.LoanAmount * 0.03m; // 3% of the total loan amount
                decimal totalPayment = l.LoanAmount + totalInterest;

                // Calculate remaining principal
                decimal remainingPrincipal = l.LoanAmount - l.PaidLoan;

                // If loan is fully paid, no more deductions
                if (remainingPrincipal <= 0)
                    return 0m;

                // Calculate remaining payments
                decimal remainingPayments = l.LoanTerm - (l.PaidLoan / (totalPayment / l.LoanTerm));

                // Handle case where remaining payments might be zero or negative
                if (remainingPayments <= 0)
                    return remainingPrincipal;

                // Calculate monthly payment
                decimal monthlyPayment = totalPayment / l.LoanTerm;

                Debug.WriteLine($"Loan ID: {l.LoanID}, Total: {totalPayment}, Monthly: {monthlyPayment}, Remaining: {remainingPrincipal}");

                return monthlyPayment;
            });

            decimal loanDeduction = loanDeductionwithoutdiveder / divider;
            Debug.WriteLine($"Loan Amount {loanDeduction} =  {loanDeductionwithoutdiveder}/ {divider}");



            // Total Deductions
            var totalDeductions = sssDeduction + pagibigDeduction + philhealthDeduction + tinDeduction + hmoDeduction + loanDeduction;

            // Net Salary after deductions
            var adjustedGrossSalary = grossSalary * (totalWorkedHours / scheduledWorkHours > 1 ? 1 : totalWorkedHours / scheduledWorkHours);

            

            // Calculate night differential pay
            var hourlyRate = scheduledWorkHours > 0 ? grossSalary / scheduledWorkHours : 0;
            var nightDifferentialPay = nightDifferentialHours * hourlyRate * nightDifferentialRate;
            var nightDifferentialPayRound = Math.Round(nightDifferentialPay, 2);
            Debug.WriteLine($"Night Differential Pay: {nightDifferentialPay}/{nightDifferentialHours}hr");
            var totalBonus = _context.Bonuses
                           .Where(b => b.EmployeeID == g.Key.EmployeeID && b.Date >= startDate && b.Date <= endDate)
                           .Sum(b => (decimal?)b.Amount) ?? 0;

            var overtimePay = overtimeHours * (grossSalary / scheduledWorkHours); // Hourly rate * Overtime hours

            var netSalary = totalBonus > 0
                ? totalBonus + (adjustedGrossSalary - totalDeductions) + overtimePay + nightDifferentialPay
                : adjustedGrossSalary - totalDeductions + overtimePay + nightDifferentialPayRound;
            var adjSalary = adjustedGrossSalary + overtimePay + nightDifferentialPay + totalBonus;

            return new
            {
                EmployeeID = g.Key.EmployeeID,
                FullName = g.Key.FirstName + " " + g.Key.LastName,
                TotalWorkedHours = totalWorkedHours,
                ScheduledWorkHours = scheduledWorkHours,
                OvertimeHours = overtimePay,
                NightDifferentialHours = nightDifferentialHours,
                NightDifferentialPay = nightDifferentialPayRound,
                Leaves = _context.Attendance
                    .Count(a => a.EmployeeID == g.Key.EmployeeID && a.Date >= startDate && a.Date <= endDate && leaveStatuses.Contains(a.Status)),
                Bonus = totalBonus,
                GrossSalary = adjSalary,
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
            var loanPaymentsMade = new List<LoanHistory>();

            foreach (var request in payrollRequests)
            {
                // Only process loan payments if there's a loan deduction
                if (request.LoanDeduction.HasValue && request.LoanDeduction.Value > 0)
                {
                    var loan = await _context.Loans
                        .FirstOrDefaultAsync(l => l.EmployeeID == request.EmployeeID && l.LoanStatus == "Approved");

                    if (loan != null)
                    {
                        // Update the paid loan amount
                        loan.PaidLoan += request.LoanDeduction.Value;

                        // Check if loan is fully paid
                        if (loan.PaidLoan >= loan.LoanAmount)
                        {
                            loan.LoanStatus = "Paid";
                        }

                        // Create loan payment history record
                        var loanHistory = new LoanHistory
                        {
                            LoanID = loan.LoanID,
                            LoanAmount = request.LoanDeduction.Value,
                            DateIssued = now
                        };

                        loanPaymentsMade.Add(loanHistory);
                    }
                }
            }

            // Add the loan history records
            if (loanPaymentsMade.Any())
            {
                _context.LoanHistory.AddRange(loanPaymentsMade);
            }

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
                NightDifferentialHours = r.NightDifferentialHours,
                NightDifferentialPay = r.NightDifferentialPay,
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

            _context.Payroll.AddRange(payrollEntries);
            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "Payroll saved successfully!",
                loanPayments = loanPaymentsMade.Count > 0 ? loanPaymentsMade.Count : 0
            });
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
            public decimal? NightDifferentialHours { get; set; }
            public decimal? NightDifferentialPay { get; set; }
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

    public class NightDifferentialResult
    {
        public DateTime Date { get; set; }
        public double NightHours { get; set; }
    }

    public List<NightDifferentialResult> GetNightDifferentials(List<Attendance> records)
    {
        var results = new List<NightDifferentialResult>();

        foreach (var record in records)
        {
            if (record.CheckInTime == null || record.CheckOutTime == null)
                continue;

            var checkIn = record.CheckInTime.Value;
            var checkOut = record.CheckOutTime.Value;

            var totalNightHours = 0.0;

            // Define the night window range (10 PM to 6 AM)
            var start = checkIn;
            while (start < checkOut)
            {
                var currentDate = start.Date;

                var nightStart = currentDate.AddHours(22); // 10:00 PM
                var nightEnd = currentDate.AddDays(1).AddHours(6); // 6:00 AM next day

                var overlapStart = Max(checkIn, nightStart);
                var overlapEnd = Min(checkOut, nightEnd);

                if (overlapEnd > overlapStart)
                {
                    totalNightHours += (overlapEnd - overlapStart).TotalHours;
                    results.Add(new NightDifferentialResult
                    {
                        Date = currentDate,
                        NightHours = (overlapEnd - overlapStart).TotalHours
                    });
                }

                start = nightEnd;
            }
        }

        return results;
    }

    // Helpers
    private DateTime Max(DateTime a, DateTime b) => a > b ? a : b;
    private DateTime Min(DateTime a, DateTime b) => a < b ? a : b;

}