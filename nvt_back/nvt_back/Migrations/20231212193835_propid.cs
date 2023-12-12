using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nvt_back.Migrations
{
    /// <inheritdoc />
    public partial class propid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PropertyId",
                table: "Devices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_PropertyId",
                table: "Devices",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Properties_PropertyId",
                table: "Devices",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Properties_PropertyId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_PropertyId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "PropertyId",
                table: "Devices");
        }
    }
}
