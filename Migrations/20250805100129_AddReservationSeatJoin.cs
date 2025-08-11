using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineMate.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationSeatJoin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d4444444-dddd-4ddd-dddd-dddddddddddd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5b74b89a-be44-4237-b1ac-181d8e969433", "AQAAAAIAAYagAAAAEDXGGQ/+p6rc9vbxF2qWOHOx2Tw6XEj9KdHWDCutuGc4+WXc4X+D8TArxjustlqH2w==", "c850dea8-06db-4786-86dc-b54d18aca695" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e5555555-eeee-4eee-eeee-eeeeeeeeeee5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d74303f9-9b4e-49f7-9433-d385622141e2", "AQAAAAIAAYagAAAAEMRuCYoOZlU/qiF3gDWOLTbHRDS5STBdD/65qM0l5ikT+CYATcyUJI0O636/2DnejA==", "1cdb9dc2-f446-4cc1-a593-5d3fa48689fe" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f6666666-ffff-4fff-ffff-fffffffffff6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "11641811-1210-4049-bbe8-e26c0477dc63", "AQAAAAIAAYagAAAAEAy8NeiUql+dgsUC7lOzzOOlIzHlwFCoJwyPTLqzETowDHQ1S51nnLunD0UeeA/vFQ==", "1b7ae4f8-a7e3-4ea3-9040-5edbdf81bbaa" });
        }
    }
}
