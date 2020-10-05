using DevryServiceBot.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevryServiceBot
{
    public class DevryDbContext : DbContext
    {
        public DevryDbContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=DevryCommunity.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Questionaire>().HasData(
                    new Questionaire
                    {
                            Title = "Errors",
                            Description = "What are the 3 types of errors which can occur during the execution of a programming?",
                            Type = QuestionaireType.Message,
                            AnswerDescription = "The 3 types are:\n`Syntax`:\tNot following the rules of the language (how it's written). This prevents you from compiling/running your program" +
                            "\n\n`Runtime`: \t While running your program an error occurred. These can be mitigated via try/except try/catch (depending on language)" +
                            "\n\n`Logical`: \t A simple programming mistake. Perhaps using the wrong variable, or wrong mathmatical operator\n",
                            JSON = JsonConvert.SerializeObject(
                                new MessageAnswer
                                {
                                    AllowPartialCredit = true,
                                    PointsForAnswer = 3,
                                    CorrectPhrasesCSV = string.Join(", ", new List<string>()
                                    {
                                        "syntax",
                                        "runtime",
                                        "logical"
                                    })
                                })
                    },

                    new Questionaire
                    {
                        Title = "Errors",
                        Description = "When does a runtime error occur?",
                        Type = QuestionaireType.Reaction,
                        AnswerDescription = "While running your program an uncaught exception occurred. These can be mitigated via `try/except`, `try/catch` (depending on language)",
                        JSON = JsonConvert.SerializeObject(new List<ReactionAnswer>()
                        {
                            new ReactionAnswer
                            {
                                IsCorrect = true,
                                PointsForAnswer = 1,
                                Text = "While the program is running, whenever an un-caught exception is thrown/raised"
                            },
                            new ReactionAnswer
                            {
                                IsCorrect = false,
                                PointsForAnswer = 0,
                                Text = "During compilation"
                            },
                            new ReactionAnswer
                            {
                                IsCorrect = false,
                                PointsForAnswer = 0,
                                Text = "Neither, it's just a bug/mistake in the code."
                            }
                        })
                    },

                    new Questionaire
                    {
                        Title = "Structures",
                        Description = "What are the different kind of `loop` structures in programming?",
                        AnswerDescription = "`For`: \t execute something for a finite amount of times\n\n" +
                        "`While`: \t so long as a condition is true... execute the body of the loop\n\n" +
                        "`Do While`: \t similar to while, except it runs the body first, THEN checks the condition\n\n" +
                        "`Foreach`: \t similar to `for`, except it's a short-hand way of iterating over a collection of items",
                        Type = QuestionaireType.Message,
                        JSON = JsonConvert.SerializeObject(new MessageAnswer
                        {
                            AllowPartialCredit = true,
                            PointsForAnswer = 8,
                            CorrectPhrasesCSV = string.Join(",", new string[] { "for", "while", "do-while", "foreach" })
                        })
                    },

                    new Questionaire
                    {
                        Title = "Structures",
                        Description = "What are the different types of `if` statements?",
                        Type = QuestionaireType.Message,
                        AnswerDescription = "`If`: \t determine if a condition is true, then run the body\n\n" +
                        "`else if` / `elif`: \t When the if statement (or elif / else if) above is NOT true, if the condition here is true...execute the body\n\n" +
                        "`else`: \t when the above statement(s) are not true... this shall get run",
                        JSON = JsonConvert.SerializeObject(new MessageAnswer
                        {
                            AllowPartialCredit = true,
                            PointsForAnswer = 8,
                            CorrectPhrasesCSV = string.Join(",", new string[] { "if", "else", "else if", "elif" })
                        })
                    },

                    new Questionaire
                    {
                        Title = "Access Types",
                        Description = "In Object-Oriented Programming (OOP) what does the keyword `public` mean?",
                        AnswerDescription = "This is an Object-Oriented Programming (OOP) concept. When you create (gaming folks, \"spawn\") an object (a class) it's known " +
                        "as an `instance`. `Public` members (variables/methods) allow you to `access` them outside the scope of the class.",
                        Type = QuestionaireType.Reaction,
                        JSON = JsonConvert.SerializeObject(new List<ReactionAnswer>()
                        {
                            new ReactionAnswer
                            {
                                IsCorrect = true,
                                PointsForAnswer = 2,
                                Text = "The method/variable will be externally accessible"
                            },
                            new ReactionAnswer
                            {
                                IsCorrect = false,
                                PointsForAnswer = 1,
                                Text = "Makes a variable accessible to other methods in the class"
                            },
                            new ReactionAnswer
                            {
                                IsCorrect = false,
                                PointsForAnswer = 0,
                                Text = "Only children, and \"myself\" can access it"
                            }
                        })
                    },

                    new Questionaire
                    {
                        Title = "Access Types",
                        Description = "In Object-Oriented Programming (OOP) what does the keyword `protected` mean?",
                        AnswerDescription = "This is an Object-Oriented Programm (OOP) concept. Protected means \"only myself, and my children can access this\"",
                        Type = QuestionaireType.Reaction,
                        JSON = JsonConvert.SerializeObject(new List<ReactionAnswer>()
                        {
                            new ReactionAnswer
                            {
                                IsCorrect = false,
                                PointsForAnswer = 1,
                                Text = "Only \"I\" can access"
                            },
                            new ReactionAnswer
                            {
                                IsCorrect = false,
                                PointsForAnswer = 0,
                                Text = "\"Anyone\" can access"
                            },
                            new ReactionAnswer
                            {
                                IsCorrect = true,
                                PointsForAnswer = 2,
                                Text = "Only \"myself and my children\" can access\""
                            }
                        })
                    },

                    new Questionaire
                    {
                        Title = "Access Types",
                        Description = "In Object-Oriented Programming (OOP) what does the keyword `private` mean?",
                        AnswerDescription = "In Object-Oriented Programming (OOP) `private` means only \"I\" can access. Meaning, the scope is ONLY within that instance",
                        Type = QuestionaireType.Reaction,
                        JSON = JsonConvert.SerializeObject(new List<ReactionAnswer>()
                        {
                            new ReactionAnswer
                            {
                                IsCorrect = true,
                                PointsForAnswer = 2,
                                Text = "Only \"I\" can access"
                            },
                            new ReactionAnswer
                            {
                                IsCorrect = false,
                                PointsForAnswer = 0,
                                Text = "Only \"myself and my children\" can access"
                            },
                            new ReactionAnswer
                            {
                                IsCorrect = false,
                                PointsForAnswer = 0,
                                Text = "\"Anyone\" can access"
                            }
                        })
                    }
                );
        }

        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Questionaire> Questions { get; set; }
        public DbSet<MemberStats> Stats { get; set; }
        
    }
}
