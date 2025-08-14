using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetCoreArchTemplate.Data.Migrations
{
    /// <inheritdoc />
    public partial class DealershipDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Dealership_DealershipId",
                table: "Cars");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dealership",
                table: "Dealership");

            migrationBuilder.RenameTable(
                name: "Dealership",
                newName: "Dealerships");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dealerships",
                table: "Dealerships",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Dealerships_DealershipId",
                table: "Cars",
                column: "DealershipId",
                principalTable: "Dealerships",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Dealerships_DealershipId",
                table: "Cars");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dealerships",
                table: "Dealerships");

            migrationBuilder.RenameTable(
                name: "Dealerships",
                newName: "Dealership");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dealership",
                table: "Dealership",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Dealership_DealershipId",
                table: "Cars",
                column: "DealershipId",
                principalTable: "Dealership",
                principalColumn: "Id");
        }
    }
}
