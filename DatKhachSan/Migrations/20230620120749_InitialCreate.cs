using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatKhachSan.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalGuests",
                table: "Bookings",
                newName: "NumberChildren");

            migrationBuilder.AddColumn<int>(
                name: "NumberAdults",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberAdults",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "NumberChildren",
                table: "Bookings",
                newName: "TotalGuests");
        }
    }
}
