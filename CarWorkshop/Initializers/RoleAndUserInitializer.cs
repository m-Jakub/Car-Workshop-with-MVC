using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarWorkshop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class RoleAndUserInitializer
{
	private readonly RoleManager<IdentityRole> roleManager;
	private readonly UserManager<AppUser> userManager;
	private readonly ILogger<RoleAndUserInitializer> logger;

	public RoleAndUserInitializer(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, ILogger<RoleAndUserInitializer> logger)
	{
		this.roleManager = roleManager;
		this.userManager = userManager;
		this.logger = logger;
	}

	public async Task InitializeRolesAndUsers()
	{
		if (!await roleManager.RoleExistsAsync("Admin"))
			await roleManager.CreateAsync(new IdentityRole("Admin"));

		if (!await roleManager.RoleExistsAsync("Employee"))
			await roleManager.CreateAsync(new IdentityRole("Employee"));

		var adminUser = await userManager.FindByEmailAsync("admin@example.com");
		if (adminUser == null)
		{
			adminUser = new AppUser { Name = "Admin", UserName = "admin@example.com", Email = "admin@example.com" };
			var result = await userManager.CreateAsync(adminUser, "Admin123");
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					logger.LogError($"Error: {error.Code} - {error.Description}");
				}
				throw new Exception("Cannot create admin user");
			}
		}

		if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
			await userManager.AddToRoleAsync(adminUser, "Admin");

		var otherUsers = await userManager.Users.ToListAsync();
		foreach (var user in otherUsers)
		{
			if (user.UserName != "admin@example.com" && !await userManager.IsInRoleAsync(user, "Employee"))
				await userManager.AddToRoleAsync(user, "Employee");
		}
	}
}
