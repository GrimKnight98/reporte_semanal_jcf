using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoMvc.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReportInstructorRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DoubtSolver",
                table: "Reports",
                newName: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_InstructorId",
                table: "Reports",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Instructors_InstructorId",
                table: "Reports",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Instructors_InstructorId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_InstructorId",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "InstructorId",
                table: "Reports",
                newName: "DoubtSolver");
        }
    }
}
