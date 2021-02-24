using Microsoft.EntityFrameworkCore.Migrations;

namespace AlMualim.Migrations
{
    public partial class addingcollectionstotopics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Notes_NotesID",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_NotesID",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "NotesID",
                table: "Topics");

            migrationBuilder.CreateTable(
                name: "NotesTopics",
                columns: table => new
                {
                    NotesID = table.Column<int>(type: "int", nullable: false),
                    TopicsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotesTopics", x => new { x.NotesID, x.TopicsID });
                    table.ForeignKey(
                        name: "FK_NotesTopics_Notes_NotesID",
                        column: x => x.NotesID,
                        principalTable: "Notes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotesTopics_Topics_TopicsID",
                        column: x => x.TopicsID,
                        principalTable: "Topics",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotesTopics_TopicsID",
                table: "NotesTopics",
                column: "TopicsID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotesTopics");

            migrationBuilder.AddColumn<int>(
                name: "NotesID",
                table: "Topics",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_NotesID",
                table: "Topics",
                column: "NotesID");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Notes_NotesID",
                table: "Topics",
                column: "NotesID",
                principalTable: "Notes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
