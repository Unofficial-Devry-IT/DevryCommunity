using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using DevryInfrastructure;

namespace SnippetAssistant
{
    public static class SnippetStorage
    {
        public static readonly string TopicDescriptionFile = "topic_description.md";
        
        /// <summary>
        /// Retrieve all the directories that are located in the <see cref="StorageHandler.Snippets"/> directory
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSnippetTopics()
            => Directory.GetDirectories(StorageHandler.Snippets).ToList();

        /// <summary>
        /// Collect the various languages represented in the topical directory
        /// </summary>
        /// <param name="topicDirectory">Path to topic directory located in <see cref="StorageHandler.Snippets"/> directory</param>
        /// <returns></returns>
        public static async Task<(Dictionary<string, List<string>> topics, string description)> GetTopicLanguages(string topicDirectory)
        {
            string description = "";

            Dictionary<string, List<string>> results = new();

            foreach (string file in Directory.GetFiles(topicDirectory))
            {
                FileInfo fileInfo = new FileInfo(file);

                // If we find the markdown file containing the topical description -- we shall snag that and continue
                if (fileInfo.Name.Equals(TopicDescriptionFile))
                {
                    description = await File.ReadAllTextAsync(file);
                    continue;
                }
                
                string extension = fileInfo.Extension.Replace(".", "");
                string language = CodeReviewService.GetLanguage(extension);

                if (results.ContainsKey(language))
                    results[language].Add(file);
                else
                    results.Add(language, new List<string>{file});
            }

            return (results, description);
        }

        /// <summary>
        /// Creates the topic directory inside <see cref="StorageHandler.Snippets"/> directory.
        /// Also creates the markdown file containing the topical description
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="topicDescription"></param>
        public static async Task AddTopic(string topicName, string topicDescription)
        {
            string directory = Path.Join(StorageHandler.Snippets, topicName);

            Directory.CreateDirectory(directory);
            await File.WriteAllTextAsync(Path.Join(directory, TopicDescriptionFile), topicDescription);
        }
        
        /// <summary>
        /// Take the textual contents the user provides and stick it in the appropriate place
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="languageName">Name of the language to be used</param>
        /// <param name="fileContents"></param>
        public static async Task AddToTopic(string topicName, string languageName, string fileContents)
        {
            string topicPath = Path.Join(StorageHandler.Snippets, topicName);
            string extension = CodeReviewService.GetExtension(languageName);

            if (!Directory.Exists(topicPath))
                Directory.CreateDirectory(topicName);
            
            int currentAmount = Directory.GetFiles(topicPath).Count(x => x.EndsWith(extension));
            string filePath = Path.Join(topicPath, $"{currentAmount}.{extension}");
            await File.WriteAllTextAsync(filePath, fileContents);
        }

        /// <summary>
        /// Requires the <paramref name="zipFileLocation"/> be a zip file
        /// Allows the user to upload zipped up topics and add them to the snippet collection
        /// </summary>
        /// <param name="topicName">Name of the topic the user wants to create/add to</param>
        /// <param name="zipFileLocation">Location of the zip file (downloaded file)</param>
        /// <param name="deleteZipAfter">Should the zip file be deleted after it's extracted? -- default is true</param>
        /// <exception cref="FileNotFoundException">When the zip file is not located</exception>
        /// <exception cref="NotSupportedException">When the provided file does not have the .zip extension</exception>
        public static void AddToTopic(string topicName, string zipFileLocation, bool deleteZipAfter = true)
        {
            if (!File.Exists(zipFileLocation))
                throw new FileNotFoundException(zipFileLocation);
            
            if (!zipFileLocation.EndsWith(".zip"))
                throw new NotSupportedException("This method only works with zip files, with the .zip extension");

            string topicPath = Path.Join(StorageHandler.Snippets, topicName);

            if (!Directory.Exists(topicPath))
                Directory.CreateDirectory(topicPath);
            
            using ZipArchive archive = ZipFile.OpenRead(zipFileLocation);
            
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string destination = Path.Join(topicPath, entry.Name);
                entry.ExtractToFile(destination);
            }

            // Finally delete the zip file now that it has been consumed
            if(deleteZipAfter)
                File.Delete(zipFileLocation);
        }
    }
}