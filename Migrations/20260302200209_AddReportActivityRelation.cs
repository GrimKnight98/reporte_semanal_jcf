using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoMvc.Migrations
{
    /// <inheritdoc />
    public partial class AddReportActivityRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportActivities",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "INTEGER", nullable: false),
                    WeeklyActivityId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportActivities", x => new { x.ReportId, x.WeeklyActivityId });
                    table.ForeignKey(
                        name: "FK_ReportActivities_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportActivities_WeeklyActivities_WeeklyActivityId",
                        column: x => x.WeeklyActivityId,
                        principalTable: "WeeklyActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportActivities_WeeklyActivityId",
                table: "ReportActivities",
                column: "WeeklyActivityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportActivities");
        }
    }
}
