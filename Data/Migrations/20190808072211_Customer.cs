using Microsoft.EntityFrameworkCore.Migrations;

namespace EnergyApp.Data.Migrations
{
    public partial class Customer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "Meters",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "Chargers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    CustomerNumber = table.Column<string>(nullable: true),
                    MeterID = table.Column<int>(nullable: false),
                    ChargerID = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Meters_CustomerID",
                table: "Meters",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Chargers_CustomerID",
                table: "Chargers",
                column: "CustomerID");

            /* migrationBuilder.AddForeignKey(
                name: "FK_Chargers_Customers_CustomerID",
                table: "Chargers",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Meters_Customers_CustomerID",
                table: "Meters",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
*/
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chargers_Customers_CustomerID",
                table: "Chargers");

            migrationBuilder.DropForeignKey(
                name: "FK_Meters_Customers_CustomerID",
                table: "Meters");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Meters_CustomerID",
                table: "Meters");

            migrationBuilder.DropIndex(
                name: "IX_Chargers_CustomerID",
                table: "Chargers");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "Meters");

            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "Chargers");
        }
    }
}
