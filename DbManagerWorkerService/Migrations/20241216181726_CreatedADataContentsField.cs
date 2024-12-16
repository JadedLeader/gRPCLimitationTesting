using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbManagerWorkerService.Migrations
{
    /// <inheritdoc />
    public partial class CreatedADataContentsField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataLength",
                table: "CommunicationDelay",
                newName: "DataIterations");

            migrationBuilder.AddColumn<string>(
                name: "DataContents",
                table: "CommunicationDelay",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataContents",
                table: "CommunicationDelay");

            migrationBuilder.RenameColumn(
                name: "DataIterations",
                table: "CommunicationDelay",
                newName: "DataLength");
        }
    }
}
