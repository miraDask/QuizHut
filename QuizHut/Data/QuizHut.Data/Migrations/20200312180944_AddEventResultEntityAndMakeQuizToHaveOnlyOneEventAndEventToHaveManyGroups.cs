namespace QuizHut.Data.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddEventResultEntityAndMakeQuizToHaveOnlyOneEventAndEventToHaveManyGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Groups_GroupId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Quizzes_QuizId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "QuizzesResults");

            migrationBuilder.DropIndex(
                name: "IX_Events_GroupId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_QuizId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "EventId",
                table: "Quizzes",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuizId",
                table: "Events",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "EventsGroups",
                columns: table => new
                {
                    EventId = table.Column<string>(nullable: false),
                    GroupId = table.Column<string>(nullable: false),
                    Id = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedOn = table.Column<DateTime>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventsGroups", x => new { x.EventId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_EventsGroups_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventsGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventsResults",
                columns: table => new
                {
                    ResultId = table.Column<string>(nullable: false),
                    EventId = table.Column<string>(nullable: false),
                    Id = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedOn = table.Column<DateTime>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventsResults", x => new { x.EventId, x.ResultId });
                    table.ForeignKey(
                        name: "FK_EventsResults_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventsResults_Results_ResultId",
                        column: x => x.ResultId,
                        principalTable: "Results",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_EventId",
                table: "Quizzes",
                column: "EventId",
                unique: true,
                filter: "[EventId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EventsGroups_GroupId",
                table: "EventsGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_EventsGroups_IsDeleted",
                table: "EventsGroups",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EventsResults_IsDeleted",
                table: "EventsResults",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EventsResults_ResultId",
                table: "EventsResults",
                column: "ResultId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Events_EventId",
                table: "Quizzes",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Events_EventId",
                table: "Quizzes");

            migrationBuilder.DropTable(
                name: "EventsGroups");

            migrationBuilder.DropTable(
                name: "EventsResults");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_EventId",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Quizzes");

            migrationBuilder.AlterColumn<string>(
                name: "QuizId",
                table: "Events",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupId",
                table: "Events",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuizzesResults",
                columns: table => new
                {
                    QuizId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ResultId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EventId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizzesResults", x => new { x.QuizId, x.ResultId });
                    table.ForeignKey(
                        name: "FK_QuizzesResults_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizzesResults_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizzesResults_Results_ResultId",
                        column: x => x.ResultId,
                        principalTable: "Results",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_GroupId",
                table: "Events",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_QuizId",
                table: "Events",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizzesResults_EventId",
                table: "QuizzesResults",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizzesResults_IsDeleted",
                table: "QuizzesResults",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_QuizzesResults_ResultId",
                table: "QuizzesResults",
                column: "ResultId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Groups_GroupId",
                table: "Events",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Quizzes_QuizId",
                table: "Events",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
