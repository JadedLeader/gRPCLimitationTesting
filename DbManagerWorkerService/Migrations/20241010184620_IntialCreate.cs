using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbManagerWorkerService.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommunicationDelay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DelayGuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommunicationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataLength = table.Column<int>(type: "int", nullable: false),
                    Delay = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunicationDelay", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommunicationDelay");
        }
    }
}
