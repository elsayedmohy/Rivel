using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverLine.Api.Migrations
{
    /// <inheritdoc />
    public partial class EditVessels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vessels_CarrierProfileId",
                table: "Vessels");

            migrationBuilder.AlterColumn<decimal>(
                name: "Capacity",
                table: "Vessels",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Vessels",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "YearBuilt",
                table: "Vessels",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "ShipmentRequests",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.CreateIndex(
                name: "IX_Vessels_CarrierProfileId_IsArchived",
                table: "Vessels",
                columns: new[] { "CarrierProfileId", "IsArchived" });

            migrationBuilder.CreateIndex(
                name: "IX_Vessels_RegistrationNumber",
                table: "Vessels",
                column: "RegistrationNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vessels_CarrierProfileId_IsArchived",
                table: "Vessels");

            migrationBuilder.DropIndex(
                name: "IX_Vessels_RegistrationNumber",
                table: "Vessels");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Vessels");

            migrationBuilder.DropColumn(
                name: "YearBuilt",
                table: "Vessels");

            migrationBuilder.AlterColumn<double>(
                name: "Capacity",
                table: "Vessels",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "ShipmentRequests",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.CreateIndex(
                name: "IX_Vessels_CarrierProfileId",
                table: "Vessels",
                column: "CarrierProfileId");
        }
    }
}
