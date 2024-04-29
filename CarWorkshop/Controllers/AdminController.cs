using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
	public IActionResult Index()
	{
		// Display the admin panel dashboard
		return View();
	}

	// Additional admin actions (e.g., user management, settings) can be added here
}
