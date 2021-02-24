using Microsoft.EntityFrameworkCore.Migrations;

namespace AlMualim.Migrations
{
    public partial class testNewColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestValue",
                table: "Notes",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestValue",
                table: "Notes");
        }
    }
}
