using System;

namespace KingpinNet.UI
{
    public class Spinner : WidgetBase
    {
        const string twirl = "-\\|/";
        private int current = 0;
        private SpinnerConfig config = new SpinnerConfig {
            Style = new Twirl(),
            UseColor = false,
            Foreground = ConsoleColor.White,
            Background = ConsoleColor.Black
        };
        private ConsoleColor currentForegroundColor;
        private ConsoleColor currentBackgroundColor;

        public Spinner(IConsole console, Action<SpinnerConfig> configure = null) : base(console)
        {
            configure?.Invoke(config);
            SetConfig(config);
        }
        protected override void Draw()
        {
            var currentPos = console.CursorLeft;
            if (config.UseColor)
            {
                currentForegroundColor = console.ForegroundColor;
                currentBackgroundColor = console.BackgroundColor;
            }

            console.Write(config.Style.Items[current++ % config.Style.Items.Length]);
            if (config.UseColor)
            {
                console.ForegroundColor = currentForegroundColor;
                console.BackgroundColor = currentBackgroundColor;
            }
            console.CursorLeft = currentPos;
            console.CursorVisible = true;
        }
    }
    public class SpinnerConfig : IConfigBase
    {
        public ISpinnerStyleContainer Style { get; set; }

        public bool UseColor { get; set; }

        public ConsoleColor Foreground { get; set; }

        public ConsoleColor Background { get; set; }
    }

    public class Twirl : ISpinnerStyleContainer
    {
        public char[] Items => new[] { '-', '\\', '|', '/' };
    }
    public class Spin : ISpinnerStyleContainer
    {
        public char[] Items => new[] { '-','\\','|','/' };
    }
    public interface ISpinnerStyleContainer
    {
        public char[] Items { get; }
    }
}
