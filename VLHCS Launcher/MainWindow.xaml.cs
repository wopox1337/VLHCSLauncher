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
using System.Threading;

namespace VLHCS_Launcher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SynchronizationContext _syncContext;

        public MainWindow()
        {
            InitializeComponent();
            _syncContext = SynchronizationContext.Current;


        }

        void Display(string output)
        {
            _syncContext.Post(_ => ConsoleData.Text += $"{output}\n", null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path = Directory.GetCurrentDirectory() + "\\hlds.exe";
            string args = "-console -game cstrike +maxplayers 8 +map de_dust";

            CreateHLDS(path, args);
            ConsoleData.Text = "";

            var proc_HLDS = Find_Proc("HLDS");
        }

        private void CreateHLDS(string path, string args)
        {
            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = path,
                        Arguments = args,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                    }
                };

                process.OutputDataReceived += (sender, args) => Display(args.Data);
                process.ErrorDataReceived += (sender, args) => Display(args.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            catch (Exception err)
            {
                ConsoleData.Text = $"{err.Message} File: `{path}`";
            }
        }

        private Process Find_Proc(string name)
        {
            var procList = Process.GetProcessesByName(name);
            return procList[0];
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
    }
}
