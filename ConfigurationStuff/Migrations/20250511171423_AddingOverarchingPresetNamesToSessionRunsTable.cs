using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConfigurationStuff.Migrations
{
    /// <inheritdoc />
    public partial class AddingOverarchingPresetNamesToSessionRunsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OverarchingPresetName",
                table: "SessionRuns",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OverarchingPresetName",
                table: "SessionRuns");
        }
    }
}
