using System.Collections.Generic;

namespace KingpinNet
{
    public class ParseResult
    {
        public ParseResult()
        {
            Result = new Dictionary<string, string>();
            Completions = new List<string>();
            Errors = new List<string>();
            IsCompletion = false;
        }
        public Dictionary<string, string> Result { get; }
        public List<string> Completions { get; }
        public bool IsCompletion { get; internal set; }
        public string Suggestion { get; internal set; }
        public bool HasErrors { get; internal set; }
        public string ErrorMessage { get; internal set; }
        public List<string> Errors { get; }
    }
}
