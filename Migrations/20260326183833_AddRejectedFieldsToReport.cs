using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoMvc.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectedFieldsToReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "Reports",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedById",
                table: "Reports",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_RejectedById",
                table: "Reports",
                column: "RejectedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_RejectedById",
                table: "Reports",
                column: "RejectedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_RejectedById",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_RejectedById",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "RejectedById",
                table: "Reports");
        }
    }
}
