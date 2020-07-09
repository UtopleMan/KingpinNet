using System;
using System.Collections.Generic;
using System.Text;

namespace KingpinNet
{
    public class LevenshteinSuggestionCalculator : ISuggestionCalculator
    {
        private int Compute(string string1, string string2)
        {
            var length1 = string1.Length;
            var lenght2 = string2.Length;
            var resultArray = new int[length1 + 1, lenght2 + 1];

            if (length1 == 0) return lenght2;
            if (lenght2 == 0) return length1;

            for (var i = 0; i <= length1; resultArray[i, 0] = i++) { }
            for (var j = 0; j <= lenght2; resultArray[0, j] = j++) { }

            for (var i = 1; i <= length1; i++)
            {
                for (var j = 1; j <= lenght2; j++)
                {
                    var cost = (string2[j - 1] == string1[i - 1]) ? 0 : 1;
                    resultArray[i, j] = Math.Min(
                        Math.Min(resultArray[i - 1, j] + 1, resultArray[i, j - 1] + 1),
                        resultArray[i - 1, j - 1] + cost);
                }
            }
            return resultArray[length1, lenght2];
        }
    }

    public interface ISuggestionCalculator
    {

    }
}
