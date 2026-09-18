using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverLine.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVesselEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Vessels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "Vessels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                                     ALTER TABLE "Vessels"
                                     ALTER COLUMN "Type" TYPE integer
                                     USING CASE "Type"
                                         WHEN 'Barge' THEN 0
                                         WHEN 'SelfPropelledBarge' THEN 1
                                         WHEN 'Tugboat' THEN 2
                                         WHEN 'PushBoat' THEN 3
                                         WHEN 'CargoVessel' THEN 4
                                         WHEN 'BulkCarrier' THEN 5
                                         WHEN 'ContainerBarge' THEN 6
                                         WHEN 'TankBarge' THEN 7
                                         WHEN 'RoRo' THEN 8
                                         ELSE 0
                                     END;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Vessels");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "Vessels");

            migrationBuilder.Sql("""
                                     ALTER TABLE "Vessels"
                                     ALTER COLUMN "Type" TYPE text
                                     USING CASE "Type"
                                         WHEN 0 THEN 'Barge'
                                         WHEN 1 THEN 'SelfPropelledBarge'
                                         WHEN 2 THEN 'Tugboat'
                                         WHEN 3 THEN 'PushBoat'
                                         WHEN 4 THEN 'CargoVessel'
                                         WHEN 5 THEN 'BulkCarrier'
                                         WHEN 6 THEN 'ContainerBarge'
                                         WHEN 7 THEN 'TankBarge'
                                         WHEN 8 THEN 'RoRo'
                                         ELSE 'Barge'
                                     END;
                                 """);
        }
    }
}
