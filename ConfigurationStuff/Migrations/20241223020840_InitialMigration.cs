using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConfigurationStuff.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthToken",
                columns: table => new
                {
                    AuthUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthToken", x => x.AuthUnique);
                });

            migrationBuilder.CreateTable(
                name: "Session",
                columns: table => new
                {
                    SessionUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SessionCreated = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.SessionUnique);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeOfLogin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeOfAccountCreation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthTokenAuthUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SessionUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountUnique);
                    table.ForeignKey(
                        name: "FK_Account_AuthToken_AuthTokenAuthUnique",
                        column: x => x.AuthTokenAuthUnique,
                        principalTable: "AuthToken",
                        principalColumn: "AuthUnique");
                    table.ForeignKey(
                        name: "FK_Account_Session_SessionUnique",
                        column: x => x.SessionUnique,
                        principalTable: "Session",
                        principalColumn: "SessionUnique");
                });

            migrationBuilder.CreateTable(
                name: "ClientInstance",
                columns: table => new
                {
                    ClientUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientInstance", x => x.ClientUnique);
                    table.ForeignKey(
                        name: "FK_ClientInstance_Session_SessionUnique",
                        column: x => x.SessionUnique,
                        principalTable: "Session",
                        principalColumn: "SessionUnique",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DelayCalc",
                columns: table => new
                {
                    messageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommunicationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataIterations = table.Column<int>(type: "int", nullable: true),
                    DataContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Delay = table.Column<TimeSpan>(type: "time", nullable: true),
                    ClientInstanceClientUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DelayCalc", x => x.messageId);
                    table.ForeignKey(
                        name: "FK_DelayCalc_ClientInstance_ClientInstanceClientUnique",
                        column: x => x.ClientInstanceClientUnique,
                        principalTable: "ClientInstance",
                        principalColumn: "ClientUnique");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_AuthTokenAuthUnique",
                table: "Account",
                column: "AuthTokenAuthUnique");

            migrationBuilder.CreateIndex(
                name: "IX_Account_SessionUnique",
                table: "Account",
                column: "SessionUnique");

            migrationBuilder.CreateIndex(
                name: "IX_ClientInstance_SessionUnique",
                table: "ClientInstance",
                column: "SessionUnique");

            migrationBuilder.CreateIndex(
                name: "IX_DelayCalc_ClientInstanceClientUnique",
                table: "DelayCalc",
                column: "ClientInstanceClientUnique");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "DelayCalc");

            migrationBuilder.DropTable(
                name: "AuthToken");

            migrationBuilder.DropTable(
                name: "ClientInstance");

            migrationBuilder.DropTable(
                name: "Session");
        }
    }
}
