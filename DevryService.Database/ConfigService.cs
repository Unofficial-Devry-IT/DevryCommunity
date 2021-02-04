using DevryService.Database.Models.Configs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DevryService.Database
{
    public class ConfigService
    {
        private readonly ILogger<ConfigService> _logger;
        private readonly DevryDbContext _context;

        public string SnippetPath => Path.Combine(Directory.GetCurrentDirectory(), "Snippets");

        public ConfigService(ILogger<ConfigService> logger, DevryDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public WizardConfig GetWizardConfig(string wizardName)
        {
            return null;
        }

        public CommandConfig GetCommandConfig(string discordCommand)
        {
            CommandConfig config =  _context.CommandConfigs.FirstOrDefault(x => x.DiscordCommand.Equals(discordCommand, StringComparison.OrdinalIgnoreCase));

            return config;
        }

        /// <summary>
        /// Get <see cref="CodeInfo"/> by <see cref="CodeInfo.HumanReadableName"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CodeInfo GetCodeInfoByName(string name) => _context.CodeInfo.FirstOrDefault(x => x.HumanReadableName.Equals(name, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Search for <see cref="CodeInfo"/> by extension name
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public CodeInfo GetCodeInfoByExt(string ext) => _context.CodeInfo
            .FirstOrDefault(x => x.FileExtension
                                .Replace(".", "")
                                .Equals(ext.Replace(".", ""), StringComparison.OrdinalIgnoreCase));

        public List<CodeInfo> GetCodeInfo() => _context.CodeInfo.ToList();

        /// <summary>
        /// Add new instance of <see cref="CodeInfo"/>
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task AddCodeInfo(CodeInfo info)
        {
            await _context.CodeInfo.AddAsync(info);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Update existing code info entity
        /// </summary>
        /// <param name="info"></param>
        /// <returns>Success: whether or not item was updated</returns>
        public async Task<(bool success, string message)> UpdateCodeInfo(CodeInfo info)
        {
            var original = await _context.CodeInfo.FindAsync(info.Id);

            if (original == null)
                return (false, "Not Found");

            _context.CodeInfo.Update(info);
            await _context.SaveChangesAsync();

            return (true, string.Empty);
        }
    }
}
