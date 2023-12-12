using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nvt_back.Migrations
{
    /// <inheritdoc />
    public partial class sss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirConditioners");

            migrationBuilder.DropTable(
                name: "AmbientSensors");

            migrationBuilder.DropTable(
                name: "EVChargers");

            migrationBuilder.DropTable(
                name: "HomeBatteries");

            migrationBuilder.DropTable(
                name: "IrrigationSystems");

            migrationBuilder.DropTable(
                name: "Lamps");

            migrationBuilder.DropTable(
                name: "SolarPanels");

            migrationBuilder.DropTable(
                name: "VehicleGates");

            migrationBuilder.DropTable(
                name: "WashingMachines");

            migrationBuilder.AddColumn<List<string>>(
                name: "AllowedLicencePlates",
                table: "Devices",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BrightnessLevel",
                table: "Devices",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Capacity",
                table: "Devices",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ChargingPower",
                table: "Devices",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ChargingThreshold",
                table: "Devices",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Color",
                table: "Devices",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentCharge",
                table: "Devices",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentHumidity",
                table: "Devices",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentTemperature",
                table: "Devices",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Devices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Efficiency",
                table: "Devices",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Health",
                table: "Devices",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOn",
                table: "Devices",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOpened",
                table: "Devices",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivateModeOn",
                table: "Devices",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Lamp_IsOn",
                table: "Devices",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MaxTemperature",
                table: "Devices",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MinTemperature",
                table: "Devices",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPorts",
                table: "Devices",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Size",
                table: "Devices",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SolarPanel_IsOn",
                table: "Devices",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int[]>(
                name: "SupportedModes",
                table: "Devices",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdateIntervalSeconds",
                table: "Devices",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int[]>(
                name: "WashingMachine_SupportedModes",
                table: "Devices",
                type: "integer[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedLicencePlates",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "BrightnessLevel",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "ChargingPower",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "ChargingThreshold",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "CurrentCharge",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "CurrentHumidity",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "CurrentTemperature",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Efficiency",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Health",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "IsOn",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "IsOpened",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "IsPrivateModeOn",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Lamp_IsOn",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "MaxTemperature",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "MinTemperature",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "NumberOfPorts",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "SolarPanel_IsOn",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "SupportedModes",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "UpdateIntervalSeconds",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "WashingMachine_SupportedModes",
                table: "Devices");

            migrationBuilder.CreateTable(
                name: "AirConditioners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    MaxTemperature = table.Column<double>(type: "double precision", nullable: false),
                    MinTemperature = table.Column<double>(type: "double precision", nullable: false),
                    SupportedModes = table.Column<int[]>(type: "integer[]", nullable: false)
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
                name: "AmbientSensors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    CurrentHumidity = table.Column<double>(type: "double precision", nullable: false),
                    CurrentTemperature = table.Column<double>(type: "double precision", nullable: false),
                    UpdateIntervalSeconds = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmbientSensors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AmbientSensors_Devices_Id",
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
                    ChargingPower = table.Column<double>(type: "double precision", nullable: false),
                    ChargingThreshold = table.Column<double>(type: "double precision", nullable: false),
                    NumberOfPorts = table.Column<int>(type: "integer", nullable: false)
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
                    CurrentCharge = table.Column<double>(type: "double precision", nullable: false),
                    Health = table.Column<double>(type: "double precision", nullable: false)
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
                name: "Lamps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    BrightnessLevel = table.Column<int>(type: "integer", nullable: false),
                    Color = table.Column<int>(type: "integer", nullable: false),
                    IsOn = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lamps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lamps_Devices_Id",
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
                    Efficiency = table.Column<double>(type: "double precision", nullable: false),
                    IsOn = table.Column<bool>(type: "boolean", nullable: false),
                    Size = table.Column<double>(type: "double precision", nullable: false)
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
                    AllowedLicencePlates = table.Column<List<string>>(type: "text[]", nullable: false),
                    IsOpened = table.Column<bool>(type: "boolean", nullable: false),
                    IsPrivateModeOn = table.Column<bool>(type: "boolean", nullable: false)
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
        }
    }
}
