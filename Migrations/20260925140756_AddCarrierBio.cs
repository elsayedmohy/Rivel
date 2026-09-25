using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverLine.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCarrierBio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "CarrierProfiles",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "CarrierProfiles");
        }
    }
}
