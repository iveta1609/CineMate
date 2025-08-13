using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineMate.Migrations
{
    /// <inheritdoc />
    public partial class CartItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
