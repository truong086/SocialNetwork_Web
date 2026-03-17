using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace truyenthongso.Migrations
{
    public partial class updatedb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "users");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "tagPosts");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "sheres");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "ratings");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "postImages");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "notifitions");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "newsSourceProviders");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "newspaperTypes");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "newspaperSources");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "likes");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "interests");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "groupRoles");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "complaints");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "CommentDescription");

            migrationBuilder.DropColumn(
                name: "cretoredit",
                table: "categories");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "users",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "users",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "tags",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "tags",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "tagPosts",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "tagPosts",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "sheres",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "sheres",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "roles",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "roles",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "ratings",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "ratings",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "posts",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "posts",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "postImages",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "postImages",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "notifitions",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "notifitions",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "newsSourceProviders",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "newsSourceProviders",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "newspaperTypes",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "newspaperTypes",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "newspaperSources",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "newspaperSources",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "likes",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "likes",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "interests",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "interests",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "groups",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "groups",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "groupRoles",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "groupRoles",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "complaints",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "complaints",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "comments",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "comments",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "CommentDescription",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "CommentDescription",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "cretoredat",
                table: "categories",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updateat",
                table: "categories",
                type: "TIMESTAMP(7) WITH TIME ZONE",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "users");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "users");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "tagPosts");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "tagPosts");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "sheres");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "sheres");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "ratings");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "ratings");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "postImages");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "postImages");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "notifitions");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "notifitions");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "newsSourceProviders");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "newsSourceProviders");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "newspaperTypes");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "newspaperTypes");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "newspaperSources");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "newspaperSources");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "likes");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "likes");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "interests");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "interests");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "groupRoles");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "groupRoles");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "complaints");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "complaints");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "CommentDescription");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "CommentDescription");

            migrationBuilder.DropColumn(
                name: "cretoredat",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "updateat",
                table: "categories");

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "users",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "tags",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "tagPosts",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "sheres",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "roles",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "ratings",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "posts",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "postImages",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "notifitions",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "newsSourceProviders",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "newspaperTypes",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "newspaperSources",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "likes",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "interests",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "groups",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "groupRoles",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "complaints",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "comments",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "CommentDescription",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cretoredit",
                table: "categories",
                type: "NVARCHAR2(2000)",
                nullable: true);
        }
    }
}
