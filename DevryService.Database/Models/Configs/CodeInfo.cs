using DevryServices.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevryService.Database.Models.Configs
{
    [Serializable]
    public class CodeInfo : EntityBase
    {
        public CodeInfo()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string HumanReadableName { get; set; }
        public string FileExtension { get; set; }
        public Color EmbedColor { get; set; }
        public string DiscordCodeBlockLanguage { get; set; }
    }

    public struct Color
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
    }
}
