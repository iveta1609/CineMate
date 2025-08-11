using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineMate.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeatReservationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReservationId",
                table: "Seats",
                type: "int",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Seats_ReservationId",
                table: "Seats",
                column: "ReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Reservations_ReservationId",
                table: "Seats",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Reservations_ReservationId",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Seats_ReservationId",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "ReservationId",
                table: "Seats");

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
    }
}
