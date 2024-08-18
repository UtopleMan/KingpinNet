using System.Collections.Generic;

namespace KingpinNet;
public class ParseResult
{
    public ParseResult()
    {
        Result = new Dictionary<string, string>();
        Suggestions = new List<string>();
        IsSuggestion = false;
    }
    public Dictionary<string, string> Result { get; }
    public List<string> Suggestions { get; }
    public bool IsSuggestion { get; set; }
}
