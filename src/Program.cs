// Program.cs

using System;
using System.Windows.Forms;

namespace YtDlpGuiApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}

