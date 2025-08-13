using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineMate.Migrations
{
    /// <inheritdoc />
    public partial class AddMoviePosterUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PosterUrl",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);

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

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 1,
                column: "PosterUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 2,
                column: "PosterUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 3,
                column: "PosterUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 4,
                column: "PosterUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 5,
                column: "PosterUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 6,
                column: "PosterUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 7,
                column: "PosterUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 8,
                column: "PosterUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 9,
                column: "PosterUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 10,
                column: "PosterUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 11,
                column: "PosterUrl",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PosterUrl",
                table: "Movies");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d4444444-dddd-4ddd-dddd-dddddddddddd",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d05a4601-41fb-47a0-a191-f5f044cbdde1", "AQAAAAIAAYagAAAAELB8XQbq9GRrDm2esFPacf/oPC1s40oL6h27Lr8eDEGNss1oNmKVgE4PEHoNuYKNwA==", "c715f65a-40ad-4020-8a68-0eb5f10a559f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e5555555-eeee-4eee-eeee-eeeeeeeeeee5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8e17f378-790e-4f27-83bc-f5060ab40bd9", "AQAAAAIAAYagAAAAEOJWCj4HigB81NKOZbjuChLcrxOHzlnOV/soAEAD69RePppEoxQAn+yVZYoAdoLh5g==", "9f3f6132-88de-4070-bfaa-722675d65f2e" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f6666666-ffff-4fff-ffff-fffffffffff6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "92f9c62c-d112-4a88-917c-63113b54b9e2", "AQAAAAIAAYagAAAAEH9i9lt7HNluyB2eRJseaagTuuEQRY0hvgT+JXDuIGeHYFvnbIy5XGCMMvDQ0a2mWQ==", "68498975-9da3-41d5-ae5f-6fcc8cd6406e" });
        }
    }
}
