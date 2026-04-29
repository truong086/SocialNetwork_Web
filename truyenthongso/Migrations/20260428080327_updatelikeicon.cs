using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace truyenthongso.Migrations
{
    public partial class updatelikeicon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "icon_id",
                table: "likes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "url_icon",
                table: "likes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_likes_icon_id",
                table: "likes",
                column: "icon_id");

            migrationBuilder.AddForeignKey(
                name: "FK_likes_icons_icon_id",
                table: "likes",
                column: "icon_id",
                principalTable: "icons",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_likes_icons_icon_id",
                table: "likes");

            migrationBuilder.DropIndex(
                name: "IX_likes_icon_id",
                table: "likes");

            migrationBuilder.DropColumn(
                name: "icon_id",
                table: "likes");

            migrationBuilder.DropColumn(
                name: "url_icon",
                table: "likes");
        }
    }
}
