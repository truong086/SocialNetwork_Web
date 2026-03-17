using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace truyenthongso.Migrations
{
    public partial class dbnew0614ss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "friendships",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    UserId1 = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    UserId2 = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    status = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    cretoredat = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    updateat = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friendships", x => x.id);
                    table.ForeignKey(
                        name: "FK_friendships_users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_friendships_users_UserId2",
                        column: x => x.UserId2,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_friendships_UserId1",
                table: "friendships",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_friendships_UserId2",
                table: "friendships",
                column: "UserId2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "friendships");
        }
    }
}
