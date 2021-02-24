using Microsoft.EntityFrameworkCore.Migrations;

namespace AlMualim.Migrations
{
    public partial class testRemoveColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestValue",
                table: "Notes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestValue",
                table: "Notes",
                type: "int",
                nullable: true);
        }
    }
}
