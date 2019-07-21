using Microsoft.EntityFrameworkCore.Migrations;

namespace EnergyApp.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    ConfigurationID = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 50, nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.ConfigurationID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configurations");
        }
    }
}
