using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConfigurationStuff.Migrations
{
    /// <inheritdoc />
    public partial class AddedClientTypeFieldToLatencyMeasurements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientType",
                table: "LatencyMeasurements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientType",
                table: "LatencyMeasurements");
        }
    }
}
