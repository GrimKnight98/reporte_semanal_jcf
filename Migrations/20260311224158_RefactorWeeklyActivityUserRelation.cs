using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoMvc.Migrations
{
    /// <inheritdoc />
    public partial class RefactorWeeklyActivityUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "WeeklyActivities");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "WeeklyActivities",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyActivities_CreatedByUserId",
                table: "WeeklyActivities",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WeeklyActivities_AspNetUsers_CreatedByUserId",
                table: "WeeklyActivities",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeeklyActivities_AspNetUsers_CreatedByUserId",
                table: "WeeklyActivities");

            migrationBuilder.DropIndex(
                name: "IX_WeeklyActivities_CreatedByUserId",
                table: "WeeklyActivities");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "WeeklyActivities");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "WeeklyActivities",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
