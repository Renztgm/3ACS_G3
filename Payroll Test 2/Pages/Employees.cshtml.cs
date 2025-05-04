using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Payroll_Test_2.Pages.Data;
using Payroll_Test_2.Pages.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payroll_Test_2.Pages
{
    public class EmployeesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EmployeesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Employee> Employees { get; set; } = new List<Employee>();
        public List<Department> Departments { get; set; } = new List<Department>();
        //public List<Position> Positions { get; set; } = new List<Position>();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedDepartment { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SelectedPosition { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public async Task OnGetAsync(int? pageNumber)
        {
            PageNumber = pageNumber ?? 1;

            // Load all departments and positions for filtering
            Departments = await _context.Departments.ToListAsync();
            //Positions = await _context.Positions.ToListAsync();

            var query = _context.Employees
                .Include(e => e.Department)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                string searchLower = SearchTerm.ToLower();

                query = query.Where(e =>
                    EF.Functions.Like(e.FirstName.ToLower(), $"%{searchLower}%") ||
                    EF.Functions.Like(e.LastName.ToLower(), $"%{searchLower}%") ||
                    (searchLower.Contains(' ') && (e.FirstName.ToLower() + " " + e.LastName.ToLower()).StartsWith(searchLower, StringComparison.OrdinalIgnoreCase)) ||
                    EF.Functions.Like((e.FirstName + " " + e.LastName).ToLower(), $"%{searchLower}%") ||
                    EF.Functions.Like(e.FirstName.ToLower() + " " + (e.LastName != null && e.LastName.Length > 0 ? e.LastName.ToLower().Substring(0, 1) : ""), $"%{searchLower}%")
                );
            }

            // Apply department filter
            if (SelectedDepartment.HasValue)
            {
                query = query.Where(e => e.Department.DepartmentId == SelectedDepartment);
            }

            //// Apply position filter
            //if (SelectedPosition.HasValue)
            //{
            //    query = query.Where(e => e.Position.PositionId == SelectedPosition);
            //}

            // Retrieve the filtered list from the database
            var employeesList = await query.ToListAsync();

            // Apply in-memory filtering for "FirstName LastInitial" format
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                string searchLower = SearchTerm.ToLower();
                employeesList = employeesList
                    .Where(e =>
                        (e.FirstName + " " + e.LastName[0]).ToLower().Contains(searchLower)
                    ).ToList();
            }

            // Apply pagination
            Employees = employeesList
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
