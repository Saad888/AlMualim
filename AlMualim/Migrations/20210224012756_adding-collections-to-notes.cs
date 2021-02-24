﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace AlMualim.Migrations
{
    public partial class addingcollectionstonotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
