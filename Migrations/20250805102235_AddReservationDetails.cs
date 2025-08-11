using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineMate.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Seats",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Seats");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d4444444-dddd-4ddd-dddd-dddddddddddd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "684f8779-fde4-43ae-a1f4-621b52cb6212", "AQAAAAIAAYagAAAAED7Utow/Sh9aV/ncfMvpI320MGS4C9CplvAj1wVyGOBOTSYEGw5C8WJYVBge3SHqeQ==", "0ad64132-5edb-4405-aef5-ac3068e0533b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e5555555-eeee-4eee-eeee-eeeeeeeeeee5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1c5380a2-08c4-4f2f-9b9d-24cc2acc736c", "AQAAAAIAAYagAAAAEBtf/gwPfak+umEvIyen7cJvZSPJCUlylMlnpPNp/j/xPFIAL9XMus7z6D2tosYQ0Q==", "b40e5341-326b-415a-a614-09231b97728c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f6666666-ffff-4fff-ffff-fffffffffff6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5e3192d3-94c1-48f0-a947-809fa2c1da01", "AQAAAAIAAYagAAAAEAD/b01rWzSLBdjj/0n8oIexuU2IishpITIh8wgrbzb2QwYgCEooaALjQDjg+iTrnQ==", "fa557a1d-b47f-40ff-95aa-8861f355575d" });
        }
    }
}
