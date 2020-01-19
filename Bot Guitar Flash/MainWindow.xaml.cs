using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Forms.Integration;
using WindowsInput;
using WindowsInput.Native;
using Gecko;
using Point = System.Windows.Point;

namespace Everaldo.BotGuitarFlash
{
    public partial class MainWindow : Window
    {
        private GeckoWebBrowser browser;
        private WindowsFormsHost host;
        private readonly InputSimulator inputSimulator;
        private Thread botThread;

        public MainWindow()
        {
            InitializeComponent();

            Xpcom.Initialize("Firefox");
            inputSimulator = new InputSimulator();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            host = new WindowsFormsHost();
            browser = new GeckoWebBrowser();
            host.Child = browser;
            MainGrid.Children.Add(host);

            browser.Navigate("https://guitarflash.com");
        }

        private void StartBot_OnClick(object sender, RoutedEventArgs e)
        {
            browser.Window.ScrollTo(0, 360);

            var image = new Bitmap(host.Child.Width, host.Child.Height);
            
            botThread?.Abort();
            botThread = new Thread(() =>
            {
                while (true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        
                        using (var gfx = Graphics.FromImage(image))
                        {
                            var p = window.PointToScreen(new Point(0, 0));
                            gfx.CopyFromScreen((int) p.X,
                                (int) p.Y,
                                0, 0,
                                host.Child.ClientRectangle.Size,
                                CopyPixelOperation.SourceCopy);
                        }

                        var greenPixel = image.GetPixel(298, 575);
                        if (greenPixel.G > 100 && greenPixel.B < 100 && greenPixel.R < 100)
                        {
                            Console.WriteLine("VERDE");
                            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_A);
                        }

                        var redPixel = image.GetPixel(391, 575);
                        if (redPixel.R > 100 && redPixel.G < 100 && redPixel.B < 100)
                        {
                            Console.WriteLine("VERMELHO");
                            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_S);
                        }

                        var yellowPixel = image.GetPixel(485, 575);
                        if (yellowPixel.B < 10 && yellowPixel.R > 200 && yellowPixel.G > 200)
                        {
                            Console.WriteLine("AMARELO");
                            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_J);
                        }

                        var bluePixel = image.GetPixel(579, 575);
                        if (bluePixel.B > 100 && bluePixel.R < 100 && bluePixel.G > 100)
                        {
                            Console.WriteLine("AZUL");
                            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_K);
                        }

                        var orangePixel = image.GetPixel(670, 575);
                        if (orangePixel.B < 100 && orangePixel.R > 200 && orangePixel.G > 90)
                        {
                            Console.WriteLine("LARANJA");
                            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.VK_L);
                        }
                    });

                    Thread.Sleep(6);
                }
            });

            botThread.Start();
        }
        private void StopBot_OnClick(object sender, RoutedEventArgs e)
        {
            botThread?.Abort();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            botThread?.Abort();
        }
    }
}