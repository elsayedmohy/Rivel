using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverLine.Api.Migrations
{
    /// <inheritdoc />
    public partial class EditNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationBerthId",
                table: "ShipmentRequests");

            migrationBuilder.DropColumn(
                name: "OriginBerthId",
                table: "ShipmentRequests");

            migrationBuilder.DropColumn(
                name: "DestinationBerthId",
                table: "CarrierRoutes");

            migrationBuilder.DropColumn(
                name: "OriginBerthId",
                table: "CarrierRoutes");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ShipmentRequests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ShipmentRequests");

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationBerthId",
                table: "ShipmentRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OriginBerthId",
                table: "ShipmentRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationBerthId",
                table: "CarrierRoutes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OriginBerthId",
                table: "CarrierRoutes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
