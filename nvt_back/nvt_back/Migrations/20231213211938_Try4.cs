using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nvt_back.Migrations
{
    /// <inheritdoc />
    public partial class Try4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfPanels",
                table: "SolarPanels",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfPanels",
                table: "SolarPanels");
        }
    }
}
