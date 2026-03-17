using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace truyenthongso.Migrations
{
    public partial class dbnew0615 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "userStoryViews",
                columns: table => new
                {
                    id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    user_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    post_id = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    deleted = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    creator = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    cretoredat = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    updateat = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userStoryViews", x => x.id);
                    table.ForeignKey(
                        name: "FK_userStoryViews_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_userStoryViews_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_userStoryViews_post_id",
                table: "userStoryViews",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_userStoryViews_user_id",
                table: "userStoryViews",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "userStoryViews");
        }
    }
}
