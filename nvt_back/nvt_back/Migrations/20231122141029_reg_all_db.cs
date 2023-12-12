using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nvt_back.Migrations
{
    /// <inheritdoc />
    public partial class reg_all_db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirConditioners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    SupportedModes = table.Column<int[]>(type: "integer[]", nullable: false),
                    MinTemperature = table.Column<double>(type: "double precision", nullable: false),
                    MaxTemperature = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirConditioners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AirConditioners_Devices_Id",
                        column: x => x.Id,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EVChargers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    NumberOfPorts = table.Column<int>(type: "integer", nullable: false),
                    ChargingPower = table.Column<double>(type: "double precision", nullable: false),
                    ChargingThreshold = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVChargers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EVChargers_Devices_Id",
                        column: x => x.Id,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeBatteries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Capacity = table.Column<double>(type: "double precision", nullable: false),
                    Health = table.Column<double>(type: "double precision", nullable: false),
                    CurrentCharge = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeBatteries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomeBatteries_Devices_Id",
                        column: x => x.Id,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IrrigationSystems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IsOn = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IrrigationSystems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IrrigationSystems_Devices_Id",
                        column: x => x.Id,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolarPanels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<double>(type: "double precision", nullable: false),
                    Efficiency = table.Column<double>(type: "double precision", nullable: false),
                    IsOn = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarPanels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolarPanels_Devices_Id",
                        column: x => x.Id,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleGates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IsPrivateModeOn = table.Column<bool>(type: "boolean", nullable: false),
                    AllowedLicencePlates = table.Column<List<string>>(type: "text[]", nullable: false),
                    IsOpened = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleGates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleGates_Devices_Id",
                        column: x => x.Id,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WashingMachines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    SupportedModes = table.Column<int[]>(type: "integer[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WashingMachines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WashingMachines_Devices_Id",
                        column: x => x.Id,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AmbientSensors_Devices_Id",
                table: "AmbientSensors",
                column: "Id",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AmbientSensors_Devices_Id",
                table: "AmbientSensors");

            migrationBuilder.DropTable(
                name: "AirConditioners");

            migrationBuilder.DropTable(
                name: "EVChargers");

            migrationBuilder.DropTable(
                name: "HomeBatteries");

            migrationBuilder.DropTable(
                name: "IrrigationSystems");

            migrationBuilder.DropTable(
                name: "SolarPanels");

            migrationBuilder.DropTable(
                name: "VehicleGates");

            migrationBuilder.DropTable(
                name: "WashingMachines");
        }
    }
}
