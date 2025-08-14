using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetCoreArchTemplate.Data.Migrations
{
    /// <inheritdoc />
    public partial class Dealership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DealershipId",
                table: "Cars",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Dealership",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Logo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dealership", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cars_DealershipId",
                table: "Cars",
                column: "DealershipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Dealership_DealershipId",
                table: "Cars",
                column: "DealershipId",
                principalTable: "Dealership",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Dealership_DealershipId",
                table: "Cars");

            migrationBuilder.DropTable(
                name: "Dealership");

            migrationBuilder.DropIndex(
                name: "IX_Cars_DealershipId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "DealershipId",
                table: "Cars");
        }
    }
}
