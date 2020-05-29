using KingpinNet.UI;
using System;
using System.Threading;

namespace Widgets
{
    class Program
    {
        static void Main(string[] args)
        {
            var console = new KingpinNet.UI.Console();
            var window = new Window(console, config =>
            {
                config.Width = 50;
                config.Height = 15;
                config.Title = "Hej med dig";
                config.UseColor = true;
                config.Background = ConsoleColor.Blue;
                config.Foreground = ConsoleColor.Cyan;
                config.Style = new DoubleLine();
            });
            window.Render();

            var progressBar = new ProgressBar(console, config =>
            {
                config.ItemCount = 100;
                config.Style = new GreyScale();
            });

            for (var i = 0; i < 100; i++)
            {
                progressBar.Increment();
                Thread.Sleep(100);

            }

            var spinner = new Spinner(console);

            for (var i = 0; i < 100; i++)
            {
                spinner.Render();
                Thread.Sleep(100);
            }
            console.ReadLine();
        }
    }
}
