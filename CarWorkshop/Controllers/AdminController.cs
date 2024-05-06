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
				HourlyRate = model.HourlyRate
			};

            var result = await _userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Employee");
                return RedirectToAction("EmployeeManagement");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }

    public async Task<IActionResult> EditEmployee(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Users.FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        var model = new EditEmployeeVM
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            HourlyRate = employee.HourlyRate
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditEmployee(string? id, EditEmployeeVM model)
    {
        if (id == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var employee = await _userManager.FindByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            employee.Name = model.Name;
            employee.Email = model.Email;
            employee.UserName = model.Email;
            employee.HourlyRate = model.HourlyRate;

            var updateResult = await _userManager.UpdateAsync(employee);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.Password) && !string.IsNullOrEmpty(model.ConfirmPassword))
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError(string.Empty, "New password and confirmation password do not match.");
                    return View(model);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(employee);

                var passwordResult = await _userManager.ResetPasswordAsync(employee, token, model.Password);

                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            return RedirectToAction("EmployeeManagement");
        }

        return View(model);
    }


    public async Task<IActionResult> RemoveEmployee(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Users.FindAsync(id);

        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    [HttpPost, ActionName("RemoveEmployee")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveEmployeeConfirmed(string id)
    {
        if (id == null)
        {
            return BadRequest("Invalid employee ID.");
        }

        var employee = await _userManager.FindByIdAsync(id);

        if (employee == null)
        {
            return NotFound("Employee not found.");
        }

        var deleteResult = await _userManager.DeleteAsync(employee);

        if (!deleteResult.Succeeded)
        {
            foreach (var error in deleteResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return RedirectToAction("EmployeeManagement");
        }

        return RedirectToAction("EmployeeManagement");
    }

}
