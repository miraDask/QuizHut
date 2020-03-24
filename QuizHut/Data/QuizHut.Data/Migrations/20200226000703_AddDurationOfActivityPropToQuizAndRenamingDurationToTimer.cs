namespace QuizHut.Data.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddDurationOfActivityPropToQuizAndRenamingDurationToTimer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Quizzes");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DurationOfActivity",
                table: "Quizzes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Timer",
                table: "Quizzes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationOfActivity",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Timer",
                table: "Quizzes");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Quizzes",
                type: "int",
                nullable: true);
        }
    }
}
