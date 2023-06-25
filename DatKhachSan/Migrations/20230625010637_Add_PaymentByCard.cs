using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatKhachSan.Migrations
{
    public partial class Add_PaymentByCard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentByCard",
                columns: table => new
                {
                    PaymentID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PayForID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerPayID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDescription = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentByCard", x => x.PaymentID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentByCard");
        }
    }
}
