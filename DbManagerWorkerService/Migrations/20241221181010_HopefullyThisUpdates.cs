using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbManagerWorkerService.Migrations
{
    /// <inheritdoc />
    public partial class HopefullyThisUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nonsense",
                table: "DelayCalc",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nonsense",
                table: "DelayCalc");
        }
    }
}
