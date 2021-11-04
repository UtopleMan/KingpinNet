using System;
using System.IO;

namespace KingpinNet
{
    public class DefaultConsole : IConsole
    {
        public TextWriter Out
        {
            get
            {
                return Console.Out;
            }
        }
    }
}
