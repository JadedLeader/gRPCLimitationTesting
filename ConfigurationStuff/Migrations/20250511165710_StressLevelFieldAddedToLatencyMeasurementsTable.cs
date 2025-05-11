using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConfigurationStuff.Migrations
{
    /// <inheritdoc />
    public partial class StressLevelFieldAddedToLatencyMeasurementsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StressLevel",
                table: "LatencyMeasurements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StressLevel",
                table: "LatencyMeasurements");
        }
    }
}
