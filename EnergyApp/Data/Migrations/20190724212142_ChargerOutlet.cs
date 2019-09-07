using Microsoft.EntityFrameworkCore.Migrations;

namespace EnergyApp.Data.Migrations
{
    public partial class ChargerOutlet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chargers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OutletID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    MaxCurrent = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chargers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Outlets",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    MaxCurrent = table.Column<float>(nullable: false),
                    Type = table.Column<int>(nullable: true),
                    ChargerID = table.Column<int>(nullable: true),
                    CustomerID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outlets", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Outlets_Chargers_ChargerID",
                        column: x => x.ChargerID,
                        principalTable: "Chargers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Outlets_Customers_CustomerID",
                        column: x => x.ChargerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Outlets_ChargerID",
                table: "Outlets",
                column: "ChargerID");
                
            migrationBuilder.CreateIndex(
                name: "IX_Outlets_CustomerID",
                table: "Outlets",
                column: "CustomerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Outlets");

            migrationBuilder.DropTable(
                name: "Chargers");

        }
    }
}
