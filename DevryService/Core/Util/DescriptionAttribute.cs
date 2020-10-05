using System;
using System.Collections.Generic;
using System.Text;

namespace DevryService.Core.Util
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DescriptionAttribute : Attribute
    {
        public string Text { get; set; }
        public string Icon { get; set; }
        public DescriptionAttribute(string text) { this.Text = text; }
        public DescriptionAttribute(string icon, string text)
        {
            this.Text = text;
            this.Icon = icon;
        }
    }
}
