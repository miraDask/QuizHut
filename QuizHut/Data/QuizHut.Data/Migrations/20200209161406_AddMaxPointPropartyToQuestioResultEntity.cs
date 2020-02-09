namespace QuizHut.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddMaxPointPropartyToQuestioResultEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxPoints",
                table: "QuizzesResults",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxPoints",
                table: "QuizzesResults");
        }
    }
}
