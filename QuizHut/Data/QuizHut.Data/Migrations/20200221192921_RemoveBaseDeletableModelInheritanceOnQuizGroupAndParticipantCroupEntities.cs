namespace QuizHut.Data.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class RemoveBaseDeletableModelInheritanceOnQuizGroupAndParticipantCroupEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuizzessGroups_IsDeleted",
                table: "QuizzessGroups");

            migrationBuilder.DropIndex(
                name: "IX_ParticipantsGroups_IsDeleted",
                table: "ParticipantsGroups");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "QuizzessGroups");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "QuizzessGroups");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "QuizzessGroups");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "QuizzessGroups");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "QuizzessGroups");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "ParticipantsGroups");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "ParticipantsGroups");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ParticipantsGroups");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ParticipantsGroups");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "ParticipantsGroups");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "QuizzessGroups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "QuizzessGroups",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "QuizzessGroups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "QuizzessGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "QuizzessGroups",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ParticipantsGroups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "ParticipantsGroups",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "ParticipantsGroups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ParticipantsGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "ParticipantsGroups",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizzessGroups_IsDeleted",
                table: "QuizzessGroups",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantsGroups_IsDeleted",
                table: "ParticipantsGroups",
                column: "IsDeleted");
        }
    }
}
