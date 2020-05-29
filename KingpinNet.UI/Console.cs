using System;
using System.IO;
using System.Text;

namespace KingpinNet.UI
{
    /*
 * https://en.wikipedia.org/wiki/Box-drawing_character
 * 
var bar_styles = [
'▁▂▃▄▅▆▇█',
'⣀⣄⣤⣦⣶⣷⣿',
'⣀⣄⣆⣇⣧⣷⣿',
'○◔◐◕⬤',
'□◱◧▣■',
'□◱▨▩■',
'□◱▥▦■',
'░▒▓█',
'░█',
'⬜⬛',
'▱▰',
'▭◼',
'▯▮',
'◯⬤',
'⚪⚫',
];
 */
    public class Console : IConsole
    {
        public Console()
        {
            System.Console.OutputEncoding = Encoding.UTF8;
        }
        public bool IsInputRedirected => System.Console.IsInputRedirected;

        public int BufferHeight { get => System.Console.BufferHeight; set => System.Console.BufferHeight = value; }
        public int BufferWidth { get => System.Console.BufferWidth; set => System.Console.BufferWidth = value; }

        public bool CapsLock => System.Console.CapsLock;

        public int CursorLeft { get => System.Console.CursorLeft; set => System.Console.CursorLeft = value; }
        public int CursorSize { get => System.Console.CursorSize; set => System.Console.CursorSize = value; }
        public int CursorTop { get => System.Console.CursorTop; set => System.Console.CursorTop = value; }
        public bool CursorVisible { get => System.Console.CursorVisible; set => System.Console.CursorVisible = value; }

        public TextWriter Error => System.Console.Error;

        public ConsoleColor ForegroundColor { get => System.Console.ForegroundColor; set => System.Console.ForegroundColor = value; }

        public TextReader In => System.Console.In;

        public Encoding InputEncoding { get => System.Console.InputEncoding; set => System.Console.InputEncoding = value; }

        public bool IsErrorRedirected => System.Console.IsErrorRedirected;

        public int WindowWidth { get => System.Console.WindowWidth; set => System.Console.WindowWidth = value; }

        public bool IsOutputRedirected => System.Console.IsOutputRedirected;

        public bool KeyAvailable => System.Console.KeyAvailable;

        public int LargestWindowHeight => System.Console.LargestWindowHeight;

        public int LargestWindowWidth => System.Console.LargestWindowWidth;

        public bool NumberLock => System.Console.NumberLock;

        public TextWriter Out => System.Console.Out;

        public Encoding OutputEncoding { get => System.Console.OutputEncoding; set => System.Console.OutputEncoding = value; }
        public string Title { get => System.Console.Title; set => System.Console.Title = value; }
        public bool TreatControlCAsInput { get => System.Console.TreatControlCAsInput; set => System.Console.TreatControlCAsInput = value; }
        public int WindowHeight { get => System.Console.WindowHeight; set => System.Console.WindowHeight = value; }
        public int WindowLeft { get => System.Console.WindowLeft; set => System.Console.WindowLeft = value; }
        public int WindowTop { get => System.Console.WindowTop; set => System.Console.WindowTop = value; }
        public ConsoleColor BackgroundColor { get => System.Console.BackgroundColor; set => System.Console.BackgroundColor = value; }

        public event ConsoleCancelEventHandler CancelKeyPress
        {
            add { System.Console.CancelKeyPress += value; }
            remove { System.Console.CancelKeyPress -= value; }
        }
        public void Beep()
        {
            System.Console.Beep();
        }

        public void Beep(int frequency, int duration)
        {
            System.Console.Beep(frequency, duration);
        }

