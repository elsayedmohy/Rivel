using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RiverLine.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNileBerth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Destination",
                table: "ShipmentRequests");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "ShipmentRequests");

            migrationBuilder.DropColumn(
                name: "Destination",
                table: "CarrierRoutes");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "CarrierRoutes");

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationBerthId",
                table: "ShipmentRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationNileBerthId",
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
                name: "OriginNileBerthId",
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
                name: "DestinationNileBerthId",
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

            migrationBuilder.AddColumn<Guid>(
                name: "OriginNileBerthId",
                table: "CarrierRoutes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "NileBerths",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ArabicName = table.Column<string>(type: "text", nullable: false),
                    Governorate = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Axis = table.Column<int>(type: "integer", nullable: false),
                    CoordinateAccuracy = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NileBerths", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "NileBerths",
                columns: new[] { "Id", "ArabicName", "Axis", "CoordinateAccuracy", "Governorate", "IsActive", "Latitude", "Longitude", "Name", "Type" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "ميناء دمياط النهري", 1, 0, "دمياط", true, 31.460344m, 31.756536m, "Damietta River Port", 0 },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "رصيف طلخا", 1, 0, "الدقهلية", true, 31.059511m, 31.398000m, "Talkha Pier", 3 },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "رصيف طلخا 2", 1, 0, "الدقهلية", true, 31.048407m, 31.357553m, "Talkha Pier 2", 3 },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "رصيف زفتى النهري", 1, 0, "الغربية", true, 30.710902m, 31.255163m, "Zifta River Dock", 2 },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "القناطر الخيرية", 1, 1, "القليوبية", true, 30.205m, 31.126m, "Delta Barrage", 4 },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "ميناء بنها النهري", 1, 1, "القليوبية", true, 30.466m, 31.185m, "Banha River Port", 0 },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "آثار النبي", 0, 0, "القاهرة", true, 30.115057m, 31.215250m, "Athar El Nabi", 2 },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "إمبابة الناقلات", 0, 1, "الجيزة", true, 30.062m, 31.207m, "Imbaba Tankers", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "ميناء إمبابة النهري", 0, 1, "الجيزة", true, 30.076m, 31.208m, "Imbaba River Port", 0 },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "التبين - حجر جيري", 0, 1, "القاهرة", true, 29.805m, 31.331m, "El Tebbin Limestone", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "التبين - كوك", 0, 1, "القاهرة", true, 29.798m, 31.320m, "El Tebbin Coke", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "التبين النهري", 0, 1, "القاهرة", true, 29.800m, 31.325m, "El Tebbin El Nahree", 2 },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "طرة - أسمنت", 0, 1, "القاهرة", true, 29.927m, 31.281m, "Tora Cement", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "المسارة", 0, 1, "القاهرة", true, 29.906m, 31.282m, "El Masara", 2 },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "القومية - أسمنت", 0, 1, "بني سويف", true, 29.95m, 31.20m, "El Kawmiya Cement", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "سمالوط - أسمنت", 0, 1, "المنيا", true, 28.312m, 30.711m, "Samalout Cement", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "سمالوط - عقدة نهرية", 0, 1, "المنيا", true, 28.312m, 30.711m, "Samalout River Node", 2 },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "ميناء المنيا النهري", 0, 1, "المنيا", true, 28.109m, 30.750m, "Minya River Port", 0 },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "بني خالد / سمالوط", 0, 1, "المنيا", true, 28.420m, 30.760m, "Bany Khaled / Samalout", 2 },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "ميناء المنيا اللوجستي", 0, 1, "المنيا", true, 28.110m, 30.745m, "Minya Logistics River Port", 0 },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "العقبة", 0, 1, "أسيوط", true, 27.22m, 31.18m, "El Akaba", 2 },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "محطة أسيوط - حبوب", 0, 1, "أسيوط", true, 27.180m, 31.185m, "Asyut Calories Station", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000023"), "ميناء أسيوط البترولي", 0, 1, "أسيوط", true, 27.18m, 31.19m, "Asyut Petrol Port", 0 },
                    { new Guid("00000000-0000-0000-0000-000000000024"), "أسيوط أسمنت - منقباد", 0, 1, "أسيوط", true, 27.215m, 31.190m, "Asyut Cement – Menkbad", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000025"), "ميناء منقباد - أسمدة", 0, 1, "أسيوط", true, 27.218m, 31.185m, "Menkbad Fertilizer Port", 0 },
                    { new Guid("00000000-0000-0000-0000-000000000026"), "ميناء أسيوط النهري", 0, 1, "أسيوط", true, 27.180m, 31.183m, "Asyut River Port", 0 },
                    { new Guid("00000000-0000-0000-0000-000000000027"), "ميناء سوهاج النهري", 0, 1, "سوهاج", true, 26.56m, 31.70m, "Sohag River Port", 0 },
                    { new Guid("00000000-0000-0000-0000-000000000028"), "جرجا - سكر", 0, 1, "سوهاج", true, 26.34m, 31.89m, "Gerga Sugar", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000029"), "البلينا", 0, 1, "سوهاج", true, 26.25m, 32.00m, "El Balina", 2 },
                    { new Guid("00000000-0000-0000-0000-000000000030"), "ألومنيوم النهر", 0, 1, "قنا", true, 26.05m, 32.72m, "River Aluminum", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000031"), "نجع حمادي - سكر", 0, 1, "قنا", true, 26.04m, 32.24m, "Nagaa Hammady Sugar", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000032"), "دشنا - سكر", 0, 1, "قنا", true, 26.09m, 32.58m, "Dishna Sugar", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000033"), "قوص - سكر", 0, 1, "قنا", true, 25.91m, 32.76m, "Koss Sugar", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000034"), "ميناء دندرة النهري", 0, 1, "قنا", true, 26.14m, 32.66m, "Dandara River Port", 0 },
                    { new Guid("00000000-0000-0000-0000-000000000035"), "ميناء قنا النهري", 0, 1, "قنا", true, 26.16m, 32.72m, "Qena River Port", 0 },
                    { new Guid("00000000-0000-0000-0000-000000000036"), "ميناء الأقصر النهري", 0, 1, "الأقصر", true, 25.69m, 32.64m, "Luxor River Port", 0 },
                    { new Guid("00000000-0000-0000-0000-000000000037"), "أرمنت - سكر", 0, 1, "الأقصر", true, 25.62m, 32.55m, "Armant Sugar", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000038"), "ميناء إسنا النهري", 0, 1, "الأقصر", true, 25.29m, 32.55m, "Esna River Port", 0 },
                    { new Guid("00000000-0000-0000-0000-000000000039"), "السباعية", 0, 1, "الأقصر", true, 25.52m, 32.74m, "El Sibaaya", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000040"), "إدفو - سكر", 0, 1, "أسوان", true, 24.98m, 32.88m, "Edfu Sugar", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000041"), "رصيف إدفو النهري", 0, 1, "أسوان", true, 24.98m, 32.88m, "Edfu River Dock", 2 },
                    { new Guid("00000000-0000-0000-0000-000000000042"), "ميناء أسوان / العقب", 0, 1, "أسوان", true, 24.10m, 32.90m, "Aswan River Port / El Akab", 0 },
                    { new Guid("00000000-0000-0000-0000-000000000043"), "ميناء أسوان النهري", 0, 1, "أسوان", true, 24.09m, 32.90m, "Aswan River Port", 0 },
                    { new Guid("00000000-0000-0000-0000-000000000044"), "رصيف أبو سمبل", 2, 1, "أسوان", true, 22.34m, 31.62m, "Abu Simbel Pier", 3 },
                    { new Guid("00000000-0000-0000-0000-000000000045"), "توشكى / عمدة", 2, 1, "أسوان", true, 22.80m, 31.20m, "Tushka / Amada", 4 },
                    { new Guid("00000000-0000-0000-0000-000000000046"), "الحديد والصلب", 2, 1, "أسوان", true, 24.08m, 32.89m, "El Hadid & El Solb", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000047"), "مصنع فيرو سيليكون", 0, 1, "أسوان", true, 24.15m, 32.90m, "Firo-Silicon Factory", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000048"), "بحيرة ناصر / إنزال أسوان", 2, 1, "أسوان", true, 24.09m, 32.90m, "Lake Nasser / Aswan Landing", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentRequests_DestinationNileBerthId",
                table: "ShipmentRequests",
                column: "DestinationNileBerthId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentRequests_OriginNileBerthId",
                table: "ShipmentRequests",
                column: "OriginNileBerthId");

            migrationBuilder.CreateIndex(
                name: "IX_CarrierRoutes_DestinationNileBerthId",
                table: "CarrierRoutes",
                column: "DestinationNileBerthId");

            migrationBuilder.CreateIndex(
                name: "IX_CarrierRoutes_OriginNileBerthId",
                table: "CarrierRoutes",
                column: "OriginNileBerthId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarrierRoutes_NileBerths_DestinationNileBerthId",
                table: "CarrierRoutes",
                column: "DestinationNileBerthId",
                principalTable: "NileBerths",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CarrierRoutes_NileBerths_OriginNileBerthId",
                table: "CarrierRoutes",
                column: "OriginNileBerthId",
                principalTable: "NileBerths",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentRequests_NileBerths_DestinationNileBerthId",
                table: "ShipmentRequests",
                column: "DestinationNileBerthId",
                principalTable: "NileBerths",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentRequests_NileBerths_OriginNileBerthId",
                table: "ShipmentRequests",
                column: "OriginNileBerthId",
                principalTable: "NileBerths",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarrierRoutes_NileBerths_DestinationNileBerthId",
                table: "CarrierRoutes");

            migrationBuilder.DropForeignKey(
                name: "FK_CarrierRoutes_NileBerths_OriginNileBerthId",
                table: "CarrierRoutes");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentRequests_NileBerths_DestinationNileBerthId",
                table: "ShipmentRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentRequests_NileBerths_OriginNileBerthId",
                table: "ShipmentRequests");

            migrationBuilder.DropTable(
                name: "NileBerths");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentRequests_DestinationNileBerthId",
                table: "ShipmentRequests");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentRequests_OriginNileBerthId",
                table: "ShipmentRequests");

            migrationBuilder.DropIndex(
                name: "IX_CarrierRoutes_DestinationNileBerthId",
                table: "CarrierRoutes");

            migrationBuilder.DropIndex(
                name: "IX_CarrierRoutes_OriginNileBerthId",
                table: "CarrierRoutes");

            migrationBuilder.DropColumn(
                name: "DestinationBerthId",
                table: "ShipmentRequests");

            migrationBuilder.DropColumn(
                name: "DestinationNileBerthId",
                table: "ShipmentRequests");

            migrationBuilder.DropColumn(
                name: "OriginBerthId",
                table: "ShipmentRequests");

            migrationBuilder.DropColumn(
                name: "OriginNileBerthId",
                table: "ShipmentRequests");

            migrationBuilder.DropColumn(
                name: "DestinationBerthId",
                table: "CarrierRoutes");

            migrationBuilder.DropColumn(
                name: "DestinationNileBerthId",
                table: "CarrierRoutes");

            migrationBuilder.DropColumn(
                name: "OriginBerthId",
                table: "CarrierRoutes");

            migrationBuilder.DropColumn(
                name: "OriginNileBerthId",
                table: "CarrierRoutes");

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "ShipmentRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "ShipmentRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Destination",
                table: "CarrierRoutes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "CarrierRoutes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
