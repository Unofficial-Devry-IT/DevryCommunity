using DevryService.Core;
using DevryService.Core.Util;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Wizards
{
    [System.Serializable]
    public struct CodeInfo
    {
        public string HumanReadableName { get; set; }
        public string Extension { get; set; }
        public DiscordColor Color { get; set; }
        public string DiscordCodeBlockLanguage { get; set; }
    }

    public class SnippetWizardConfig : WizardConfig
    {
        public string RootCodePath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "Snippets");
        public List<CodeInfo> CodeBlocks { get; set; } = new List<CodeInfo>();
    }

    public class SnippetWizard : WizardBase<SnippetWizardConfig>
    {
        const string AUTHOR_NAME = "Programming Hat";
        const string DESCRIPTION = "Provides a variety of code snippets for various topics and in multiple languages";
        const string REACTION_EMOJI = ":desktop:";
        const string AUTHOR_ICON = "";
        public override SnippetWizardConfig DefaultSettings()
        {
            SnippetWizardConfig config = new SnippetWizardConfig();

            config.AuthorName = AUTHOR_NAME;
            config.Description = DESCRIPTION;
            config.ReactionEmoji = REACTION_EMOJI;
            config.Headline = "Snippets";
            config.AcceptAnyUser = false;
            config.MessageRequireMention = false;
            config.AuthorIcon = AUTHOR_ICON;

            config.CodeBlocks = new List<CodeInfo>()
            {
                new CodeInfo
                {
                    Color = DiscordColor.SpringGreen,
                    HumanReadableName = "C#",
                    Extension = ".cs",
                    DiscordCodeBlockLanguage ="csharp"
                },

                new CodeInfo
                {
                    Color = DiscordColor.CornflowerBlue,
                    HumanReadableName = "Python",
                    Extension = ".py",
                    DiscordCodeBlockLanguage = "python"
                },

                new CodeInfo
                {
                    Color = DiscordColor.Purple,
                    HumanReadableName = "C++",
                    Extension = ".cpp",
                    DiscordCodeBlockLanguage = "cpp"
                },

                new CodeInfo
                {
                    Color = DiscordColor.SpringGreen,
                    HumanReadableName = "SQL",
                    Extension = ".sql",
                    DiscordCodeBlockLanguage = "sql"
                }
            };

            config.UsesCommand = new WizardToCommandLink
            {
                DiscordCommand = "code",
                CommandConfig = DefaultCommandConfig()
            };

            return config;
        }

        public override CommandConfig DefaultCommandConfig()
        {
            return new CommandConfig
            {
                AuthorName = AUTHOR_NAME,
                Description = DESCRIPTION,
                ReactionEmoji = REACTION_EMOJI,
                IgnoreHelpWizard = false
            };
        }

        List<string> Categories = new List<string>();
        const string Basic = "Snippet - Wizard. Please follow the instructions below\n";

        public SnippetWizard(CommandContext commandContext) : base(commandContext)
        {
        }

        string GetCategory(string replyContent)
        {
            if(int.TryParse(replyContent, out int index))
            {
                index -= 1;

                if (index < 0 || index > this.Categories.Count)
                    return null;
                else
                    return Categories[index];
            }

            return null;
        }

        async Task SelectLanguage(CommandContext context, string selectedCategory)
        {
            var files = GetFilesInCategory(selectedCategory);
            var extensions = files.Select(x => x.Extension)
                .Distinct()
                .ToList();

            var groups = files.GroupBy(x => x.Extension)
                .ToDictionary(x => x.Key, x => x.ToList());

            List<string> options = new List<string>();
            foreach (var ext in extensions)
                options.Add(ExtensionToName(ext));

            var embed = EmbedBuilder().WithFooter(CANCEL_MESSAGE).WithDescription(Basic + "Select the corresponding number(s) to select a language, or languages\n\n");

            for (int i = 0; i < options.Count; i++)
                embed.AddField((i + 1).ToString(), options[i], true);

            string reply = string.Empty;

            _recentMessage = await ReplyEditWithReply(_recentMessage, embed.Build(), (context) => ReplyHandlerAction(context, ref reply));

            List<string> selectedLanguages = new List<string>();
            foreach(var parameter in reply.Replace(","," ").Split(" "))
            {
                if(int.TryParse(parameter, out int index))
                {
                    index -= 1;
                    if (index < 0 || index > options.Count)
                        continue;
                    else
                        selectedLanguages.Add(options[index]);
                }
            }

            await CleanupAsync();

            foreach (var lang in selectedLanguages)
                await DisplayCode(lang, groups[NameToExtension(lang)]);
        }

        async Task DisplayCode(string lang, List<FileInfo> files)
        {
            foreach(var file in files)
            {
                await Task.Delay(2500);
                string block = await CreateCodeBlock(file);

                DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
                    .WithAuthor(_options.AuthorName, null, _options.AuthorIcon)
                    .WithTitle(lang)
                    .WithDescription(block)
                    .WithColor(GetColor(lang));

                await _channel.SendMessageAsync(embed: builder.Build());
            }
        }

        protected override async Task ExecuteAsync(CommandContext context)
        {
            Categories = GetSnippetCategories();

            string categoryMenu = Basic + "Select the corresponding number to selct a category\n\n";

            for (int i = 0; i < Categories.Count; i++)
                categoryMenu += $"[{i + 1}] - {Categories[i]}\n";

            string reply = string.Empty;
            _recentMessage = await WithReply(context, categoryMenu, (context)=>ReplyHandlerAction(context, ref reply), true);

            if(string.IsNullOrEmpty(reply))
            {
                await CleanupAsync();
                return;
            }

            string category = GetCategory(reply.Trim());

            if(string.IsNullOrEmpty(category))
            {
                await CleanupAsync();
                await context.RespondAsync(embed: new DiscordEmbedBuilder()
                    .WithAuthor(_options.AuthorName, null, _options.AuthorIcon)
                    .WithTitle("Invalid Input")
                    .WithDescription($"Expected value between 1 - {Categories.Count}")
                    .WithColor(DiscordColor.IndianRed)
                    .Build());
                return;
            }

            var files = GetFilesInCategory(category);
            var extensions = files.Select(x => x.Extension)
                .Distinct()
                .ToList();

            var groups = files.GroupBy(x => x.Extension)
                .ToDictionary(x => x.Key, x => x.ToList());

            List<string> options = new List<string>();
            foreach (var ext in extensions)
                options.Add(ExtensionToName(ext));

            var embed = EmbedBuilder().WithFooter(CANCEL_MESSAGE).WithDescription(Basic + "Select the corresponding number(s) to select a language, or languages\n\n");

            for (int i = 0; i < options.Count; i++)
                embed.AddField((i + 1).ToString(), options[i], true);

            /*
                TODO: Fix Reply Edit With Message/Reaction
             */

            _recentMessage = await ReplyEdit(_recentMessage, embed.Build());

            var response = await context.Message.GetNextMessageAsync();
            if(response.TimedOut)
            {
                await SimpleReply(context, $"{_options.AuthorName} Wizard Timed Out...", false, false);
                throw new StopWizardException(_options.AuthorName);
            }


            reply = response.Result.Content.Trim();
            List<string> selectedLanguages = new List<string>();

            foreach(var parameter in reply.Replace(",", " ").Split(" "))
            {
                if(int.TryParse(parameter, out int index))
                {
                    index -= 1;

                    if (index < 0 || index > options.Count)
                        continue;
                    else 
                        selectedLanguages.Add(options[index]);
                }
            }

            await CleanupAsync();

            foreach (var lang in selectedLanguages)
                await DisplayCode(lang, groups[NameToExtension(lang)]);
        }        

        /// <summary>
        /// Get color that shall be used in <see cref="DiscordEmbed"/>
        /// </summary>
        /// <param name="name">Language name that you want the color for</param>
        /// <returns><see cref="DiscordColor"/></returns>
        DiscordColor GetColor(string name)
        {
            if (_options.CodeBlocks.Any(x => x.HumanReadableName.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return _options.CodeBlocks.FirstOrDefault(x => x.HumanReadableName.Equals(name, StringComparison.OrdinalIgnoreCase)).Color;
            else
                return DiscordColor.Cyan;
        }

        /// <summary>
        /// Get the associated language for the <paramref name="info"/>
        /// </summary>
        /// <param name="info"></param>
        /// <returns>Language to use in code block</returns>
        string CodeBlockLanguage(FileInfo info)
        {
            if (_options.CodeBlocks.Any(x => x.Extension.Equals(info.Extension, StringComparison.OrdinalIgnoreCase)))
                return _options.CodeBlocks.FirstOrDefault(x => x.Extension.Equals(info.Extension, StringComparison.OrdinalIgnoreCase)).DiscordCodeBlockLanguage;
            else
                return info.Extension.Replace(".", " ");
        }

        /// <summary>
        /// Looks for the associated <see cref="CodeInfo"/> based on extension
        /// </summary>
        /// <param name="extension">Extension to get name from</param>
        /// <returns>Name for <paramref name="extension"/></returns>
        string ExtensionToName(string extension)
        {
            if (_options.CodeBlocks.Any(x => x.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                return _options.CodeBlocks.FirstOrDefault(x => x.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase)).HumanReadableName;
            else
                return extension.Replace(".", "");
        }

        string NameToExtension(string name)
        {
            if (_options.CodeBlocks.Any(x => x.HumanReadableName.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return _options.CodeBlocks.FirstOrDefault(x => x.HumanReadableName.Equals(name, StringComparison.OrdinalIgnoreCase)).Extension;

            return $".{name}";
        }

        /// <summary>
        /// Get all folders located at <see cref="SnippetWizardConfig.RootCodePath"/>
        /// </summary>
        /// <returns>List of folders within <see cref="SnippetWizardConfig.RootCodePath"/></returns>
        List<string> GetSnippetCategories()
        {
            string basePath = _options.RootCodePath;

            return Directory.GetDirectories(basePath)
                .Select(x => new DirectoryInfo(x).Name)
                .ToList();
        }

        /// <summary>
        /// Grab all files within specified directory
        /// </summary>
        /// <param name="directory">Path to directory</param>
        /// <returns>List of <see cref="FileInfo"/> for files in <paramref name="directory"/></returns>
        List<FileInfo> GetFilesInCategory(string directory)
        {
            string path = Path.Combine(_options.RootCodePath, directory);
            return Directory.GetFiles(path).Select(x => new FileInfo(x)).ToList();
        }

        /// <summary>
        /// Generate code block from provided file
        /// </summary>
        /// <param name="info">File to load and output as code block</param>
        /// <returns>Formatted string (in code block format)</returns>
        async Task<string> CreateCodeBlock(FileInfo info)
        {
            string contents = await File.ReadAllTextAsync(info.FullName);
            string lang = CodeBlockLanguage(info);

            return $"```{lang}\n{contents}\n```";
        }
    }
}
