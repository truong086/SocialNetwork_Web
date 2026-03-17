using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace truyenthongso.Migrations
{
    public partial class updatenew0614sang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "likes",
                type: "NUMBER(10)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "likes");
        }
    }
}
