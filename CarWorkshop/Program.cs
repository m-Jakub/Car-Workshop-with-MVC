using CarWorkshop.Data;
using CarWorkshop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("default");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<AppUser, IdentityRole>(
    options =>
    {
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

var app = builder.Build();

async Task InitializeRolesAndUsers(IServiceProvider serviceProvider)
{
    var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var UserManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

    if (!await RoleManager.RoleExistsAsync("Admin"))
        await RoleManager.CreateAsync(new IdentityRole("Admin"));

    if (!await RoleManager.RoleExistsAsync("Employee"))
        await RoleManager.CreateAsync(new IdentityRole("Employee"));

    var adminUser = await UserManager.FindByEmailAsync("admin");
    if (adminUser == null)
    {
        adminUser = new AppUser { Name = "Admin", UserName = "admin@example.com", Email = "admin@example.com" };
        var result = await UserManager.CreateAsync(adminUser, "Admin123");
        if (!result.Succeeded)
        {
			foreach (var error in result.Errors)
			{
				Console.WriteLine($"Error: {error.Code} - {error.Description}");
			}
			throw new Exception("Cannot create admin user");
        }
    }


    if (!await UserManager.IsInRoleAsync(adminUser, "Admin"))
        await UserManager.AddToRoleAsync(adminUser, "Admin");

    var otherUsers = await UserManager.Users.ToListAsync();
    foreach (var user in otherUsers)
    {
        if (user.UserName != "admin@example.com" && !await UserManager.IsInRoleAsync(user, "Employee"))
            await UserManager.AddToRoleAsync(user, "Employee");
    }
}

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        await InitializeRolesAndUsers(serviceProvider);
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
