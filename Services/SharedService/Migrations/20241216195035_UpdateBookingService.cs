using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Listings",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "CheckOut",
                table: "Bookings",
                newName: "To");

            migrationBuilder.RenameColumn(
                name: "CheckIn",
                table: "Bookings",
                newName: "From");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Listings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Listings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NoOfPeople",
                table: "Listings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GuestNames",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "NoOfPeople",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "GuestNames",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Listings",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "To",
                table: "Bookings",
                newName: "CheckOut");

            migrationBuilder.RenameColumn(
                name: "From",
                table: "Bookings",
                newName: "CheckIn");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
