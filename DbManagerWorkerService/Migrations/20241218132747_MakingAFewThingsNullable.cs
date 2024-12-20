using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbManagerWorkerService.Migrations
{
    /// <inheritdoc />
    public partial class MakingAFewThingsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_AuthToken_AuthTokenAuthUnique",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Account_Session_SessionUnique",
                table: "Account");

            migrationBuilder.AlterColumn<string>(
                name: "TimeOfLogin",
                table: "Account",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionUnique",
                table: "Account",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "AuthUnique",
                table: "Account",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "AuthTokenAuthUnique",
                table: "Account",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_AuthToken_AuthTokenAuthUnique",
                table: "Account",
                column: "AuthTokenAuthUnique",
                principalTable: "AuthToken",
                principalColumn: "AuthUnique");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Session_SessionUnique",
                table: "Account",
                column: "SessionUnique",
                principalTable: "Session",
                principalColumn: "SessionUnique");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_AuthToken_AuthTokenAuthUnique",
                table: "Account");

            migrationBuilder.DropForeignKey(
                name: "FK_Account_Session_SessionUnique",
                table: "Account");

            migrationBuilder.AlterColumn<string>(
                name: "TimeOfLogin",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionUnique",
                table: "Account",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AuthUnique",
                table: "Account",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AuthTokenAuthUnique",
                table: "Account",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Account_AuthToken_AuthTokenAuthUnique",
                table: "Account",
                column: "AuthTokenAuthUnique",
                principalTable: "AuthToken",
                principalColumn: "AuthUnique",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Session_SessionUnique",
                table: "Account",
                column: "SessionUnique",
                principalTable: "Session",
                principalColumn: "SessionUnique",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
