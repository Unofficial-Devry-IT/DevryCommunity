using System;
using System.Collections.Generic;
using System.Text;

namespace DevryService.Core
{
    public struct EmojiData
    {
        public string Text;
        public string Icon;
    }

    public static class Emojis
    {
        public static Dictionary<string, string> Data = new Dictionary<string, string>()
        {
            { ":classical_building:", "🏛" },
            { ":white_check_mark:", "✅"},
            { ":negative_squared_cross_mark:", "" },
            {  ":date:", "📅" },
            {  ":desktop:", "🖥" }
        };

        public static readonly EmojiData Role = new EmojiData { Text = ":classical_building:", Icon = "🏛" };
        public static readonly EmojiData CheckMark = new EmojiData { Text = ":white_check_mark:", Icon = "✅" };
        public static readonly EmojiData XMark = new EmojiData { Text = ":negative_squared_cross_mark:", Icon = "" };
        public static readonly EmojiData Date = new EmojiData { Text = ":date:", Icon = "📅" };
        public static readonly EmojiData Code = new EmojiData { Text = ":desktop:", Icon = "🖥" };
    }
}
