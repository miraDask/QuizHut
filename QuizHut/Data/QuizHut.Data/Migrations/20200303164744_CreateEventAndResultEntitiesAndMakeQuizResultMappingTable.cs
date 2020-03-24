namespace QuizHut.Data.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class CreateEventAndResultEntitiesAndMakeQuizResultMappingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ManagerId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantsGroups_AspNetUsers_ParticipantId",
                table: "ParticipantsGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizzesResults_AspNetUsers_ParticipantId",
                table: "QuizzesResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizzesResults",
                table: "QuizzesResults");

            migrationBuilder.DropIndex(
                name: "IX_QuizzesResults_ParticipantId",
                table: "QuizzesResults");

            migrationBuilder.DropIndex(
                name: "IX_QuizzesResults_QuizId",
                table: "QuizzesResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParticipantsGroups",
                table: "ParticipantsGroups");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ManagerId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MaxPoints",
                table: "QuizzesResults");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "QuizzesResults");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "QuizzesResults");

            migrationBuilder.DropColumn(
                name: "ActivationDateAndTime",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "DurationOfActivity",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "ParticipantsGroups");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "QuizId",
                table: "QuizzesResults",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "QuizzesResults",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ResultId",
                table: "QuizzesResults",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Quizzes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StudentId",
                table: "ParticipantsGroups",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "TeacherId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizzesResults",
                table: "QuizzesResults",
                columns: new[] { "QuizId", "ResultId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParticipantsGroups",
                table: "ParticipantsGroups",
                columns: new[] { "StudentId", "GroupId" });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedOn = table.Column<DateTime>(nullable: true),
                    ActivationDateAndTime = table.Column<DateTime>(nullable: true),
                    DurationOfActivity = table.Column<TimeSpan>(nullable: true),
                    GroupId = table.Column<string>(nullable: true),
                    QuizId = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedOn = table.Column<DateTime>(nullable: true),
                    Points = table.Column<int>(nullable: false),
                    MaxPoints = table.Column<int>(nullable: false),
                    StudentId = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Results_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizzesResults_ResultId",
                table: "QuizzesResults",
                column: "ResultId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TeacherId",
                table: "AspNetUsers",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_GroupId",
                table: "Events",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_IsDeleted",
                table: "Events",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Events_QuizId",
                table: "Events",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Results_IsDeleted",
                table: "Results",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Results_StudentId",
                table: "Results",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_TeacherId",
                table: "AspNetUsers",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantsGroups_AspNetUsers_StudentId",
                table: "ParticipantsGroups",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizzesResults_Results_ResultId",
                table: "QuizzesResults",
                column: "ResultId",
                principalTable: "Results",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_TeacherId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantsGroups_AspNetUsers_StudentId",
                table: "ParticipantsGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizzesResults_Results_ResultId",
                table: "QuizzesResults");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizzesResults",
                table: "QuizzesResults");

            migrationBuilder.DropIndex(
                name: "IX_QuizzesResults_ResultId",
                table: "QuizzesResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParticipantsGroups",
                table: "ParticipantsGroups");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TeacherId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ResultId",
                table: "QuizzesResults");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "ParticipantsGroups");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "QuizzesResults",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuizId",
                table: "QuizzesResults",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "MaxPoints",
                table: "QuizzesResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ParticipantId",
                table: "QuizzesResults",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "QuizzesResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivationDateAndTime",
                table: "Quizzes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DurationOfActivity",
                table: "Quizzes",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParticipantId",
                table: "ParticipantsGroups",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizzesResults",
                table: "QuizzesResults",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParticipantsGroups",
                table: "ParticipantsGroups",
                columns: new[] { "ParticipantId", "GroupId" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizzesResults_ParticipantId",
                table: "QuizzesResults",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizzesResults_QuizId",
                table: "QuizzesResults",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ManagerId",
                table: "AspNetUsers",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ManagerId",
                table: "AspNetUsers",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantsGroups_AspNetUsers_ParticipantId",
                table: "ParticipantsGroups",
                column: "ParticipantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizzesResults_AspNetUsers_ParticipantId",
                table: "QuizzesResults",
                column: "ParticipantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
