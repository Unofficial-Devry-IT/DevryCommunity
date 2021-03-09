using System;
using System.Text;

namespace Application.Helpers
{
    public static class TextExtensions
    {
        public static string FromBase64(this string text)
        {
            var valueBytes = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(valueBytes);
        }

        public static string ToBase64(this string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }
    }
}