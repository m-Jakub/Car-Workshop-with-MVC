using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarWorkshop.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppUserWithHourlyRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "HourlyRate",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HourlyRate",
                table: "AspNetUsers");
        }
    }
}
