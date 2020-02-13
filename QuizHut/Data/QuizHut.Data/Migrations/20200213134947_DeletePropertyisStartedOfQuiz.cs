using Microsoft.EntityFrameworkCore.Migrations;

namespace QuizHut.Data.Migrations
{
    public partial class DeletePropertyisStartedOfQuiz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStarted",
                table: "Quizzes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStarted",
                table: "Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }
    }
}
