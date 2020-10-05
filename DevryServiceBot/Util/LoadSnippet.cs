using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using DSharpPlus.Entities;

namespace DevryServiceBot.Util
{
    public struct LangInfo
    {
        public string Name;
        public string Extension;
        public DiscordColor Color;
        public string CodeBlock;
    }

    public static class LoadSnippet
    {
        public static List<LangInfo> LanguageInformation = new List<LangInfo>
        {
            new LangInfo
            {
                Color = DiscordColor.SpringGreen,
                Name = "C#",
                Extension = ".cs",
                CodeBlock = "csharp"
            },
            new LangInfo
            {
                Color = DiscordColor.CornflowerBlue,
                Name = "Python",
                Extension = ".py",
                CodeBlock = "python"
            },
            new LangInfo
            {
                Color = DiscordColor.Purple,
                Name = "C++",
                Extension = ".cpp",
                CodeBlock = "cpp"
            },
            new LangInfo
            {
                Color = DiscordColor.SpringGreen,
                Name = "SQL",
                Extension = ".sql",
                CodeBlock = "sql"
            }
        };

        /// <summary>
        /// Get Language Color based off display name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DiscordColor GetColor(string name)
        {
            if (LanguageInformation.Any(x => x.Name.ToLower() == name.ToLower()))
                return LanguageInformation.FirstOrDefault(x => x.Name.ToLower() == name.ToLower()).Color;
            else
                return DiscordColor.Cyan;
        }

        /// <summary>
        /// Generate a discord-code block based off file
        /// </summary>
        /// <param name="info">File to display</param>
        /// <returns>Formatted code block</returns>
        public static string CodeBlockForFile(FileInfo info)
        {
            if (LanguageInformation.Any(x => x.Extension.Equals(info.Extension, System.StringComparison.OrdinalIgnoreCase)))
                return LanguageInformation.FirstOrDefault(x => x.Extension.Equals(info.Extension, System.StringComparison.OrdinalIgnoreCase)).CodeBlock;
            else 
                return info.Extension.Replace(".", " ");
        }

        /// <summary>
        /// Display name to extension
        /// </summary>
        /// <param name="name">Display name of a language</param>
        /// <returns></returns>
        public static string NameToExtension(string name)
        {
            if (LanguageInformation.Any(x => name.Equals(x.Name, System.StringComparison.OrdinalIgnoreCase)))
                return LanguageInformation.FirstOrDefault(x => name.Equals(x.Name, System.StringComparison.OrdinalIgnoreCase)).Extension;
            else
                return name.ToLower();
        }

        /// <summary>
        /// Extension to Display name
        /// </summary>
        /// <param name="extension">File extension</param>
        /// <returns>Display name of language</returns>
        public static string ExtensionToName(string extension)
        {
            if (LanguageInformation.Any(x => x.Extension.Equals(extension, System.StringComparison.OrdinalIgnoreCase)))
                return LanguageInformation.FirstOrDefault(x => x.Extension.Equals(extension, System.StringComparison.OrdinalIgnoreCase)).Name;
            return 
                extension.Replace(".", "");
        }

        /// <summary>
        /// Retrieve all the folders under /snippets
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSnippetCategories()
        {
            return Directory.GetDirectories("snippets")
                .Select(x => new DirectoryInfo(x).Name)
                .ToList();
        }

        /// <summary>
        /// Get all files within a category
        /// </summary>
        /// <seealso cref="GetSnippetCategories"/>
        /// <param name="directory">Directory to scan files</param>
        /// <returns>All files within directory</returns>
        public static List<FileInfo> GetFilesInCategory(string directory)
        {            
            string path = $"{Directory.GetCurrentDirectory()}/snippets/{directory}";

            return Directory.GetFiles(path).Select(x => new FileInfo(x)).ToList();
        }

        /// <summary>
        /// Generate a discord code block based on file information
        /// </summary>
        /// <param name="info"></param>
        /// <returns>Formatted fcode block</returns>
        public static async Task<string> CreateCodeBlock(FileInfo info)
        {
            string contents = await File.ReadAllTextAsync(info.FullName);
            string lang = CodeBlockForFile(info);

            return $"```{lang}\n{contents}\n```";            
        }
    }
}
