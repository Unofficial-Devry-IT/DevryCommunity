using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DevryServiceBot.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    AnswerDescription = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    LastUsed = table.Column<DateTime>(nullable: true),
                    Duration = table.Column<double>(nullable: false),
                    JSON = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    GuildId = table.Column<ulong>(nullable: false),
                    ChannelId = table.Column<ulong>(nullable: false),
                    CronString = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Contents = table.Column<string>(nullable: true),
                    NextRunTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Points = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "AnswerDescription", "Description", "Duration", "JSON", "LastUsed", "Title", "Type" },
                values: new object[] { new Guid("b42f9613-964f-4324-b073-c8c63a60c723"), @"The 3 types are:
`Syntax`:	Not following the rules of the language (how it's written). This prevents you from compiling/running your program

`Runtime`: 	 While running your program an error occurred. These can be mitigated via try/except try/catch (depending on language)

`Logical`: 	 A simple programming mistake. Perhaps using the wrong variable, or wrong mathmatical operator
", "What are the 3 types of errors which can occur during the execution of a programming?", 0.0, "{\"CorrectPhrasesCSV\":\"syntax, runtime, logical\",\"AllowPartialCredit\":true,\"PointsForAnswer\":3,\"CorrectPhrases\":[\"syntax\",\" runtime\",\" logical\"]}", null, "Errors", 1 });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "AnswerDescription", "Description", "Duration", "JSON", "LastUsed", "Title", "Type" },
                values: new object[] { new Guid("6f67be31-b143-4c8d-bc32-32ecc5f15866"), "While running your program an uncaught exception occurred. These can be mitigated via `try/except`, `try/catch` (depending on language)", "When does a runtime error occur?", 0.0, "[{\"Text\":\"While the program is running, whenever an un-caught exception is thrown/raised\",\"IsCorrect\":true,\"PointsForAnswer\":1},{\"Text\":\"During compilation\",\"IsCorrect\":false,\"PointsForAnswer\":0},{\"Text\":\"Neither, it's just a bug/mistake in the code.\",\"IsCorrect\":false,\"PointsForAnswer\":0}]", null, "Errors", 0 });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "AnswerDescription", "Description", "Duration", "JSON", "LastUsed", "Title", "Type" },
                values: new object[] { new Guid("f2c13a4e-b571-4b9e-b799-0220b395e30a"), @"`For`: 	 execute something for a finite amount of times

`While`: 	 so long as a condition is true... execute the body of the loop

`Do While`: 	 similar to while, except it runs the body first, THEN checks the condition

`Foreach`: 	 similar to `for`, except it's a short-hand way of iterating over a collection of items", "What are the different kind of `loop` structures in programming?", 0.0, "{\"CorrectPhrasesCSV\":\"for,while,do-while,foreach\",\"AllowPartialCredit\":true,\"PointsForAnswer\":8,\"CorrectPhrases\":[\"for\",\"while\",\"do-while\",\"foreach\"]}", null, "Structures", 1 });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "AnswerDescription", "Description", "Duration", "JSON", "LastUsed", "Title", "Type" },
                values: new object[] { new Guid("f0e75811-088b-47c5-b2d0-fc53be157e46"), @"`If`: 	 determine if a condition is true, then run the body

`else if` / `elif`: 	 When the if statement (or elif / else if) above is NOT true, if the condition here is true...execute the body

`else`: 	 when the above statement(s) are not true... this shall get run", "What are the different types of `if` statements?", 0.0, "{\"CorrectPhrasesCSV\":\"if,else,else if,elif\",\"AllowPartialCredit\":true,\"PointsForAnswer\":8,\"CorrectPhrases\":[\"if\",\"else\",\"else if\",\"elif\"]}", null, "Structures", 1 });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "AnswerDescription", "Description", "Duration", "JSON", "LastUsed", "Title", "Type" },
                values: new object[] { new Guid("38fd3f75-e62c-468d-95af-ff66251fcb68"), "This is an Object-Oriented Programming (OOP) concept. When you create (gaming folks, \"spawn\") an object (a class) it's known as an `instance`. `Public` members (variables/methods) allow you to `access` them outside the scope of the class.", "In Object-Oriented Programming (OOP) what does the keyword `public` mean?", 0.0, "[{\"Text\":\"The method/variable will be externally accessible\",\"IsCorrect\":true,\"PointsForAnswer\":2},{\"Text\":\"Makes a variable accessible to other methods in the class\",\"IsCorrect\":false,\"PointsForAnswer\":1},{\"Text\":\"Only children, and \\\"myself\\\" can access it\",\"IsCorrect\":false,\"PointsForAnswer\":0}]", null, "Access Types", 0 });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "AnswerDescription", "Description", "Duration", "JSON", "LastUsed", "Title", "Type" },
                values: new object[] { new Guid("768c3b03-c98f-4d51-b888-edbbe2b92a82"), "This is an Object-Oriented Programm (OOP) concept. Protected means \"only myself, and my children can access this\"", "In Object-Oriented Programming (OOP) what does the keyword `protected` mean?", 0.0, "[{\"Text\":\"Only \\\"I\\\" can access\",\"IsCorrect\":false,\"PointsForAnswer\":1},{\"Text\":\"\\\"Anyone\\\" can access\",\"IsCorrect\":false,\"PointsForAnswer\":0},{\"Text\":\"Only \\\"myself and my children\\\" can access\\\"\",\"IsCorrect\":true,\"PointsForAnswer\":2}]", null, "Access Types", 0 });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "AnswerDescription", "Description", "Duration", "JSON", "LastUsed", "Title", "Type" },
                values: new object[] { new Guid("1a7c5e31-9d9d-4aec-839c-55ba1557daac"), "In Object-Oriented Programming (OOP) `private` means only \"I\" can access. Meaning, the scope is ONLY within that instance", "In Object-Oriented Programming (OOP) what does the keyword `private` mean?", 0.0, "[{\"Text\":\"Only \\\"I\\\" can access\",\"IsCorrect\":true,\"PointsForAnswer\":2},{\"Text\":\"Only \\\"myself and my children\\\" can access\",\"IsCorrect\":false,\"PointsForAnswer\":0},{\"Text\":\"\\\"Anyone\\\" can access\",\"IsCorrect\":false,\"PointsForAnswer\":0}]", null, "Access Types", 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.DropTable(
                name: "Stats");
        }
    }
}
