using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bachelor_Gr4_Chatbot_MVC.Migrations
{
    public partial class AlterOpeningHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateFrom",
                table: "OpeningHours");

            migrationBuilder.DropColumn(
                name: "DateTo",
                table: "OpeningHours");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OpenTo",
                table: "OpeningHours",
                nullable: false,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<DateTime>(
                name: "OpenFrom",
                table: "OpeningHours",
                nullable: false,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AddColumn<bool>(
                name: "StandardOpeningHours",
                table: "OpeningHours",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StandardOpeningHours",
                table: "OpeningHours");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "OpenTo",
                table: "OpeningHours",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "OpenFrom",
                table: "OpeningHours",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateFrom",
                table: "OpeningHours",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTo",
                table: "OpeningHours",
                type: "date",
                nullable: true);
        }
    }
}
