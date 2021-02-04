using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevryServices.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNull(this string text) => text == null;
        public static bool IsNullOrEmpty(this string text) => string.IsNullOrEmpty(text);
        public static bool IsNullOrWhiteSpace(this string text) => string.IsNullOrWhiteSpace(text);

        public static string OnlyLettersAndNumbers(this string text)
        {
            string result = "";

            for (int i = 0; i < text.Length; i++)
                if (char.IsDigit(text[i]) || char.IsLetter(text[i]))
                    result += text[i];
            
            return result;
        }
    }
}
