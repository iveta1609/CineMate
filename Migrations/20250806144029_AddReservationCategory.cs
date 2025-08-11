using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineMate.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d4444444-dddd-4ddd-dddd-dddddddddddd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3edd5682-72d0-4e2d-8caf-6a8147422726", "AQAAAAIAAYagAAAAENb4d9kFBtsiB84LwHGzygVCCllkLglt82SDp/U1xbnpvrty2dYfBq1ypISLH3FT2w==", "47858726-eb71-40e7-b299-e8493a5d21cb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e5555555-eeee-4eee-eeee-eeeeeeeeeee5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "98704d7f-9193-4de3-ac4a-f7e626f748f9", "AQAAAAIAAYagAAAAECTzW4GptKZxqAWxknhefw2HDy1u0XoMT2M7kp3WcxUajnluQ28REbBdknEhQQZo+g==", "2d7022c8-b63e-408d-a587-6d2030bb9096" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f6666666-ffff-4fff-ffff-fffffffffff6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5dd9f8fc-9908-4272-ae08-535817792312", "AQAAAAIAAYagAAAAEDYcRg35FC1Mx6VBQnol7f3pJ/LG4S4dVzte5TBsmX4H5vC6lEudDmzC2QmG2tY26w==", "25ebfeb2-b6f5-4666-a503-a3223fe2bcd2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Reservations");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d4444444-dddd-4ddd-dddd-dddddddddddd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e50d6334-7776-4700-9aa0-3c78c081976b", "AQAAAAIAAYagAAAAEFKmKTFAfkRLV3whhEYm8WxeroTBzlPaaHCojG6udJ1c4NKLrCFzmcHRfwPmx0DkhQ==", "22fd53a5-0cab-4607-a6d3-adf9aa57f196" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e5555555-eeee-4eee-eeee-eeeeeeeeeee5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4d663758-0444-4a46-956f-4d5f2092b844", "AQAAAAIAAYagAAAAEPI+qVYHZX3YvtIwVT7QwTj3UP2hPDewePSd7NU8sPPwzin5okjrm43EYIDA5zuDBA==", "0e9504cd-03d5-4d38-b5b5-99f2eba53fc0" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f6666666-ffff-4fff-ffff-fffffffffff6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "41355451-6859-4019-96b1-f4733bfeb6fb", "AQAAAAIAAYagAAAAEPGAyZ4IsOt4lCWI1HWlFP6aqIk2ih9RxVY61wYi/PplIFAc0r9b9PkfyHIMv8TYBQ==", "4f42c64f-280c-41fe-ac5c-9e7472e7f17d" });
        }
    }
}
