using CarWorkshop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarWorkshop.Data
{
	public class AppDbContext : IdentityDbContext<AppUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Configure the HourlyRate property of the AppUser entity
			modelBuilder.Entity<AppUser>()
				.Property(u => u.HourlyRate)
				.HasColumnType("decimal(18, 2)"); // Adjust precision and scale as needed
		}
	}
}
