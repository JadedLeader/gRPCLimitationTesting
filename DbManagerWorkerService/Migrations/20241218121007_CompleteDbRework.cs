using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbManagerWorkerService.Migrations
{
    /// <inheritdoc />
    public partial class CompleteDbRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommunicationDelay");

            migrationBuilder.CreateTable(
                name: "AuthToken",
                columns: table => new
                {
                    AuthUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthToken", x => x.AuthUnique);
                });

            migrationBuilder.CreateTable(
                name: "ClientInstance",
                columns: table => new
                {
                    ClientUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientInstance", x => x.ClientUnique);
                });

            migrationBuilder.CreateTable(
                name: "DelayCalc",
                columns: table => new
                {
                    messageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommunicationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataIterations = table.Column<int>(type: "int", nullable: false),
                    DataContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Delay = table.Column<TimeSpan>(type: "time", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Session",
                columns: table => new
                {
                    SessionUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientInstanceClientUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.SessionUnique);
                    table.ForeignKey(
                        name: "FK_Session_ClientInstance_ClientInstanceClientUnique",
                        column: x => x.ClientInstanceClientUnique,
                        principalTable: "ClientInstance",
                        principalColumn: "ClientUnique",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeOfLogin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeOfAccountCreation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthTokenAuthUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionUnique = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountUnique);
                    table.ForeignKey(
                        name: "FK_Account_AuthToken_AuthTokenAuthUnique",
                        column: x => x.AuthTokenAuthUnique,
                        principalTable: "AuthToken",
                        principalColumn: "AuthUnique",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Account_Session_SessionUnique",
                        column: x => x.SessionUnique,
                        principalTable: "Session",
                        principalColumn: "SessionUnique",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_DelayCalc_ClientInstanceClientUnique",
                table: "DelayCalc",
                column: "ClientInstanceClientUnique");

            migrationBuilder.CreateIndex(
                name: "IX_Session_ClientInstanceClientUnique",
                table: "Session",
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
                name: "Session");

            migrationBuilder.DropTable(
                name: "ClientInstance");

            migrationBuilder.CreateTable(
                name: "CommunicationDelay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommunicationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataContents = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataIterations = table.Column<int>(type: "int", nullable: false),
                    Delay = table.Column<TimeSpan>(type: "time", nullable: false),
                    MessageDelayId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunicationDelay", x => x.Id);
                });
        }
    }
}
