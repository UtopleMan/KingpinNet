namespace KingpinNet
{
    public static class StringExtensions
    {
        public static string[] SplitFirst(this string item, string find)
        {
            var index = item.IndexOf(find);
            if (index <= -1 || index >= item.Length)
                return new string[] { item };
            var firstSubString = item.Substring(0, index);
            var secondSubString = item.Substring(index + 1, item.Length - index - 1);
            return new string[] { firstSubString, secondSubString };
        }

        public static string[] SplitFirst(this string item, char find)
        {
            return item.SplitFirst(find.ToString());
        }
    }
}
