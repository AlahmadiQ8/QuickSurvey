using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickSurvey.Web.Authentication
{
    public class BasicObfuscator
    {
        private const int Shift = 42;

        public string Obfuscate(int sessionId, string username)
        {
            var input = sessionId + ":" + username;
            var obfuscated = Caesar(input, Shift);
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(obfuscated));
            return encoded;
        }

        public (int sessionId, string username) DeObfuscate(string input)
        {
            var fromBase64String = Convert.FromBase64String(input);
            var decoded = Encoding.UTF8.GetString(fromBase64String);
            var deObfuscated = Caesar(decoded, -Shift).Split(':');
            var sessionId = int.Parse(deObfuscated[0]);
            var username = deObfuscated[1];
            return (sessionId, username);
        }

        /// <summary>
        /// Basic Obfuscation Algorithm
        /// https://stackoverflow.com/a/13026595/5431968
        /// </summary>
        /// <param name="source"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        /// <example>
        /// var obfuscated = Caesar("username", 42);
        /// var shouldBeTrue = Caesar(obfuscated, -42) == "username";
        /// </example>
        private static string Caesar(string source, short shift)
        {
            var maxChar = Convert.ToInt32(char.MaxValue);
            var minChar = Convert.ToInt32(char.MinValue);

            var buffer = source.ToCharArray();

            for (var i = 0; i < buffer.Length; i++)
            {
                var shifted = Convert.ToInt32(buffer[i]) + shift;

                if (shifted > maxChar)
                {
                    shifted -= maxChar;
                }
                else if (shifted < minChar)
                {
                    shifted += maxChar;
                }

                buffer[i] = Convert.ToChar(shifted);
            }

            return new string(buffer);
        }
    }
}
