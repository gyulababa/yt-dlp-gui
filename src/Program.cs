// Program.cs

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize(); // sets up fonts, DPI, etc.
        Application.Run(new MainForm()); // launches your GUI
    }
}
