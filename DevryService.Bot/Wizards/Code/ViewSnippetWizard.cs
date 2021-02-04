using DevryService.Database.Models.Configs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DevryService.Bot.Wizards.Code
{
    

    public class ViewSnippetWizard : WizardBase
    {
        class ExtensionConfig
        {
            public string RootCodePath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "Snippets");
        }

        private ExtensionConfig _extensionConfig;
        private List<string> _categories;
        private Dictionary<string, CodeInfo> codeInfoMap = new Dictionary<string, CodeInfo>();

        public ViewSnippetWizard(CommandContext context) : base(context)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            _extensionConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ExtensionConfig>(ExtensionData);
            _categories = GetSnippetCategories();

            codeInfoMap = ConfigService.GetCodeInfo()
                .ToDictionary(x => x.HumanReadableName, x => x);
        }

        string BasicMessage => $"{AuthorName} Wizard. Please follow the instructions below.\n";


        protected override async Task ExecuteAsync()
        {
            var embed = BasicEmbed()
                .WithDescription(BasicMessage);

            for (int i = 0; i < _categories.Count; i++)
                embed.AddField((i + 1).ToString(), _categories[i]);

            var response = await WithReply<string>(embed.Build());

        }

        async Task SelectLanguage(string selectedCategory)
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

            var embed = BasicEmbed()
                .WithDescription($"{BasicMessage}Selected the corresponding number(s) to select a language, or languages\n\n");

            for (int i = 0; i < options.Count; i++)
                embed.AddField((i + 1).ToString(), options[i], true);

            var response = await WithReply<string>(embed.Build(), true);

            List<string> selectedLanguages = new List<string>();

            foreach(var parameter in response.message.Content.Replace(","," ").Split(" "))
            {
                if(int.TryParse(parameter, out int index))
                {
                    index -= 1;

                    if (index < 0 || index > options.Count)
                        continue;

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
                await Task.Delay(250);

                string block = await CreateCodeBlock(file);

                var embed = BasicEmbed()
                    .WithTitle(lang)
                    .WithDescription(block)
                    .WithColor(GetColor(lang));

                await Context.Channel.SendMessageAsync(embed: embed.Build());
            }
        }

        List<string> GetSnippetCategories()
        {
            string basePath = Path.Combine(_extensionConfig.RootCodePath);

            return Directory.GetDirectories(basePath)
                .Select(x => new DirectoryInfo(x).Name)
                .ToList();
        }

        /// <summary>
        /// Get color that shall be used in <see cref="DiscordEmbed"/>
        /// </summary>
        /// <param name="name">Language that you want the color for</param>
        /// <returns></returns>
        DiscordColor GetColor(string name)
        {
            if(codeInfoMap.ContainsKey(name))
            {
                var info = codeInfoMap[name];
                return new DiscordColor(info.EmbedColor.Red, info.EmbedColor.Green, info.EmbedColor.Blue);
            }

            return DiscordColor.Cyan;
        }

        /// <summary>
        /// Grab all files within specified directory
        /// </summary>
        /// <param name="directory">Path to directory</param>
        /// <returns>List of <see cref="FileInfo"/> for files in <paramref name="directory"/></returns>
        List<FileInfo> GetFilesInCategory(string directory)
        {
            string path = Path.Combine(_extensionConfig.RootCodePath, directory);

            return Directory.GetFiles(path).Select(x => new FileInfo(x)).ToList();
        }

        string CodeBlockLanguage(FileInfo info)
        {
            var obj = ConfigService.GetCodeInfoByExt(info.Extension);

            if (obj != null)
                return obj.DiscordCodeBlockLanguage;

            return info.Extension.Replace(".", "");
        }

        string ExtensionToName(string extension)
        {
            var info = ConfigService.GetCodeInfoByExt(extension);

            if (info == null)
                return string.Empty;

            return info.FileExtension.Replace(".","");
        }

        string NameToExtension(string name)
        {
            if(codeInfoMap.ContainsKey(name))
                return codeInfoMap[name].FileExtension;
            return $".{name}";
        }

        string GetCategory(string text)
        {
            if(int.TryParse(text, out int index))
            {
                index -= 1;

                if (index < 0 || index > _categories.Count)
                    return null;
                return _categories[index];
            }

            return null;
        }

        /// <summary>
        /// Generate code block from provided file
        /// </summary>
        /// <param name="info">File to load and ouptut as code block</param>
        /// <returns>Formatted string</returns>
        async Task<string> CreateCodeBlock(FileInfo info)
        {
            string contents = await File.ReadAllTextAsync(info.FullName);
            string lang = CodeBlockLanguage(info);
            return $"```{lang}\n{contents}\n```";
        }

        protected override string GetDefaultAuthorIcon() => "";
        protected override string GetDefaultAuthorName() => "Programming Hat";
        protected override string GetDefaultDescription() => "Provides a variety of code snippets for various topics and in multiple languages";
        protected override string GetDefaultHeadline() => "Snippets";
        protected override TimeSpan? GetDefaultTimeoutOverride() => null;

        protected override string GetDefaultExtensionData() => Newtonsoft.Json.JsonConvert.SerializeObject(new ExtensionConfig
        {
           
        });
    }
}
