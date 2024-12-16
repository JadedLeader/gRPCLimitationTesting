using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbManagerWorkerService.Migrations
{
    /// <inheritdoc />
    public partial class CreatedClientIdField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DelayGuid",
                table: "CommunicationDelay",
                newName: "MessageDelayId");

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "CommunicationDelay",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "CommunicationDelay");

            migrationBuilder.RenameColumn(
                name: "MessageDelayId",
                table: "CommunicationDelay",
                newName: "DelayGuid");
        }
    }
}
