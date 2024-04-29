using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarWorkshop.Models;
using CarWorkshop.ViewModels.Admin;
using CarWorkshop.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public AdminController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        // Display the admin dashboard
        return View();
    }

    public async Task<IActionResult> EmployeeManagement(int page = 1)
    {
        int pageSize = 10;

        var allUsers = await _context.Users.ToListAsync();

        List<AppUser> filteredEmployees = new List<AppUser>();

        foreach (var user in allUsers)
        {
            bool isEmployee = await _userManager.IsInRoleAsync(user, "Employee");
            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (isEmployee && !isAdmin)
            {
                filteredEmployees.Add(user);
            }
        }

        var employees = filteredEmployees.Skip((page - 1) * pageSize).Take(pageSize);

        var model = new EmployeeManagementVM
        {
            Employees = employees.ToList(),
            Page = page,
            PageSize = pageSize,
            TotalEmployees = filteredEmployees.Count
        };

        return View(model);
    }

    public IActionResult AddEmployee()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddEmployee(AddEmployeeVM model)
    {
        if (ModelState.IsValid)
        {
            var user = new AppUser
            {
                Name = model.Name,
                UserName = model.Email,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Employee");
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }

}

