using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nvt_back.Migrations
{
    /// <inheritdoc />
    public partial class reg_all_db_all : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeviceType",
                table: "Devices",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "Devices");
        }
    }
}
