using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace truyenthongso.Migrations
{
    public partial class updatedb0630 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "CommentDescription",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "CommentDescription",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "descript",
                table: "CommentDescription",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentDescription_CommentDescript3_2",
                table: "CommentDescription",
                column: "CommentDescript3_2");

            migrationBuilder.CreateIndex(
                name: "IX_CommentDescription_CommentDescription_id",
                table: "CommentDescription",
                column: "CommentDescription_id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentDescription_CommentDescription_CommentDescript3_2",
                table: "CommentDescription",
                column: "CommentDescript3_2",
                principalTable: "CommentDescription",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentDescription_CommentDescription_CommentDescription_id",
                table: "CommentDescription",
                column: "CommentDescription_id",
                principalTable: "CommentDescription",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentDescription_CommentDescription_CommentDescript3_2",
                table: "CommentDescription");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentDescription_CommentDescription_CommentDescription_id",
                table: "CommentDescription");

            migrationBuilder.DropIndex(
                name: "IX_CommentDescription_CommentDescript3_2",
                table: "CommentDescription");

            migrationBuilder.DropIndex(
                name: "IX_CommentDescription_CommentDescription_id",
                table: "CommentDescription");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "CommentDescription");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "CommentDescription");

            migrationBuilder.DropColumn(
                name: "descript",
                table: "CommentDescription");
        }
    }
}
