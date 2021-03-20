using Microsoft.EntityFrameworkCore.Migrations;

namespace QuickSurvey.Infrastructure.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "quicksurvey");

            migrationBuilder.CreateTable(
                name: "sessions",
                schema: "quicksurvey",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Choices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(type: "TEXT", nullable: true),
                    Voters = table.Column<string>(type: "TEXT", nullable: true),
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Choices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Choices_sessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "quicksurvey",
                        principalTable: "sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participants_sessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "quicksurvey",
                        principalTable: "sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Choices_SessionId",
                table: "Choices",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Choices_Text_SessionId",
                table: "Choices",
                columns: new[] { "Text", "SessionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participants_SessionId",
                table: "Participants",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_Username_SessionId",
                table: "Participants",
                columns: new[] { "Username", "SessionId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Choices");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "sessions",
                schema: "quicksurvey");
        }
    }
}
