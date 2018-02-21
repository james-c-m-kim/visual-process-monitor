using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace VisualProcessMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr hwnd;
        private Process proc;
        private IDisposable subscription;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Capture_Click(object sender, RoutedEventArgs e)
        {
            var bmp = ScreenCaptureUtil.PrintWindow(hwnd);

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var saveFile = Path.Combine(folder, "screencap.png");
            if (File.Exists(saveFile))
            {
                File.Delete(saveFile);
            }

            bmp.Save(saveFile, ImageFormat.Png);
        }

        private void Process_Start_Click(object sender, RoutedEventArgs e)
        {
            proc = Process.Start("cmd.exe");

            // async task to pick up the mainwindowhandle because:
            // the mainwindowhandle gets updated after the console window is created...
            // yarp.
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);

                if (proc.MainWindowHandle != IntPtr.Zero)
                {
                    hwnd = proc.MainWindowHandle;
                    btnCapture.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        new Action(() =>
                        {
                            btnCapture.IsEnabled = true;
                        }));
                }
            });
        }


    }
}
