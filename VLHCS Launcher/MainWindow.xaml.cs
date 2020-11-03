using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Timers;

namespace VLHCS_Launcher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string path = Directory.GetCurrentDirectory() + "\\hlds.exe";
        //string path = "G:\\GAMES\\SteamLibrary\\steamapps\\common\\Half-Life\\hlds.exe";
        string args = "-console -game cstrike +maxplayers 8 +map de_dust";
        string processName = "hlds";

        public MainWindow()
        {
            InitializeComponent();

            if(!File.Exists(path))
            {
                MessageBox.Show($"Не найден HLDS.exe!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                Environment.Exit(0);
                return;
            }
        }

        private void Button_Click_Exit(object sender, RoutedEventArgs e)
        {
            ProcessClose(processName);
            Environment.Exit(0);
        }
        private void Button_Click_Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_Click_StartServer(object sender, RoutedEventArgs e)
        {
            CreateHLDS(path, args);

            var timer = new Timer(5);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ProcessHide(processName);
        }

        private void CreateHLDS(string path, string args)
        {
            try {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = path,
                        Arguments = args,
                        UseShellExecute = false,
                        WorkingDirectory = System.IO.Path.GetDirectoryName(path),
                    }
                };

                process.Start();
                process.WaitForInputIdle();
            }
            catch (Exception err)
            {
                MessageBox.Show($"{err.Message} File: `{path}`", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessClose(string name)
        {
            Process[] processRunning = Process.GetProcesses();
            foreach (Process pr in processRunning)
            {
                if (pr.ProcessName == name)
                {
                    pr.CloseMainWindow();
                    pr.Close();
                }
            }
        }

        private const int SW_HIDE = 0;
        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        private void ProcessHide(string name)
        {
            Process[] processRunning = Process.GetProcesses();
            foreach (Process pr in processRunning)
            {
                if(pr.ProcessName == name)
                {
                    var hWnd = pr.MainWindowHandle.ToInt32();
                    ShowWindow(hWnd, SW_HIDE);
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }
    }
}
