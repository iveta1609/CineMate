using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineMate.Migrations
{
    /// <inheritdoc />
    public partial class Reservation_AddUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Reservations",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UserId",
                table: "Reservations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AspNetUsers_UserId",
                table: "Reservations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AspNetUsers_UserId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_UserId",
                table: "Reservations");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d4444444-dddd-4ddd-dddd-dddddddddddd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "76f6d359-7016-4758-b61b-220d7a62b607", "AQAAAAIAAYagAAAAEGVVNy5pKVhFW3xihhixoLGrMNNZW+XFu1q4rUCu37Bdsu4mClmezirAnxfGIGCyMQ==", "c61af4b4-f717-4359-a0b5-8e83e0d18f38" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e5555555-eeee-4eee-eeee-eeeeeeeeeee5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b0911bca-be2e-406d-a5f0-d160fe2ef369", "AQAAAAIAAYagAAAAEBgpMFF8t8LIeOEC7UVnyHXGcOSiqZ0qQ7GdjGBgQUbnxAZIgxhZ2NG3iDVwxNGi5A==", "9afaa19c-04ac-47e0-a8c6-3becf20a634a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f6666666-ffff-4fff-ffff-fffffffffff6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b0476020-3449-4f79-a139-1ef45cce9868", "AQAAAAIAAYagAAAAEKkd04uaNGa5er014Ll1L57KIIQhKfHnAH0+qsygGXTdZfkd/YuUwe4wyqf6maWd+w==", "9e21a976-bedd-408b-a520-72f798d5eacd" });
        }
    }
}
