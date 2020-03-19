namespace QuizHut.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class DeleteCompositePrimaryKeyForQuizResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizzesResults",
                table: "QuizzesResults");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "QuizzesResults",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuizId",
                table: "QuizzesResults",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ParticipantId",
                table: "QuizzesResults",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizzesResults",
                table: "QuizzesResults",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_QuizzesResults_ParticipantId",
                table: "QuizzesResults",
                column: "ParticipantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizzesResults",
                table: "QuizzesResults");

            migrationBuilder.DropIndex(
                name: "IX_QuizzesResults_ParticipantId",
                table: "QuizzesResults");

            migrationBuilder.AlterColumn<string>(
                name: "QuizId",
                table: "QuizzesResults",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParticipantId",
                table: "QuizzesResults",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "QuizzesResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizzesResults",
                table: "QuizzesResults",
                columns: new[] { "ParticipantId", "QuizId" });
        }
    }
}
