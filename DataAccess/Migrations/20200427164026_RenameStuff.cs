using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class RenameStuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Expense");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Expense");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Expense");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Expense",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Expense",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "Expense",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Expense",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Expense");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Expense");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "Expense");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Expense");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Expense",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Expense",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Expense",
                type: "timestamp without time zone",
                nullable: true);
        }
    }
}
