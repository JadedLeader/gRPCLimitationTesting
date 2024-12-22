using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbManagerWorkerService.Migrations
{
    /// <inheritdoc />
    public partial class ApparentlyThereIsAMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionUniqueIdentifier",
                table: "Account");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SessionUniqueIdentifier",
                table: "Account",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
