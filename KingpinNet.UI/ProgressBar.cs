using System;

namespace KingpinNet.UI
{
/*
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
    public class ProgressBar : WidgetBase
    {
        private int currentCount = 0;
        private int lastWhole = -1;
        private ProgressBarConfig config = new ProgressBarConfig
        {
            BarWidth = 30,
            Style = new Boxes(),
            UseColor = false,
            BackgroundColor = ConsoleColor.Black,
            ForegroundColor = ConsoleColor.Gray
        };

        public ProgressBar(IConsole console, Action<ProgressBarConfig> configure = null) : base(console)
        {
            configure?.Invoke(config);
            SetConfig(config);
        }
        public void SetProgress(int count)
        {
            currentCount = count;
            Render();
        }

        public void Increment()
        {
            currentCount++;
            Render();
        }

        protected override void Draw()
        {
            double percent = currentCount / (config.ItemCount / 100.0);

            if ((int)percent <= lastWhole)
            {
                return;
            }
            lastWhole = (int)percent;

            console.Write(config.Style.Begin, percent);
            var progress = (int)(((percent / 100) * (float) config.BarWidth * config.Style.OnItems.Length) + .5f);
            for (var i = 0; i < config.BarWidth * config.Style.OnItems.Length; ++i)
            {
                if (i >= progress)
                {
                    if (i % config.Style.OnItems.Length == 0)
                        console.Write(config.Style.Off);
                }
                else
                {
                    var fullColor = (progress / config.Style.OnItems.Length) * config.Style.OnItems.Length;
                    if (i < fullColor)
                    {
                        if (i % config.Style.OnItems.Length == 0)
                            console.Write(config.Style.OnItems[config.Style.OnItems.Length - 1]);
                    }
                    else
                    { 
                        console.Write(config.Style.OnItems[progress % config.Style.OnItems.Length]);
                        i += config.Style.OnItems.Length - 1;
                    }
                }
            }
            console.Write(config.Style.End, percent);
        }
    }
    public class ProgressBarConfig : IConfigBase
    {
        public int ItemCount { get; set; }
        public int BarWidth { get; set; }
        public bool UseColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public IProgressBarStyleContainer Style { get; set; }
    }

    public interface IProgressBarStyleContainer
    {
        char[] OnItems { get; }
        char Off { get; }
        string Begin { get; }
        string End { get; }
    }

    public class Boxes : IProgressBarStyleContainer
    {
        public char[] OnItems => new [] { '⬛' };
        public char Off => '⬜';
        public string Begin => "[";
        public string End => "] {0,3:##0}%";
    }
    public class Dots1 : IProgressBarStyleContainer
    {
        public char[] OnItems => new[] { '⣄', '⣆', '⣇', '⣧', '⣷', '⣿' };
        public char Off => '⣀';
        public string Begin => "[";
        public string End => "] {0,3:##0}%";
    }
    public class GreyScale : IProgressBarStyleContainer
    {
        public char[] OnItems => new[] { '░', '▒', '▓', '█' };

        public char Off => ' ';

        public string Begin => "Progress: {0,3:##0}% |";

        public string End => "|";
    }
}
