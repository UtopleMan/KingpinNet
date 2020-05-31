using System;

namespace KingpinNet.UI
{
    public abstract class WidgetBase
    {
        protected IConsole console;
        private IConfigBase config;
        private ConsoleColor currentForeground;
        private ConsoleColor currentBackground;
        private int currentLeft;
        private int currentTop;

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
            console.BeginRendering();
            currentLeft = console.CursorLeft;
            currentTop = console.CursorTop;
            if (config.UseColor)
            {
                currentForeground = console.ForegroundColor;
                currentBackground = console.BackgroundColor;
                console.ForegroundColor = config.ForegroundColor;
                console.BackgroundColor = config.BackgroundColor;
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
                console.EndRendering();
            }
        }

        protected abstract void Draw();
    }

    public interface IConfigBase
    {
        public bool UseColor { get;  }
        public ConsoleColor ForegroundColor { get; }
        public ConsoleColor BackgroundColor { get; }
    }
}
