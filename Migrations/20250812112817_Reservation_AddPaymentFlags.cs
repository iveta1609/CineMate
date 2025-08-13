using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineMate.Migrations
{
    /// <inheritdoc />
    public partial class Reservation_AddPaymentFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Reservations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Reservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentRef",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "PaymentRef",
                table: "Reservations");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d4444444-dddd-4ddd-dddd-dddddddddddd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "38c92568-c62e-4fa6-ba92-0e056605a35f", "AQAAAAIAAYagAAAAEBIpoyEV8da1Fvrr0wDYK422qUJBMTpxCnEXQTDvW6qDm2mM+tTLuVKnQzAQ4n8U3w==", "c66d1213-93b0-4fe0-a81c-f7315ef474f3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e5555555-eeee-4eee-eeee-eeeeeeeeeee5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c9bb63f7-dd5a-47cd-9dca-99c425069553", "AQAAAAIAAYagAAAAEOl0aLk3QkdapV5Clo6mf4GfYB1klk7J8joq/TFA21/zJVEDFEdt8TIjy2J1po4DFQ==", "f12be423-6e16-4c15-ad35-154f4d2d7bb6" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f6666666-ffff-4fff-ffff-fffffffffff6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "12a7725c-69f1-4e65-97b6-fac75603813c", "AQAAAAIAAYagAAAAEBl65jnp075pauyS1OtTnG1/ThMheu98Nr8nZ+Bx+gXrUFzrDuJ0CBraXl/sLnHk5A==", "e9c31c37-13ba-4f2e-be36-8336ede99124" });
        }
    }
}
