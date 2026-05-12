using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace truyenthongso.Migrations
{
    public partial class updatetagfriend : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tag_Friend",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    post_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    friend_id = table.Column<int>(type: "int", nullable: true),
                    postid = table.Column<int>(type: "int", nullable: true),
                    deleted = table.Column<bool>(type: "bit", nullable: false),
                    creator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cretoredat = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    updateat = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag_Friend", x => x.id);
                    table.ForeignKey(
                        name: "FK_Tag_Friend_posts_postid",
                        column: x => x.postid,
                        principalTable: "posts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Tag_Friend_users_friend_id",
                        column: x => x.friend_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tag_Friend_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tag_Friend_friend_id",
                table: "Tag_Friend",
                column: "friend_id");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_Friend_postid",
                table: "Tag_Friend",
                column: "postid");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_Friend_user_id",
                table: "Tag_Friend",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tag_Friend");
        }
    }
}
