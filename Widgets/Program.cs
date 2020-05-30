using KingpinNet.UI;
using KingpinNet.UI.DataTable;
using KingpinNet.UI.ProgressBar;
using KingpinNet.UI.Spinner;
using KingpinNet.UI.Window;
using System;
using System.Threading;

namespace Widgets
{
    class Program
    {
        static void Main(string[] args)
        {
            var console = new KingpinNet.UI.Console();
            console.BeginRendering();
            try
            {
                var data = new[] {
                    new { Column1 = 1234.56, Column2 = "23", ColumnWithLongLength = ConsoleColor.Cyan },
                    new { Column1 = 1234.56, Column2 = "wsergfwer", ColumnWithLongLength = ConsoleColor.Blue },
                    new { Column1 = 1234.56, Column2 = "ref", ColumnWithLongLength = ConsoleColor.DarkCyan },
                    new { Column1 = 1234.5656, Column2 = "234523", ColumnWithLongLength = ConsoleColor.DarkYellow },
                    new { Column1 = 1234.56, Column2 = "ferfgbfgh", ColumnWithLongLength = ConsoleColor.Blue },
                    new { Column1 = 1234.56, Column2 = "&/(&%/¤", ColumnWithLongLength = ConsoleColor.DarkGreen }
                };

                var dataTable = new DataTable(console, config => { });
                dataTable.Update(data);

                var window = new Window(console, config =>
                {
                    config.Width = 50;
                    config.Height = 15;
                    config.Title = "Hej med dig";
                    config.TitleColor = ConsoleColor.White;
                    config.UseColor = true;
                    config.BackgroundColor = ConsoleColor.Blue;
                    config.ForegroundColor = ConsoleColor.Cyan;
                    config.Style = new Fat();
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

                var spinner = new Spinner(console, config => { config.Style = new Spin(); });

                for (var i = 0; i < 100; i++)
                {
                    spinner.Render();
                    Thread.Sleep(100);
                }


            }
            finally
            {
                console.EndRendering();
            }
            console.ReadLine();
        }
    }
}
