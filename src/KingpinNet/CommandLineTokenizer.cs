using System.Collections.Generic;
using System.Linq;

namespace KingpinNet;

public class CommandLineTokenizer
{
    private const char Space = ' ';
    private const char Tab = '\t';
    private const char Carriage = '\r';
    private const char Newline = '\n';
    private const char Quotes = '"';
    private readonly char[] illegalChars = [Space, Tab, Carriage, Newline, Quotes];

    private readonly char[] separators = [Space, Tab, Carriage, Newline];
    private bool inQuotes;
    private bool separatorFound;
    private bool wordFound;

    public List<string> ToTokens(string commandLine)
    {
        var result = new List<string>();
        var currentWord = "";
        var trimmedCommandLine = commandLine.TrimStart();
        foreach (var chr in trimmedCommandLine)
        {
            switch (chr)
            {
                case Quotes when !inQuotes && separatorFound:
                    inQuotes = true;
                    continue;
                case Quotes when inQuotes:
                    result.Add(currentWord);
                    currentWord = "";
                    inQuotes = false;
                    break;
                default:
                {
                    if (separators.Contains(chr) && !inQuotes && !separatorFound)
                    {
                        result.Add(currentWord);
                        currentWord = "";
                        separatorFound = true;
                        wordFound = false;
                    }
                    else if (!separators.Contains(chr) && separatorFound && !inQuotes)
                    {
                        separatorFound = false;
                        wordFound = true;
                    }
                    else if (!separators.Contains(chr) && !wordFound && !inQuotes)
                    {
                        wordFound = true;
                    }

                    break;
                }
            }

            if (!illegalChars.Contains(chr) || inQuotes)
                currentWord += chr;
        }

        if (wordFound)
            result.Add(currentWord);

        if (inQuotes)
            result.Add(currentWord);

        return result;
    }
}
