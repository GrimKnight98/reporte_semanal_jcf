using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoMvc.Migrations
{
    /// <inheritdoc />
    public partial class RefactorReportSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportActivities_Reports_ReportId",
                table: "ReportActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Instructors_InstructorId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "LearnedActivities",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "TrainingSessions",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "ReportId",
                table: "ReportActivities",
                newName: "ReportDetailId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Reports",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "InstructorId",
                table: "Reports",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Reports",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReportDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReportId = table.Column<int>(type: "INTEGER", nullable: false),
                    LearnedActivities = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    TrainingSessions = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Observations = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedById = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportDetails_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportDetails_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_UpdatedById",
                table: "Reports",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorJcfAssignments_CreatedByUserId",
                table: "InstructorJcfAssignments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorJcfAssignments_UpdatedByUserId",
                table: "InstructorJcfAssignments",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportDetails_ReportId",
                table: "ReportDetails",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportDetails_UpdatedById",
                table: "ReportDetails",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorJcfAssignments_AspNetUsers_CreatedByUserId",
                table: "InstructorJcfAssignments",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorJcfAssignments_AspNetUsers_UpdatedByUserId",
                table: "InstructorJcfAssignments",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportActivities_ReportDetails_ReportDetailId",
                table: "ReportActivities",
                column: "ReportDetailId",
                principalTable: "ReportDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_UpdatedById",
                table: "Reports",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Instructors_InstructorId",
                table: "Reports",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstructorJcfAssignments_AspNetUsers_CreatedByUserId",
                table: "InstructorJcfAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_InstructorJcfAssignments_AspNetUsers_UpdatedByUserId",
                table: "InstructorJcfAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportActivities_ReportDetails_ReportDetailId",
                table: "ReportActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_UpdatedById",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Instructors_InstructorId",
                table: "Reports");

            migrationBuilder.DropTable(
                name: "ReportDetails");

            migrationBuilder.DropIndex(
                name: "IX_Reports_UpdatedById",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_InstructorJcfAssignments_CreatedByUserId",
                table: "InstructorJcfAssignments");

            migrationBuilder.DropIndex(
                name: "IX_InstructorJcfAssignments_UpdatedByUserId",
                table: "InstructorJcfAssignments");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "ReportDetailId",
                table: "ReportActivities",
                newName: "ReportId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Reports",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "InstructorId",
                table: "Reports",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearnedActivities",
                table: "Reports",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TrainingSessions",
                table: "Reports",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportActivities_Reports_ReportId",
                table: "ReportActivities",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Instructors_InstructorId",
                table: "Reports",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
