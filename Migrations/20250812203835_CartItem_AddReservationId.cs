using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineMate.Migrations
{
    /// <inheritdoc />
    public partial class CartItem_AddReservationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReservationId",
                table: "CartItems",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d4444444-dddd-4ddd-dddd-dddddddddddd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "113b3696-1c7d-4f2d-a965-cc3f5b7a302f", "AQAAAAIAAYagAAAAEN/2Bkd8UvriTOD4l+baJedfNsFfEXdAgCMI2iOh0C8QutNKC1AG8uIxKY3Ghu2Cmg==", "6bad73a6-82f2-4601-80e6-82cc778cdb71" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e5555555-eeee-4eee-eeee-eeeeeeeeeee5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8bc34e78-4ee0-4916-8aeb-a29166aea4cf", "AQAAAAIAAYagAAAAENnUss0yFm/TMjnsrqCrM+XXbaRWuUGP3qeJF1G9daRz/iR89vteU8/lyhs4FWfL6w==", "b1770f4d-4333-467c-bbd0-1d2577fa7c48" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f6666666-ffff-4fff-ffff-fffffffffff6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d096cea5-3cf0-4795-a2af-0597bf355277", "AQAAAAIAAYagAAAAELIOKaEii3eZmvyqdODQAxzHXGNyjKbtNxbHnLOiKdVPomjwoC7SBWWI8K6jIUdiiA==", "9611aa0f-fd13-416a-91e1-49a6a5c4e934" });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ReservationId",
                table: "CartItems",
                column: "ReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Reservations_ReservationId",
                table: "CartItems",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Reservations_ReservationId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ReservationId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ReservationId",
                table: "CartItems");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d4444444-dddd-4ddd-dddd-dddddddddddd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5f14bec8-abe7-4d85-8a1a-edd35cf3d2f5", "AQAAAAIAAYagAAAAEFpp972TA0Z4Gx/dd4y0FM5HtpSkSnSeQjhESyXRsoV3N1ZZCYGs+HCltdpORBM0sQ==", "dc9a4621-4e69-4bba-984f-261afa5d6840" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e5555555-eeee-4eee-eeee-eeeeeeeeeee5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "68bbddae-56c6-4a6a-bade-699d1ed0fefa", "AQAAAAIAAYagAAAAEOX2wscPAN++EufVMFlLzhceg4ZrXuMnUWXSy80eY+HdEoTTt4zU8uajaA4FLBZYww==", "3eeddd37-7c85-411e-9b99-f2ee33b8a154" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f6666666-ffff-4fff-ffff-fffffffffff6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "45aaefc7-fa7c-48b3-88eb-9758698e68a4", "AQAAAAIAAYagAAAAECEHXU2jrlGJzSsszcoWjvKNxfx90eUhEKTlJSU/+7buI8SR8DIMbtyjLf9S/BrDQQ==", "0fbec615-3123-4ce4-8155-a0a8d6758f7d" });
        }
    }
}
