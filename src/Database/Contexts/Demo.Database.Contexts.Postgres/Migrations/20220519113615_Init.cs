using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Database.Contexts.Postgres.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "timeeventsdata",
                columns: table => new
                {
                    studentid = table.Column<Guid>(type: "uuid", nullable: false),
                    eventid = table.Column<Guid>(type: "uuid", nullable: false),
                    timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    payload = table.Column<string>(type: "json", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateIndex(
                name: "IX_timeeventsdata_eventid",
                table: "timeeventsdata",
                column: "eventid");

            migrationBuilder.CreateIndex(
                name: "IX_timeeventsdata_timestamp_studentid",
                table: "timeeventsdata",
                columns: new[] { "timestamp", "studentid" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "timeeventsdata");
        }
    }
}
