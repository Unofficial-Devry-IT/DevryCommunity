using DevryService.Bot.Exceptions;
using DevryService.Database.Models.Configs;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevryServices.Common.Helpers;

namespace DevryService.Bot.Wizards.Code
{
    public class CreateSnippetWizard : WizardBase
    {
        class ExtensionConfig
        {
            public List<string> RestrictedRoles = new List<string>();
        }

        string _language = string.Empty,
            _content = string.Empty,
            _topic = string.Empty,
            _extension = string.Empty,
            _filename = string.Empty;

        ExtensionConfig _extensionConfig;

        Dictionary<string, CodeInfo> codeInfoMap = new Dictionary<string, CodeInfo>();

        public CreateSnippetWizard(CommandContext context) : base(context)
        {
            
        }

        protected override void Initialize()
        {
            base.Initialize();
            _extensionConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ExtensionConfig>(ExtensionData);

            codeInfoMap = ConfigService.GetCodeInfo()
                .ToDictionary(x => x.HumanReadableName, x => x);
        }

        protected override async Task ExecuteAsync()
        {
            // This shall start the process of being able to modify our first embed message for further inquery 
            var response = await WithReply<string>(BasicEmbed()
                .WithDescription("What's the topic of this code snippet? (i.e Arrays, classes, etc)")
                .Build());

            _topic = response.value;
            _extension = await ReplyWithEdit<string>(response.message, BasicEmbed()
                .WithDescription("What's the file extension of this particular language? (csv, json, without the period")
                .Build());
            _filename = await ReplyWithEdit<string>(response.message, BasicEmbed()
                .WithDescription($"What should the name of this file be? For easier reference... no extension as {_extension} will be applied automatically")
                .Build());

            // Attempt to locate the code information based on user input
            var info = codeInfoMap.Values.FirstOrDefault(x => x.FileExtension.Replace(".", "").Equals(_extension, StringComparison.OrdinalIgnoreCase));

            // If not found we need to ask the user some additional questions
            if(info == null)
            {
                // Readable name for the interface
                string readableName = readableName = await ReplyWithEdit<string>(response.message, BasicEmbed()
                    .WithDescription("Looks like a new type of language is being used... What is the 'human readable' text for this language? For instance, C#, C++, YAML, JSON, etc")
                    .Build());

                // langauge that shall be used to format the discord message
                _language = await ReplyWithEdit<string>(response.message, BasicEmbed()
                    .WithDescription("What is the programming language for this? (this is what will be used to format the code in discord)").Build());

                info = new CodeInfo
                {
                    EmbedColor = new Color { },
                    DiscordCodeBlockLanguage = _language,
                    FileExtension = _extension,
                    HumanReadableName = readableName
                };

                await ConfigService.AddCodeInfo(info);
            }

            // Content that will be inserted into the appropriate file
            _content = await ReplyWithEdit<string>(response.message, BasicEmbed()
                .WithDescription("Provide the content you wish to share (in discord's code-block format").Build());

            string directoryPath = System.IO.Path.Combine(ConfigService.SnippetPath, _topic);

            // Ensure the user provided some valid content
            if(string.IsNullOrEmpty(_content))
            {
                await SimpleReply(ErrorEmbed("Must provide some sort of content"));
                throw new StopWizardException(AuthorName);
            }

            // Must start and end with the proper formatting, otherwise the user provided something we weren't expecting.
            if(!_content.StartsWith("```") || !_content.EndsWith("```"))
            {
                await SimpleReply(ErrorEmbed("Was expecting just a code block, which means your message " +
                    "should start AND end with 3 backticks (look for the ~ key on your keyboard). " +
                    "Can't use backticks in this response otherwise it won't appear."), false);
                throw new StopWizardException(AuthorName);
            }

            // First line should contain the language and backticks
            // Last line should contain the back ticks... however it is possible the user may
            // have actual content there too... so replace the ticks instead
            List<string> lines = _content.Split("\n").ToList();
            lines.RemoveAt(0);
            lines[lines.Count - 1] = lines[lines.Count - 1].Replace("```", "");

            try
            {
                if (!System.IO.Directory.Exists(directoryPath))
                    System.IO.Directory.CreateDirectory(directoryPath);

                await System.IO.File.WriteAllTextAsync(System.IO.Path.Combine(directoryPath, $"{_filename}.{_extension}"), string.Join("\n", lines));
            }
            catch(Exception ex)
            {
                await SimpleReply(ErrorEmbed($"An error occurred while saving your snippet. View logs for more information..."));

                this.PrintItems($"{AuthorName} Wizard has an error...",
                    new string[]
                    {
                        nameof(_extension),
                        nameof(_language),
                        nameof(_topic),
                        nameof(_filename),
                        nameof(_content)
                    }, new string[] { });
                

                throw new StopWizardException(AuthorName);
            }
        }

        protected override string GetDefaultExtensionData() =>
            Newtonsoft.Json.JsonConvert.SerializeObject(
                new ExtensionConfig
                {
                    RestrictedRoles = new List<string>()
                    {
                        "Moderator",
                        "Admin",
                        "Tutor",
                        "Professor"
                    }
                });

        protected override string GetDefaultAuthorIcon() => "";
        protected override string GetDefaultAuthorName() => "Coder Hat";
        protected override string GetDefaultDescription() => "Add some code snippets to help users out!";
        protected override string GetDefaultHeadline() => "";
        protected override TimeSpan? GetDefaultTimeoutOverride() => null;
    }
}
