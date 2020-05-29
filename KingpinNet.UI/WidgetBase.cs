using System;

namespace KingpinNet.UI
{
    public abstract class WidgetBase
    {
        protected IConsole console;
        private IConfigBase config;
        private ConsoleColor currentForeground;
        private ConsoleColor currentBackground;

        public WidgetBase(IConsole console)
        {
            this.console = console;
        }

        public void SetConfig(IConfigBase config)
        {
            this.config = config;
        }
        public void Render()
        {
            var currentLeft = console.CursorLeft;
            var currentTop = console.CursorTop;
            console.CursorVisible = false;

            if (config.UseColor)
            {
                currentForeground = console.ForegroundColor;
                currentBackground = console.BackgroundColor;
                console.ForegroundColor = config.Foreground;
                console.BackgroundColor = config.Background;
            }
            try
            {
                Draw();
            }
            finally
            {
                if (config.UseColor)
                {
                    console.ForegroundColor = currentForeground;
                    console.BackgroundColor = currentBackground;
                }
                console.SetCursorPosition(currentLeft, currentTop);
                console.CursorVisible = true;

            }
        }

        protected abstract void Draw();
    }

    public interface IConfigBase
    {
        public bool UseColor { get;  }
        public ConsoleColor Foreground { get; }
        public ConsoleColor Background { get; }
    }
}
