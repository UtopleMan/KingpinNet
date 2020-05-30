using System;

namespace KingpinNet.UI.Window
{
/*
88        88                                     88              
88        ""                                     ""              
88   ,d8  88 8b,dPPYba,   ,adPPYb,d8 8b,dPPYba,  88 8b,dPPYba,   
88 ,a8"   88 88P'   `"8a a8"    `Y88 88P'    "8a 88 88P'   `"8a  
8888[     88 88       88 8b       88 88       d8 88 88       88  
88`"Yba,  88 88       88 "8a,   ,d88 88b,   ,a8" 88 88       88  
88   `Y8a 88 88       88  `"YbbdP"Y8 88`YbbdP"'  88 88       88  
                          aa,    ,88 88                          
                           "Y8bbdP"  88 
*/
    // https://en.wikipedia.org/wiki/Box-drawing_character
    public class Window : WidgetParentBase
    {
        private WindowConfig config = new WindowConfig
        {
            Title = "Window",
            UseColor = false,
            BackgroundColor = ConsoleColor.Black,
            ForegroundColor = ConsoleColor.Gray,
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
                        console.Write(config.Style.TopHorizontal);
                    }
                    else if (y == config.Top + config.Height - 1)
                    {
                        console.Write(config.Style.BottomHorizontal);
                    }
                    else if (x == config.Left)
                    {
                        console.Write(config.Style.LeftVertical);
                    }
                    else if (x == config.Left + config.Width - 1)
                    {
                        console.Write(config.Style.RightVertical);
                    }
                    else if (x == config.Left + 1)
                    {
                        var line = "".PadRight(config.Width - 2);
                        console.Write(line);
                    }
                }
            }
            var titleLength = (config.Style.StartTitle + config.Title + config.Style.EndTitle).Length;
            console.SetCursorPosition(config.Left + ((config.Width - titleLength) / 2), config.Top);
            console.Write(config.Style.StartTitle);
            console.ForegroundColor = config.TitleColor;
            console.Write(config.Title);
            console.ForegroundColor = config.ForegroundColor;
            console.Write(config.Style.EndTitle);

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
        public char TopHorizontal => '─';
        public char BottomHorizontal => '─';
        public char LeftVertical => '│';
        public char RightVertical => '│';
    }

    public class DoubleLine : IWindowStyleContainer
    {
        public char LeftTop => '╔';

        public char RightTop => '╗';

        public char LeftBottom => '╚';

        public char RightBottom => '╝';

        public string StartTitle => "╡ ";

        public string EndTitle => " ╞";
        public char TopHorizontal => '═';
        public char BottomHorizontal => '═';
        public char LeftVertical => '║';
        public char RightVertical => '║';
    }

    public class Fat : IWindowStyleContainer
    {
        public char LeftTop => '▛';

        public char RightTop => '▜';

        public char LeftBottom => '▙';

        public char RightBottom => '▟';

        public string StartTitle => "▘ ";

        public string EndTitle => " ▝";
        public char TopHorizontal => '▀';
        public char BottomHorizontal => '▄';
        public char LeftVertical => '▌';
        public char RightVertical => '▐';
    }

    public interface IWindowStyleContainer
    {
        char LeftTop { get; }
        char RightTop { get; }
        char LeftBottom { get; }
        char RightBottom { get; }
        string StartTitle { get; }
        string EndTitle { get; }
        char TopHorizontal { get; }
        char BottomHorizontal { get; }
        char LeftVertical { get; }
        char RightVertical { get; }
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
        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor TitleColor { get; set; }
    }
}
