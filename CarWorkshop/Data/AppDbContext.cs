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

			// Configure HourlyRate
			modelBuilder.Entity<AppUser>()
				.Property(u => u.HourlyRate)
				.HasColumnType("decimal(18, 2)");

			// Configure relationship between CalendarEvent and AppUser
			modelBuilder.Entity<CalendarEvent>()
				.HasOne(c => c.Employee)
				.WithMany()
				.HasForeignKey(c => c.EmployeeId);
		}
		public DbSet<CalendarEvent> CalendarEvents { get; set; }
	    public DbSet<CarWorkshop.Models.Ticket> Ticket { get; set; } = default!;
	    public DbSet<CarWorkshop.Models.Part> Part { get; set; } = default!;

	}
}
