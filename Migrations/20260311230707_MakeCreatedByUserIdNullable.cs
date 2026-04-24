using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoMvc.Migrations
{
    public partial class MakeCreatedByUserIdNullable : Migration
    {
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AlterColumn<string>(
        name: "CreatedByUserId",
        table: "WeeklyActivities",
        type: "nvarchar(450)",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "nvarchar(450)");

    // Si CreatedByUser también quedó como NOT NULL, asegúrate de que sea nullable
    // pero normalmente EF no crea columna para la navegación, solo para el FK
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AlterColumn<string>(
        name: "CreatedByUserId",
        table: "WeeklyActivities",
        type: "nvarchar(450)",
        nullable: false,
        oldClrType: typeof(string),
        oldType: "nvarchar(450)",
        oldNullable: true);
}

    }
}
