namespace KingpinNet.UI
{
    public class Spinner : WidgetBase
    {
        const string _twirl = "-\\|/";
        private int current = 0;
        public Spinner(IConsole console) : base(console)
        {
        }
        protected override void Draw()
        {
            var currentPos = console.CursorLeft;
            console.Write(_twirl[current++ % _twirl.Length]);
            console.CursorLeft = currentPos;
            console.CursorVisible = true;
        }
    }
}
