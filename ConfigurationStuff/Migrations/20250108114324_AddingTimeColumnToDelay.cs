using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConfigurationStuff.Migrations
{
    /// <inheritdoc />
    public partial class AddingTimeColumnToDelay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DataIterations",
                table: "DelayCalc",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecordCreation",
                table: "DelayCalc",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordCreation",
                table: "DelayCalc");

            migrationBuilder.AlterColumn<int>(
                name: "DataIterations",
                table: "DelayCalc",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
