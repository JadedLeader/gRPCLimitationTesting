using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbManagerWorkerService.Migrations
{
    /// <inheritdoc />
    public partial class ImplementingRequestTypeField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestType",
                table: "CommunicationDelay",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "CommunicationDelay");
        }
    }
}
