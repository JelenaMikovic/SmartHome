using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nvt_back.Migrations
{
    /// <inheritdoc />
    public partial class Try1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Properties_PropertyId",
                table: "Devices");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyId",
                table: "Devices",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

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

            migrationBuilder.AlterColumn<int>(
                name: "PropertyId",
                table: "Devices",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Properties_PropertyId",
                table: "Devices",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id");
        }
    }
}
