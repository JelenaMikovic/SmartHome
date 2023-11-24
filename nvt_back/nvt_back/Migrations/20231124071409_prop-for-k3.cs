using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nvt_back.Migrations
{
    /// <inheritdoc />
    public partial class propfork3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Properties_PropertyId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_PropertyId",
                table: "Devices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
