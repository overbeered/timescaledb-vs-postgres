using Demo.Database.Contexts.TimescaleDB.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Database.Contexts.TimescaleDB.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:timescaledb", ",,");

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

            migrationBuilder.ConvertToHypertable("timeeventsdata", "timestamp")
                .EnableCompressionOnHypertable("timeeventsdata", "studentid", "timestamp")
                .AddCompressionPolicy("timeeventsdata", 30);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "timeeventsdata");
        }
    }
}
