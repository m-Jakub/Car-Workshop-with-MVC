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
            // Leave Password and ConfirmPassword blank initially for security reasons
            Password = string.Empty,
            ConfirmPassword = string.Empty
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
            var employee = await _context.Users.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            employee.Name = model.Name;
            employee.Email = model.Email;

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

            _context.Update(employee);
            await _context.SaveChangesAsync();

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
        var employee = await _context.Users.FindAsync(id);

        if (employee != null)
        {
            _context.Users.Remove(employee);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("EmployeeManagement");
    }
}

// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using CarWorkshop.Models;
// using CarWorkshop.ViewModels.Admin;
// using CarWorkshop.Data;
// using Microsoft.AspNetCore.Identity;
// using System.Threading.Tasks;
// using System.Linq;

// [Authorize(Roles = "Admin")]
// public class AdminController : Controller
// {
//     private readonly AppDbContext _context;
//     private readonly UserManager<AppUser> _userManager;

//     public AdminController(AppDbContext context, UserManager<AppUser> userManager)
//     {
//         _context = context;
//         _userManager = userManager;
//     }

//     public IActionResult Index()
//     {
//         // Display the admin dashboard
//         return View();
//     }

//     public async Task<IActionResult> EmployeeManagement(int page = 1)
//     {
//         int pageSize = 10;

//         var allUsers = await _context.Users.ToListAsync();

//         List<AppUser> filteredEmployees = new List<AppUser>();

// 		foreach (var user in allUsers)
// 		{
// 			bool isEmployee = await _userManager.IsInRoleAsync(user, "Employee");
// 			bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

// 			if (isEmployee && !isAdmin)
// 			{
// 				filteredEmployees.Add(user);
// 			}
// 		}

// 		var employees = filteredEmployees.Skip((page - 1) * pageSize).Take(pageSize);

//         var model = new EmployeeManagementVM
//         {
//             Employees = employees.ToList(),
//             Page = page,
//             PageSize = pageSize,
//             TotalEmployees = filteredEmployees.Count
//         };

//         return View(model);
//     }

//     public IActionResult AddEmployee()
//     {
//         return View();
//     }

//     [HttpPost]
//     public async Task<IActionResult> AddEmployee(AddEmployeeVM model)
//     {
//         if (ModelState.IsValid)
//         {
//             var user = new AppUser
//             {
//                 Name = model.Name,
//                 UserName = model.Email,
//                 Email = model.Email,
//             };

//             var result = await _userManager.CreateAsync(user, model.Password!);

//             if (result.Succeeded)
//             {
//                 await _userManager.AddToRoleAsync(user, "Employee");
//                 return RedirectToAction("EmployeeManagement");
//             }

//             foreach (var error in result.Errors)
//             {
//                 ModelState.AddModelError("", error.Description);
//             }
//         }

//         return View(model);
//     }

//     public async Task<IActionResult> EditEmployee(string? id)
//     {
//         if (id == null)
//         {
//             return NotFound();
//         }

//         var employee = await _context.Users.FindAsync(id);
//         if (employee == null)
//         {
//             return NotFound();
//         }

//         return View(employee);
//     }

//     [HttpPost]
//     public async Task<IActionResult> EditEmployee(string id, AppUser updatedEmployee, string newPassword, string confirmPassword)
//     {
//         if (id != updatedEmployee.Id)
//         {
//             return NotFound();
//         }

//         if (ModelState.IsValid)
//         {
//             try
//             {
//                 var existingEmployee = await _context.Users.FindAsync(id);
//                 if (existingEmployee == null)
//                 {
//                     return NotFound();
//                 }

//                 existingEmployee.Name = updatedEmployee.Name;
//                 existingEmployee.Email = updatedEmployee.Email;

//                 if (!string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(confirmPassword))
//                 {
//                     if (newPassword != confirmPassword)
//                     {
//                         ModelState.AddModelError(string.Empty, "New password and confirmation password do not match.");
//                         return View(updatedEmployee);
//                     }

//                     var token = await _userManager.GeneratePasswordResetTokenAsync(existingEmployee);

//                     var passwordResult = await _userManager.ResetPasswordAsync(existingEmployee, token, newPassword);

//                     if (!passwordResult.Succeeded)
//                     {
//                         foreach (var error in passwordResult.Errors)
//                         {
//                             ModelState.AddModelError(string.Empty, error.Description);
//                         }
//                         return View(updatedEmployee);
//                     }
//                 }

//                 _context.Update(existingEmployee);
//                 await _context.SaveChangesAsync();

//                 return RedirectToAction("EmployeeManagement");
//             }
//             catch (DbUpdateConcurrencyException)
//             {
//                 if (!_context.Users.Any(e => e.Id == id))
//                 {
//                     return NotFound();
//                 }
//                 else
//                 {
//                     throw;
//                 }
//             }
//         }

//         return View(updatedEmployee);
//     }


//     public async Task<IActionResult> RemoveEmployee(string? id)
//     {
//         if (id == null)
//         {
//             return NotFound();
//         }

//         var employee = await _context.Users.FindAsync(id);

//         if (employee == null)
//         {
//             return NotFound();
//         }

//         return View(employee);
//     }

//     [HttpPost, ActionName("RemoveEmployee")]
//     [ValidateAntiForgeryToken]
//     public async Task<IActionResult> RemoveEmployeeConfirmed(string id)
//     {
//         var employee = await _context.Users.FindAsync(id);

//         if (employee != null)
//         {
//             _context.Users.Remove(employee);
//             await _context.SaveChangesAsync();
//         }

//         return RedirectToAction("EmployeeManagement");
//     }
// }