        public void Clear()
        {
            System.Console.Clear();
        }

        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
        {
            System.Console.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop);
        }

        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor)
        {
            System.Console.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop, sourceChar, sourceForeColor, sourceBackColor);
        }

        public Stream OpenStandardError(int bufferSize)
        {
            return System.Console.OpenStandardError(bufferSize);
        }

        public Stream OpenStandardError()
        {
            return System.Console.OpenStandardError();
        }

        public Stream OpenStandardInput(int bufferSize)
        {
            return System.Console.OpenStandardInput(bufferSize);
        }

        public Stream OpenStandardInput()
        {
            return System.Console.OpenStandardInput();
        }

        public Stream OpenStandardOutput(int bufferSize)
        {
            return System.Console.OpenStandardOutput(bufferSize);
        }

        public Stream OpenStandardOutput()
        {
            return System.Console.OpenStandardOutput();
        }

        public int Read()
        {
            return System.Console.Read();
        }

        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            return System.Console.ReadKey(intercept);
        }

        public ConsoleKeyInfo ReadKey()
        {
            return System.Console.ReadKey();
        }

        public string ReadLine()
        {
            return System.Console.ReadLine();
        }

        public void ResetColor()
        {
            System.Console.ResetColor();
        }

        public void SetBufferSize(int width, int height)
        {
            System.Console.SetBufferSize(width, height);
        }

        public void SetCursorPosition(int left, int top)
        {
            System.Console.SetCursorPosition(left, top);
        }

        public void SetError(TextWriter newError)
        {
            System.Console.SetError(newError);
        }

        public void SetIn(TextReader newIn)
        {
            System.Console.SetIn(newIn);
        }

        public void SetOut(TextWriter newOut)
        {
            System.Console.SetOut(newOut);
        }

        public void SetWindowPosition(int left, int top)
        {
            System.Console.SetWindowPosition(left, top);
        }

        public void SetWindowSize(int width, int height)
        {
            System.Console.SetWindowSize(width, height);
        }

        public void Write(ulong value)
        {
            System.Console.Write(value);
        }

        public void Write(bool value)
        {
            System.Console.Write(value);
        }

        public void Write(char value)
        {
            System.Console.Write(value);
        }

        public void Write(char[] buffer)
        {
            System.Console.Write(buffer);
        }

        public void Write(char[] buffer, int index, int count)
        {
            System.Console.Write(buffer, index, count);
        }

        public void Write(double value)
        {
            System.Console.Write(value);
        }

        public void Write(long value)
        {
            System.Console.Write(value);
        }

        public void Write(object value)
        {
            System.Console.Write(value);
        }

        public void Write(float value)
        {
            System.Console.Write(value);
        }

        public void Write(string value)
        {
            System.Console.Write(value);
        }

        public void Write(string format, object arg0)
        {
            System.Console.Write(format, arg0);
        }

        public void Write(string format, object arg0, object arg1)
        {
            System.Console.Write(format, arg0, arg1);
        }

        public void Write(string format, object arg0, object arg1, object arg2)
        {
            System.Console.Write(format, arg0, arg1, arg2);
        }

        public void Write(string format, params object[] arg)
        {
            System.Console.Write(format, arg);
        }

        public void Write(uint value)
        {
            System.Console.Write(value);
        }

        public void Write(decimal value)
        {
            System.Console.Write(value);
        }

        public void Write(int value)
        {
            System.Console.Write(value);
        }

        public void WriteLine(ulong value)
        {
            System.Console.WriteLine(value);
        }

        public void WriteLine(string format, params object[] arg)
        {
            System.Console.WriteLine(format, arg);
        }

        public void WriteLine()
        {
            System.Console.WriteLine();
        }

        public void WriteLine(bool value)
        {
            System.Console.WriteLine(value);
        }

        public void WriteLine(char[] buffer)
        {
            System.Console.WriteLine(buffer);
        }

        public void WriteLine(char[] buffer, int index, int count)
        {
            System.Console.WriteLine(buffer, index, count);
        }

        public void WriteLine(decimal value)
        {
            System.Console.WriteLine(value);
        }

        public void WriteLine(double value)
        {
            System.Console.WriteLine(value);
        }

        public void WriteLine(uint value)
        {
            System.Console.WriteLine(value);
        }

        public void WriteLine(int value)
        {
            System.Console.WriteLine(value);
        }

        public void WriteLine(object value)
        {
            System.Console.WriteLine(value);
        }

        public void WriteLine(float value)
        {
            System.Console.WriteLine(value);
        }

        public void WriteLine(string value)
        {
            System.Console.WriteLine(value);
        }

        public void WriteLine(string format, object arg0)
        {
            System.Console.WriteLine(format, arg0);
        }

        public void WriteLine(string format, object arg0, object arg1)
        {
            System.Console.WriteLine(format, arg0, arg1);
        }

        public void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            System.Console.WriteLine(format, arg0, arg1, arg2);
        }

        public void WriteLine(long value)
        {
            System.Console.WriteLine(value);
        }

        public void WriteLine(char value)
        {
            System.Console.WriteLine(value);
        }
    }
}
