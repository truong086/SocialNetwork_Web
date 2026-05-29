using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace truyenthongso.Migrations
{
    public partial class updatestatuspost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Friend_posts_postid",
                table: "Tag_Friend");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Friend_users_friend_id",
                table: "Tag_Friend");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_Friend_users_user_id",
                table: "Tag_Friend");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tag_Friend",
                table: "Tag_Friend");

            migrationBuilder.RenameTable(
                name: "Tag_Friend",
                newName: "tag_Friend");

            migrationBuilder.RenameIndex(
                name: "IX_Tag_Friend_user_id",
                table: "tag_Friend",
                newName: "IX_tag_Friend_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Tag_Friend_postid",
                table: "tag_Friend",
                newName: "IX_tag_Friend_postid");

            migrationBuilder.RenameIndex(
                name: "IX_Tag_Friend_friend_id",
                table: "tag_Friend",
                newName: "IX_tag_Friend_friend_id");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "posts",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tag_Friend",
                table: "tag_Friend",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_tag_Friend_posts_postid",
                table: "tag_Friend",
                column: "postid",
                principalTable: "posts",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_tag_Friend_users_friend_id",
                table: "tag_Friend",
                column: "friend_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tag_Friend_users_user_id",
                table: "tag_Friend",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tag_Friend_posts_postid",
                table: "tag_Friend");

            migrationBuilder.DropForeignKey(
                name: "FK_tag_Friend_users_friend_id",
                table: "tag_Friend");

            migrationBuilder.DropForeignKey(
                name: "FK_tag_Friend_users_user_id",
                table: "tag_Friend");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tag_Friend",
                table: "tag_Friend");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "posts");

            migrationBuilder.RenameTable(
                name: "tag_Friend",
                newName: "Tag_Friend");

            migrationBuilder.RenameIndex(
                name: "IX_tag_Friend_user_id",
                table: "Tag_Friend",
                newName: "IX_Tag_Friend_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_tag_Friend_postid",
                table: "Tag_Friend",
                newName: "IX_Tag_Friend_postid");

            migrationBuilder.RenameIndex(
                name: "IX_tag_Friend_friend_id",
                table: "Tag_Friend",
                newName: "IX_Tag_Friend_friend_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tag_Friend",
                table: "Tag_Friend",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Friend_posts_postid",
                table: "Tag_Friend",
                column: "postid",
                principalTable: "posts",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Friend_users_friend_id",
                table: "Tag_Friend",
                column: "friend_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_Friend_users_user_id",
                table: "Tag_Friend",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
