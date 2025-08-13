using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineMate.Migrations
{
    /// <inheritdoc />
    public partial class AddFormatAndAudioToScreening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Audio",
                table: "Screenings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "Screenings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScreeningId = table.Column<int>(type: "int", nullable: false),
                    SeatIdsCsv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Screenings_ScreeningId",
                        column: x => x.ScreeningId,
                        principalTable: "Screenings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d4444444-dddd-4ddd-dddd-dddddddddddd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8300a0d3-0942-415b-a5b5-c44d8bf34ee2", "AQAAAAIAAYagAAAAEOS2Ktiuq/ag/ic32SNO8PWrhGJccdk4ju84zVEKDPwBdGiNuW75/FJJYskv9qD6ug==", "b57afe59-e8f8-410b-bacc-99d38fb6fc00" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e5555555-eeee-4eee-eeee-eeeeeeeeeee5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a990661a-2b7c-405d-8d7d-b09bf3674c5d", "AQAAAAIAAYagAAAAEL1/RkFTUcRV2wpe+zsBFaYwbAS3HvwUQETyqoO0ANepgJVkMSGrzPX2BPJSu/rzrA==", "ff23b8c3-07cb-46cf-aa31-0fd4c6506296" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f6666666-ffff-4fff-ffff-fffffffffff6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "04547713-f1c2-4a53-a323-82f3917f9cbe", "AQAAAAIAAYagAAAAEPXJCPBAQZt/pgF4z16xypKearckNL4ZeNAMy/6nybzYMVDKNhsh12S65cTc+dkdTg==", "649f1969-0455-49c9-badd-f88aed422821" });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ScreeningId",
                table: "CartItems",
                column: "ScreeningId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropColumn(
                name: "Audio",
                table: "Screenings");

            migrationBuilder.DropColumn(
                name: "Format",
                table: "Screenings");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d4444444-dddd-4ddd-dddd-dddddddddddd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fd1a3fa5-e79e-45ea-aad6-f2e92bd81af1", "AQAAAAIAAYagAAAAELzAKYmao62Yt6+Hd2PAvO9nRcKLoElp7ZhUB9Wv4LKiPs2M05pWE1a3vxc23SUnlQ==", "50c7b79a-fab3-4120-8df4-829fb3833c59" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e5555555-eeee-4eee-eeee-eeeeeeeeeee5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c8030ffd-198d-4a2d-ac6f-e04b747dfe0d", "AQAAAAIAAYagAAAAEAPuv9VFYaITiaxY8B2/rrk3k8DU+VCA0ZqhFjRzuUYRX5ZMBlS04lIg3A6wdi5tCQ==", "38f859d1-31e8-4a79-915c-458f5cffc94c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f6666666-ffff-4fff-ffff-fffffffffff6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "63a7bb52-f5c7-49f8-a98f-13520729d54a", "AQAAAAIAAYagAAAAEMALr+PSrpZXIYn/fx8llMfs6lh9nEIkq0uA1VRgVoa4j9RJEe30dFzgtTkigTiP+w==", "46660f03-d2a4-4954-a98c-69de4715c701" });
        }
    }
}
