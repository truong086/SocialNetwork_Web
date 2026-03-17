using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace truyenthongso.Migrations
{
    public partial class dbnew0614toi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "users",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "tokens",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "tags",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "tagPosts",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "sheres",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "roles",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "ratings",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "posts",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "postImages",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "notifitions",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "newsSourceProviders",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "newspaperTypes",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "newspaperSources",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "likes",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "interests",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "groups",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "groupRoles",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "friendships",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "complaints",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "comments",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "CommentDescription",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator",
                table: "categories",
                type: "NVARCHAR2(2000)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "creator",
                table: "users");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "tokens");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "tagPosts");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "sheres");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "ratings");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "postImages");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "notifitions");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "newsSourceProviders");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "newspaperTypes");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "newspaperSources");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "likes");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "interests");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "groupRoles");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "friendships");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "complaints");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "CommentDescription");

            migrationBuilder.DropColumn(
                name: "creator",
                table: "categories");
        }
    }
}
