using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoMvc.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueConstraintInstructorJcf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InstructorJcfAssignments_InstructorUserId_JcfUserId",
                table: "InstructorJcfAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorJcfAssignments_InstructorUserId_JcfUserId",
                table: "InstructorJcfAssignments",
                columns: new[] { "InstructorUserId", "JcfUserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InstructorJcfAssignments_InstructorUserId_JcfUserId",
                table: "InstructorJcfAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorJcfAssignments_InstructorUserId_JcfUserId",
                table: "InstructorJcfAssignments",
                columns: new[] { "InstructorUserId", "JcfUserId" },
                unique: true);
        }
    }
}
