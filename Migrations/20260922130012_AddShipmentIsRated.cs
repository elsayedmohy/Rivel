using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverLine.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddShipmentIsRated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRated",
                table: "Shipments",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRated",
                table: "Shipments");
        }
    }
}
