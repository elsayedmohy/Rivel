using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverLine.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCarrierRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarrierRoutes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CarrierProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Origin = table.Column<string>(type: "text", nullable: false),
                    Destination = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarrierRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarrierRoutes_CarrierProfiles_CarrierProfileId",
                        column: x => x.CarrierProfileId,
                        principalTable: "CarrierProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarrierRoutes_CarrierProfileId",
                table: "CarrierRoutes",
                column: "CarrierProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarrierRoutes");
        }
    }
}
