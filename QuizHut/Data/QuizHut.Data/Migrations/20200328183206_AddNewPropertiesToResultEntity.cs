namespace QuizHut.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddNewPropertiesToResultEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventName",
                table: "Results",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuizName",
                table: "Results",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventName",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "QuizName",
                table: "Results");
        }
    }
}
