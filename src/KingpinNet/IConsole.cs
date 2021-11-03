using System.IO;

namespace KingpinNet
{
    public interface IConsole
    {
        void Write(string text);
        void WriteLine(string text);
    }
}
