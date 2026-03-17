using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace truyenthongso.Migrations
{
    public partial class dbnew0616share : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "sheres",
                type: "NUMBER(10)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "sheres",
                type: "NVARCHAR2(2000)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "sheres");

            migrationBuilder.DropColumn(
                name: "title",
                table: "sheres");
        }
    }
}
