using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverLine.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCarrierLogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoPath",
                table: "CarrierProfiles",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoPath",
                table: "CarrierProfiles");
        }
    }
}
