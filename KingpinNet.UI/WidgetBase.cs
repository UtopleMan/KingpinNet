namespace KingpinNet.UI
{
    public abstract class WidgetBase
    {
        protected IConsole console;

        public WidgetBase(IConsole console)
        {
            this.console = console;

        }
        public void Render()
        {
            var currentLeft = console.CursorLeft;
            var currentTop = console.CursorTop;
            console.CursorVisible = false;
            try
            {
                Draw();
            }
            finally
            {
                console.SetCursorPosition(currentLeft, currentTop);
                console.CursorVisible = true;
            }
        }

        protected abstract void Draw();
    }
}
