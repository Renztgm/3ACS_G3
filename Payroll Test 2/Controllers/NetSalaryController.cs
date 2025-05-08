using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Data;
using Payroll_Test_2.Pages.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Payroll_Test_2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[AllowAnonymous]
    public class NetSalaryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NetSalaryController> _logger;

        public NetSalaryController(ApplicationDbContext context, ILogger<NetSalaryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("forecast")]
        public async Task<IActionResult> GetForecast()
        {
            try
            {
                // Get the base net salary from database
                decimal baseNetSalary = await _context.Payroll.SumAsync(e => e.NetSalary ?? 0m);

                // Generate forecast data
                var forecastData = GenerateForecastData(baseNetSalary);

                return Ok(forecastData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating net salary forecast");
                return StatusCode(500, "An error occurred while generating the forecast data.");
            }
        }

        [HttpGet("contributions")]
        public async Task<IActionResult> GetContributions()
        {
            try
            {
                // Get various contribution totals from the database
                var contributions = new
                {
                    SSS = await _context.Payroll.SumAsync(p => p.SSS ?? 0m),
                    Philhealth = await _context.Payroll.SumAsync(p => p.Philhealth ?? 0m),
                    Pagibig = await _context.Payroll.SumAsync(p => p.Pagibig ?? 0m),
                    TIN = await _context.Payroll.SumAsync(p => p.TIN ?? 0m),
                    HMO = await _context.Payroll.SumAsync(p => p.HMO ?? 0m),
                    LoanDeduction = await _context.Payroll.SumAsync(p => p.LoanDeduction ?? 0m)
                };

                return Ok(contributions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching contribution data");
                return StatusCode(500, "An error occurred while fetching contribution data.");
            }
        }

        [HttpGet("payroll-summary")]
        public async Task<IActionResult> GetPayrollSummary()
        {
            try
            {
                // Get summary of payroll data
                var summary = new
                {
                    TotalGrossSalary = await _context.Payroll.SumAsync(p => p.GrossSalary ?? 0m),
                    TotalNetSalary = await _context.Payroll.SumAsync(p => p.NetSalary ?? 0m),
                    TotalDeductions = await _context.Payroll.SumAsync(p => p.TotalDeductions ?? 0m),
                    PayrollCount = await _context.Payroll.CountAsync(),
                    LatestPayrollDate = await _context.Payroll.MaxAsync(p => p.PayrollEndDate as DateTime?)
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payroll summary data");
                return StatusCode(500, "An error occurred while fetching payroll summary data.");
            }
        }

        private List<DailySalaryForecast> GenerateForecastData(decimal baseNetSalary)
        {
            var forecast = new List<DailySalaryForecast>();
            var currentDate = DateTime.Now;
            var lastDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));

            // Start from current date
            for (var date = currentDate; date <= lastDayOfMonth; date = date.AddDays(1))
            {
                // Simulate processing trend - more processing happens mid-month
                var dayOfMonth = date.Day;
                decimal processedAmount;

                // Create a bell curve-like distribution with higher values in the middle of the month
                if (dayOfMonth < 15)
                {
                    processedAmount = baseNetSalary * (1 + dayOfMonth / 20m);
                }
                else
                {
                    processedAmount = baseNetSalary * (2 - dayOfMonth / 20m);
                }

                // Weekend days have reduced processing
                var dayOfWeek = (int)date.DayOfWeek;
                if (dayOfWeek == 0 || dayOfWeek == 6) // Sunday or Saturday
                {
                    processedAmount *= 0.4m;
                }

                forecast.Add(new DailySalaryForecast
                {
                    Date = date,
                    NetSalary = Math.Round(processedAmount, 2)
                });
            }

            return forecast;
        }
    }

    public class DailySalaryForecast
    {
        public DateTime Date { get; set; }
        public decimal NetSalary { get; set; }
    }
}