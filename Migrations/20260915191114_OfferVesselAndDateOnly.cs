using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverLine.Api.Migrations
{
    /// <inheritdoc />
    public partial class OfferVesselAndDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "ProposedPickupDate",
                table: "Offers",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<Guid>(
                name: "VesselId",
                table: "Offers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Offers_VesselId",
                table: "Offers",
                column: "VesselId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Vessels_VesselId",
                table: "Offers",
                column: "VesselId",
                principalTable: "Vessels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Vessels_VesselId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_VesselId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "VesselId",
                table: "Offers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProposedPickupDate",
                table: "Offers",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
