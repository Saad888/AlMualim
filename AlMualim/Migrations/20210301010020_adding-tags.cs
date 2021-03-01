using Microsoft.EntityFrameworkCore.Migrations;

namespace AlMualim.Migrations
{
    public partial class addingtags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "NotesTags",
                columns: table => new
                {
                    NotesID = table.Column<int>(type: "int", nullable: false),
                    TagsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotesTags", x => new { x.NotesID, x.TagsID });
                    table.ForeignKey(
                        name: "FK_NotesTags_Notes_NotesID",
                        column: x => x.NotesID,
                        principalTable: "Notes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotesTags_Tags_TagsID",
                        column: x => x.TagsID,
                        principalTable: "Tags",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotesTags_TagsID",
                table: "NotesTags",
                column: "TagsID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotesTags");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
