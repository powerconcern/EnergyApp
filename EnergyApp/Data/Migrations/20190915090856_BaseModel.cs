using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EnergyApp.Data.Migrations
{
    public partial class BaseModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chargers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    MaxCurrent = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chargers", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 50, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Meters",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    MaxCurrent = table.Column<float>(nullable: false),
                    Type = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meters", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Partners",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    UserReference = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partners", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ChargeSession",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false),
                    Kwh = table.Column<int>(nullable: false),
                    OutletID = table.Column<int>(nullable: false),
                    ChargerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeSession", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChargeSession_Chargers_ChargerID",
                        column: x => x.ChargerID,
                        principalTable: "Chargers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Outlets",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    ChargerID = table.Column<int>(nullable: false),
                    MaxCurrent = table.Column<float>(nullable: false),
                    Type = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outlets", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Outlets_Chargers_ChargerID",
                        column: x => x.ChargerID,
                        principalTable: "Chargers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CMPAssignments",
                columns: table => new
                {
                    MeterID = table.Column<int>(nullable: false),
                    ChargerID = table.Column<int>(nullable: false),
                    PartnerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMPAssignments", x => new { x.ChargerID, x.PartnerID, x.MeterID });
                    table.ForeignKey(
                        name: "FK_CMPAssignments_Chargers_ChargerID",
                        column: x => x.ChargerID,
                        principalTable: "Chargers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CMPAssignments_Meters_MeterID",
                        column: x => x.MeterID,
                        principalTable: "Meters",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CMPAssignments_Partners_PartnerID",
                        column: x => x.PartnerID,
                        principalTable: "Partners",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Adjustment",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Current = table.Column<int>(nullable: false),
                    iPhase = table.Column<int>(nullable: false),
                    CMPAssignmentChargerID = table.Column<int>(nullable: true),
                    CMPAssignmentMeterID = table.Column<int>(nullable: true),
                    CMPAssignmentPartnerID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adjustment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Adjustment_CMPAssignments_CMPAssignmentChargerID_CMPAssignmentPartnerID_CMPAssignmentMeterID",
                        columns: x => new { x.CMPAssignmentChargerID, x.CMPAssignmentPartnerID, x.CMPAssignmentMeterID },
                        principalTable: "CMPAssignments",
                        principalColumns: new[] { "ChargerID", "PartnerID", "MeterID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adjustment_CMPAssignmentChargerID_CMPAssignmentPartnerID_CMPAssignmentMeterID",
                table: "Adjustment",
                columns: new[] { "CMPAssignmentChargerID", "CMPAssignmentPartnerID", "CMPAssignmentMeterID" });

            migrationBuilder.CreateIndex(
                name: "IX_ChargeSession_ChargerID",
                table: "ChargeSession",
                column: "ChargerID");

            migrationBuilder.CreateIndex(
                name: "IX_CMPAssignments_MeterID",
                table: "CMPAssignments",
                column: "MeterID");

            migrationBuilder.CreateIndex(
                name: "IX_CMPAssignments_PartnerID",
                table: "CMPAssignments",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_Outlets_ChargerID",
                table: "Outlets",
                column: "ChargerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adjustment");

            migrationBuilder.DropTable(
                name: "ChargeSession");

            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "Outlets");

            migrationBuilder.DropTable(
                name: "CMPAssignments");

            migrationBuilder.DropTable(
                name: "Chargers");

            migrationBuilder.DropTable(
                name: "Meters");

            migrationBuilder.DropTable(
                name: "Partners");
        }
    }
}
