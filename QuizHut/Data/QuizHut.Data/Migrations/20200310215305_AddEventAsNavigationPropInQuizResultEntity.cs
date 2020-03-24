namespace QuizHut.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddEventAsNavigationPropInQuizResultEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventId",
                table: "QuizzesResults",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizzesResults_EventId",
                table: "QuizzesResults",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizzesResults_Events_EventId",
                table: "QuizzesResults",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizzesResults_Events_EventId",
                table: "QuizzesResults");

            migrationBuilder.DropIndex(
                name: "IX_QuizzesResults_EventId",
                table: "QuizzesResults");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "QuizzesResults");
        }
    }
}
