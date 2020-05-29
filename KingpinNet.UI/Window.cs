using System;

namespace KingpinNet.UI
{
    public class Window : WidgetParentBase
    {
        private WindowConfig config = new WindowConfig
        {
            Title = "Window",
            UseColor = false,
            Foreground = ConsoleColor.White,
            Background = ConsoleColor.Black,
            Style = new SingleLine(),
            Left = 10,
            Top = 10,
            Width = 40,
            Height = 30
        };

        public Window(IConsole console, Action<WindowConfig> configure = null) : base(console)
        {
            configure?.Invoke(config);
            SetConfig(config);
        }

        protected override void Draw()
        {
            var title = config.Style.StartTitle + config.Title + config.Style.EndTitle;
            for (var y = config.Top; y < config.Top + config.Height; y++)
            {
                for (var x = config.Left; x < config.Left + config.Width; x++)
                {
                    console.SetCursorPosition(x, y);
                    if (x == config.Left && y == config.Top)
                    {
                        console.Write(config.Style.LeftTop);
                    }
                    else if (x == config.Left + config.Width - 1 && y == config.Top)
                    {
                        console.Write(config.Style.RightTop);
                    }
                    else if (x == config.Left && y == config.Top + config.Height - 1)
                    {
                        console.Write(config.Style.LeftBottom);
                    }
                    else if (x == config.Left + config.Width - 1 && y == config.Top + config.Height - 1)
                    {
                        console.Write(config.Style.RightBottom);
                    }
                    else if (y == config.Top)
                    {
                        console.Write(config.Style.Horizontal);
                    }
                    else if (y == config.Top + config.Height - 1)
                    {
                        console.Write(config.Style.Horizontal);
                    }
                    else if (x == config.Left)
                    {
                        console.Write(config.Style.Vertical);
                    }
                    else if (x == config.Left + config.Width - 1)
                    {
                        console.Write(config.Style.Vertical);
                    }
                    else if (x == config.Left + 1)
                    {
                        var line = "".PadRight(config.Width - 2);
                        console.Write(line);
                    }
                }
            }
            console.SetCursorPosition(config.Left + ((config.Width - title.Length) / 2), config.Top);
            console.Write(title);

            base.Draw();
        }
    }

    public class SingleLine : IWindowStyleContainer
    {
        public char LeftTop => '┌';

        public char RightTop => '┐';

        public char LeftBottom => '└';

        public char RightBottom => '┘';

        public string StartTitle => "┤ ";

        public string EndTitle => " ├";
        public char Horizontal => '─';
        public char Vertical => '│';
    }

    public class DoubleLine : IWindowStyleContainer
    {
        public char LeftTop => '╔';

        public char RightTop => '╗';

        public char LeftBottom => '╚';

        public char RightBottom => '╝';

        public string StartTitle => "╣ ";

        public string EndTitle => " ╠";
        public char Horizontal => '═';
        public char Vertical => '║';
    }

    public interface IWindowStyleContainer
    {
        char LeftTop { get; }
        char RightTop { get; }
        char LeftBottom { get; }
        char RightBottom { get; }
        string StartTitle { get; }
        string EndTitle { get; }
        char Horizontal { get; }
        char Vertical { get; }
    }

    public class WindowConfig : IConfigBase
    {
        public string Title { get; set; }
        public IWindowStyleContainer Style { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public bool UseColor { get; set; }

        public ConsoleColor Foreground { get; set; }

        public ConsoleColor Background { get; set; }
    }
}
