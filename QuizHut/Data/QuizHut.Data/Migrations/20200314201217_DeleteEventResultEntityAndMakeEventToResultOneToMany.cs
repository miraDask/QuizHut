namespace QuizHut.Data.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class DeleteEventResultEntityAndMakeEventToResultOneToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventsResults");

            migrationBuilder.AddColumn<string>(
                name: "EventId",
                table: "Results",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Results_EventId",
                table: "Results",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Results_Events_EventId",
                table: "Results",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Results_Events_EventId",
                table: "Results");

            migrationBuilder.DropIndex(
                name: "IX_Results_EventId",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Results");

            migrationBuilder.CreateTable(
                name: "EventsResults",
                columns: table => new
                {
                    EventId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ResultId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventsResults", x => new { x.EventId, x.ResultId });
                    table.ForeignKey(
                        name: "FK_EventsResults_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventsResults_Results_ResultId",
                        column: x => x.ResultId,
                        principalTable: "Results",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventsResults_IsDeleted",
                table: "EventsResults",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EventsResults_ResultId",
                table: "EventsResults",
                column: "ResultId");
        }
    }
}
