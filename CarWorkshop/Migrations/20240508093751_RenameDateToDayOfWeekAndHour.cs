using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarWorkshop.Migrations
{
    /// <inheritdoc />
    public partial class RenameDateToDayOfWeekAndHour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "CalendarEvents");

            migrationBuilder.AddColumn<int>(
                name: "DayOfWeek",
                table: "CalendarEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Hour",
                table: "CalendarEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "CalendarEvents");

            migrationBuilder.DropColumn(
                name: "Hour",
                table: "CalendarEvents");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "CalendarEvents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
