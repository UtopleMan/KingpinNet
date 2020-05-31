using System;

namespace KingpinNet.UI.Spinner
{
    public class Spinner : WidgetBase
    {
        private int current = 0;
        private SpinnerConfig config = new SpinnerConfig {
            Style = new Spin(),
            UseColor = false,
            BackgroundColor = ConsoleColor.Black,
            ForegroundColor = ConsoleColor.Gray
        };

        public Spinner(IConsole console, Action<SpinnerConfig> configure = null) : base(console)
        {
            configure?.Invoke(config);
            SetConfig(config);
        }
        protected override void Draw()
        {
            console.Write(config.Style.Items[current++ % config.Style.Items.Length]);
        }
    }
    public class SpinnerConfig : IConfigBase
    {
        public ISpinnerStyleContainer Style { get; set; }

        public bool UseColor { get; set; }

        public ConsoleColor ForegroundColor { get; set; }

        public ConsoleColor BackgroundColor { get; set; }
    }

    public class Fade : ISpinnerStyleContainer
    {
        public char[] Items => new[] { '░', '▒', '▓', '█', '▓', '▒', '░', ' ' };
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
