namespace QuizHut.Data.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddPasswordAsNewEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Quizzes_Password",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Quizzes");

            migrationBuilder.AddColumn<int>(
                name: "PasswordId",
                table: "Quizzes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Password",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    Content = table.Column<string>(maxLength: 16, nullable: false),
                    QuizId = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Password", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_PasswordId",
                table: "Quizzes",
                column: "PasswordId",
                unique: true,
                filter: "[PasswordId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Password_Content",
                table: "Password",
                column: "Content",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Password_PasswordId",
                table: "Quizzes",
                column: "PasswordId",
                principalTable: "Password",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Password_PasswordId",
                table: "Quizzes");

            migrationBuilder.DropTable(
                name: "Password");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_PasswordId",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "PasswordId",
                table: "Quizzes");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Quizzes",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_Password",
                table: "Quizzes",
                column: "Password",
                unique: true);
        }
    }
}
