using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConfigurationStuff.Migrations
{
    /// <inheritdoc />
    public partial class TablesLatencyMeasurementsAndSessionsRunsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SessionRuns",
                columns: table => new
                {
                    SessionsRunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PresetName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionRuns", x => x.SessionsRunId);
                });

            migrationBuilder.CreateTable(
                name: "LatencyMeasurements",
                columns: table => new
                {
                    MeasurementUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SessionRunsSessionsRunId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LatencyMeasurements", x => x.MeasurementUnique);
                    table.ForeignKey(
                        name: "FK_LatencyMeasurements_SessionRuns_SessionRunsSessionsRunId",
                        column: x => x.SessionRunsSessionsRunId,
                        principalTable: "SessionRuns",
                        principalColumn: "SessionsRunId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LatencyMeasurements_SessionRunsSessionsRunId",
                table: "LatencyMeasurements",
                column: "SessionRunsSessionsRunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LatencyMeasurements");

            migrationBuilder.DropTable(
                name: "SessionRuns");
        }
    }
}
