using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Schema;

namespace KingpinNet
{
    public class CommandLineTokenizer
    {
        private const char space = ' ';
        private const char tab = '\t';
        private const char carriage = '\r';
        private const char newline = '\n';
        private const char quotes = '"';

        private char[] seperators = new [] { space, tab, carriage, newline };
        private char[] illegalChars = new[] { space, tab, carriage, newline, quotes };
        private bool inQuotes = false;
        private bool seperatorFound = false;
        private bool wordFound = false;
        public List<string> ToTokens(string commandLine)
        {
            var result = new List<string>();
            var currentWord = "";
            int currentPosition = 0;
            var trimmedCommandLine = commandLine.TrimStart();
            foreach (var chr in trimmedCommandLine)
            {
                if (chr == quotes && !inQuotes && seperatorFound)
                {
                    inQuotes = true;
                    currentPosition++;
                    continue;
                }
                else if (chr == quotes && inQuotes)
                {
                    result.Add(currentWord);
                    currentWord = "";
                    inQuotes = false;
                }
                else if (seperators.Contains(chr) && !inQuotes && !seperatorFound)
                {
                    result.Add(currentWord);
                    currentWord = "";
                    seperatorFound = true;
                    wordFound = false;
                }
                else if (!seperators.Contains(chr) && seperatorFound && !inQuotes)
                {
                    seperatorFound = false;
                    wordFound = true;
                }
                else if (!seperators.Contains(chr) && !wordFound && !inQuotes)
                {
                    wordFound = true;
                }
                if (!illegalChars.Contains(chr) || inQuotes)
                    currentWord += chr;
                currentPosition++;
            }
            if (wordFound)
                result.Add(currentWord);

            if (inQuotes)
                result.Add(currentWord);

            return result;
        }
    }
}
