using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Core.Util
{
    public struct LangInfo
    {
        public string Name;
        public string Extension;
        public DiscordColor Color;
        public string CodeBlock;
    }

    public static class LoadFile
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
        /// Get language color based off display name
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
        /// Generaete a discord-code block based off file
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
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
        /// <param name="name"></param>
        /// <returns></returns>
        public static string NameToExtension(string name)
        {
            if (LanguageInformation.Any(x => name.Equals(x.Name, StringComparison.OrdinalIgnoreCase)))
                return LanguageInformation.FirstOrDefault(x => name.Equals(x.Name, StringComparison.OrdinalIgnoreCase)).Extension;
            else
                return name.ToLower();
        }

        /// <summary>
        /// Extension to Display Name
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string ExtensionToName(string extension)
        {
            if (LanguageInformation.Any(x => x.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                return LanguageInformation.FirstOrDefault(x => x.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase)).Name;
            else
                return extension.Replace(".", "");
        }

        private static string SnippetsBasePath()
        {
            if (Directory.Exists("Snippets"))
                return "Snippets";
            else if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Snippets")))
                return Path.Combine(Directory.GetCurrentDirectory(), "Snippets");
            else
            {
                Console.WriteLine($"Unable to find Snippets directory");
                throw new DirectoryNotFoundException("Unable to locate Snippets directory");
            }
        }

        /// <summary>
        /// Retrieve all folders under /snippets
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSnippetCategories()
        {
            string basePath = SnippetsBasePath();

            return Directory.GetDirectories(basePath)
                .Select(x => new DirectoryInfo(x).Name)
                .ToList();
        }

        /// <summary>
        /// Get all files within a category
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static List<FileInfo> GetFilesInCategory(string directory)
        {
            string path = Path.Combine(SnippetsBasePath(), directory);
            return Directory.GetFiles(path).Select(x => new FileInfo(x)).ToList();
        }

        /// <summary>
        /// Generate a discord code block based on file extension
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static async Task<string> CreateCodeBlock(FileInfo info)
        {
            string contents = await File.ReadAllTextAsync(info.FullName);
            string lang = CodeBlockForFile(info);

            return $"```{lang}\n{contents}\n```";
        }
    }
}
