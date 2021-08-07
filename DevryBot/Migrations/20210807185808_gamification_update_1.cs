using Microsoft.EntityFrameworkCore.Migrations;

namespace DevryBot.Migrations
{
    public partial class gamification_update_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "Reasoning",
                table: "ChallengeResponses");

            migrationBuilder.RenameColumn(
                name: "LastTimeUsed",
                table: "Challenges",
                newName: "Explanation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Explanation",
                table: "Challenges",
                newName: "LastTimeUsed");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Challenges",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Reasoning",
                table: "ChallengeResponses",
                type: "TEXT",
                nullable: true);
        }
    }
}
