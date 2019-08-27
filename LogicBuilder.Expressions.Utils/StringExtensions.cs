using System;
using System.Linq;

namespace LogicBuilder.Expressions.Utils
{
    public static class StringExtensions
    {
        public static string ToPascalCase(this string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsLower(s[0]))
                return s;

            string str = char.ToUpperInvariant(s[0]).ToString();
            if (s.Length > 1)
                str += s.Substring(1);

            return str;
        }

        /// <summary>
        /// Converts each segment of a mulipart reference to camel case e.g. Course.Instructor.LastName becomes course.instructor.lastName
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string s)
        {
            const string PERIOD = ".";
            string[] parts = s.Split(new char[] { PERIOD[0] }, StringSplitOptions.RemoveEmptyEntries);
            parts = parts.Select(p => ConvertToCamelCase(p)).ToArray();

            return string.Join(PERIOD, parts);
        }

        private static string ConvertToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))//Quit if first character is already lowercase
                return s;

            char[] charArray = s.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                //If the second character is not upper case stop processing
                if (i == 1 && !char.IsUpper(charArray[i]))
                    break;

                //i is between the first and last characters AND the next character is lower case
                //i.e. if all previous characters were uppercase then keep setting charcters to lower case
                // until the next charArray[i + 1] is lower case.
                if (i > 0
                    && (i + 1 < charArray.Length)
                    && !char.IsUpper(charArray[i + 1]))
                    break;

                charArray[i] = char.ToLowerInvariant(charArray[i]);
            }

            return new string(charArray);
        }
    }
}
