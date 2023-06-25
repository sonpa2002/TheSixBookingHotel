using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatKhachSan.Migrations
{
    public partial class AddColum_PaymentByCard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "PaymentByCard",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "PaymentByCard");
        }
    }
}
