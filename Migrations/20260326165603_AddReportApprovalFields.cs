using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoMvc.Migrations
{
    /// <inheritdoc />
    public partial class AddReportApprovalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportActivities_WeeklyActivities_WeeklyActivityId",
                table: "ReportActivities");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Reports",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedById",
                table: "Reports",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignaturePath",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ApprovedById",
                table: "Reports",
                column: "ApprovedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportActivities_WeeklyActivities_WeeklyActivityId",
                table: "ReportActivities",
                column: "WeeklyActivityId",
                principalTable: "WeeklyActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_ApprovedById",
                table: "Reports",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportActivities_WeeklyActivities_WeeklyActivityId",
                table: "ReportActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_ApprovedById",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ApprovedById",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "SignaturePath",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportActivities_WeeklyActivities_WeeklyActivityId",
                table: "ReportActivities",
                column: "WeeklyActivityId",
                principalTable: "WeeklyActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
