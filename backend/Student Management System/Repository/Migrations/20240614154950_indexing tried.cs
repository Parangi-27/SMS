using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class indexingtried : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendances_StudentId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_TeacherId",
                table: "Attendances");

            migrationBuilder.CreateIndex(
                name: "IX_Class_Date",
                table: "Attendances",
                columns: new[] { "ClassLevel", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Student_Date",
                table: "Attendances",
                columns: new[] { "StudentId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_Subject_Date",
                table: "Attendances",
                columns: new[] { "TeacherId", "Date" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Class_Date",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Student_Date",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Teacher_Subject_Date",
                table: "Attendances");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_StudentId",
                table: "Attendances",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_TeacherId",
                table: "Attendances",
                column: "TeacherId");
        }
    }
}
