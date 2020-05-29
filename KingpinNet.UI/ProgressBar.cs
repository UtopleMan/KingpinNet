namespace KingpinNet.UI
{
    public class ProgressBar : WidgetBase
    {
        private int maxCount;
        private int barWidth;
        private int currentCount;
        private int lastWhole = 0;

        public ProgressBar(IConsole console, int maxCount, int barWidth) : base(console)
        {
            this.maxCount = maxCount;
            this.barWidth = barWidth;
            this.currentCount = 0;
        }
        public void SetProgress(int count)
        {
            currentCount = count;
            Render();
        }

        protected override void Draw()
        {
            double percent = currentCount / (maxCount / 100.0);

            if ((int)percent <= lastWhole)
            {
                return;
            }
            lastWhole = (int)percent;

            console.Write("[");
            var p = (int)(((percent / 100) * (float)barWidth) + .5f);
            for (var i = 0; i < barWidth; ++i)
            {
                if (i >= p)
                    console.Write('⬜');
                else
                    console.Write('⬛');
            }
            console.Write("] {0,3:##0}%", percent);
        }
    }
}
