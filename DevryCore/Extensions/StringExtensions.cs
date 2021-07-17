using System;
using System.Text;

namespace DevryCore.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidCron(this string content, int? min = null, int? max = null)
        {
            foreach(var c in content)
                if (!(char.IsDigit(c) || c == ',' || c == '-' || c == '/' || c == '*'))
                    return false;
            
            if(min.HasValue && max.HasValue && !content.Contains("*"))
            {
                string[] numbers = content.Replace(",", " ")
                    .Replace("-", " ")
                    .Replace("/", " ")
                    .Split(" ");

                foreach (var selection in numbers)
                {
                    if(int.TryParse(selection, out int number))
                        if (number < min || number > max)
                            return false;
                }
            }

            return true;
        }
        
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