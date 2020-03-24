namespace QuizHut.Data.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class MakePartisipantGroupBaseDeletableModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "ParticipantsGroups",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "ParticipantsGroups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "ParticipantsGroups",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ParticipantsGroups",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "ParticipantsGroups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantsGroups_IsDeleted",
                table: "ParticipantsGroups",
                column: "IsDeleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ParticipantsGroups_IsDeleted",
                table: "ParticipantsGroups");

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
    }
}
