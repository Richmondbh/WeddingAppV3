using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WeddingAppV3.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Date = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Time = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    WillAttend = table.Column<bool>(type: "INTEGER", nullable: true),
                    NumberOfAttendants = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    RegisteredOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeddingInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Heading = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeddingInfos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "Date", "Description", "Location", "Time", "Title" },
                values: new object[,]
                {
                    { 1, "July 14, 2027", "The exchange of vows. Please be seated by 16:45.", "Hope Wedding Hall, Östersund", "17:00", "Wedding Ceremony" },
                    { 2, "July 14, 2027", "A three-course dinner with speeches from family and friends.", "Hope Wedding Hall, Östersund", "19:00", "Reception Dinner" },
                    { 3, "July 14, 2027", "Music and dancing until late.", "Hope Wedding Hall, Östersund", "21:30", "Wedding Party" }
                });

            migrationBuilder.InsertData(
                table: "Guests",
                columns: new[] { "Id", "Email", "Message", "Name", "NumberOfAttendants", "PhoneNumber", "RegisteredOn", "WillAttend" },
                values: new object[,]
                {
                    { 1, "anna@rich.com", "We would not miss it for the world!", "Anna Lindberg", 2, null, new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true },
                    { 2, "johan@rich.com", "Sadly I am abroad that week. Congratulations!", "Johan Berg", 0, null, new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false },
                    { 3, "ama.mensah@rich.com", null, "Ama Mensah", 0, null, new DateTime(2026, 10, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null }
                });

            migrationBuilder.InsertData(
                table: "WeddingInfos",
                columns: new[] { "Id", "Content", "DisplayOrder", "Heading" },
                values: new object[,]
                {
                    { 1, "Richmond and Keziah met in Östersund seven years ago and have been more or less inseparable ever since. We would love for you to celebrate the day with us.", 1, "Welcome" },
                    { 2, "Everything takes place at Hope Wedding Hall in Östersund. There is free parking next to the building.", 2, "The venue" },
                    { 3, "Questions? Email hej@richkez.se or call +46 76 931 68 71.", 3, "Contact" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.DropTable(
                name: "WeddingInfos");
        }
    }
}
