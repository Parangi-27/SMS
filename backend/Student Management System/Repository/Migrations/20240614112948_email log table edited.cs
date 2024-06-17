using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class emaillogtableedited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromEmail",
                table: "EmailLogs");

            migrationBuilder.RenameColumn(
                name: "ToEmail",
                table: "EmailLogs",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "SubjectOfEmail",
                table: "EmailLogs",
                newName: "RecipientEmail");

            migrationBuilder.AddColumn<string>(
                name: "CC",
                table: "EmailLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentDate",
                table: "EmailLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CC",
                table: "EmailLogs");

            migrationBuilder.DropColumn(
                name: "SentDate",
                table: "EmailLogs");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "EmailLogs",
                newName: "ToEmail");

            migrationBuilder.RenameColumn(
                name: "RecipientEmail",
                table: "EmailLogs",
                newName: "SubjectOfEmail");

            migrationBuilder.AddColumn<string>(
                name: "FromEmail",
                table: "EmailLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
