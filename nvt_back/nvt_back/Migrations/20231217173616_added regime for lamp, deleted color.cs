using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace nvt_back.Migrations
{
    /// <inheritdoc />
    public partial class addedregimeforlampdeletedcolor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Color",
                table: "Lamps",
                newName: "Regime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Regime",
                table: "Lamps",
                newName: "Color");
        }
    }
}
