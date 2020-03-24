namespace QuizHut.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class ChangeTableNameInDbContextFromParticipantsGroupsToStudentsGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantsGroups_Groups_GroupId",
                table: "ParticipantsGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantsGroups_AspNetUsers_StudentId",
                table: "ParticipantsGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParticipantsGroups",
                table: "ParticipantsGroups");

            migrationBuilder.RenameTable(
                name: "ParticipantsGroups",
                newName: "StudentsGroups");

            migrationBuilder.RenameIndex(
                name: "IX_ParticipantsGroups_GroupId",
                table: "StudentsGroups",
                newName: "IX_StudentsGroups_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentsGroups",
                table: "StudentsGroups",
                columns: new[] { "StudentId", "GroupId" });

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsGroups_Groups_GroupId",
                table: "StudentsGroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentsGroups_AspNetUsers_StudentId",
                table: "StudentsGroups",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentsGroups_Groups_GroupId",
                table: "StudentsGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentsGroups_AspNetUsers_StudentId",
                table: "StudentsGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentsGroups",
                table: "StudentsGroups");

            migrationBuilder.RenameTable(
                name: "StudentsGroups",
                newName: "ParticipantsGroups");

            migrationBuilder.RenameIndex(
                name: "IX_StudentsGroups_GroupId",
                table: "ParticipantsGroups",
                newName: "IX_ParticipantsGroups_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParticipantsGroups",
                table: "ParticipantsGroups",
                columns: new[] { "StudentId", "GroupId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantsGroups_Groups_GroupId",
                table: "ParticipantsGroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantsGroups_AspNetUsers_StudentId",
                table: "ParticipantsGroups",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
