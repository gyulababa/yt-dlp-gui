// Helpers.cs
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace YtDlpGuiApp
{
    public static class Helpers
    {
        public static void AppendOutputSafe(string? line, TextBox outputTextBox, Label progressLabel, ProgressBar bar)
        {
            if (string.IsNullOrWhiteSpace(line)) return;

            // Handle download line separately
            if (line.StartsWith("[download]"))
            {
                if (line.Contains("100% of"))
                {
                    progressLabel.Invoke(() => progressLabel.Text = line);
                    bar.Invoke(() => bar.Value = 100);
                }
                return; // Don't print to outputTextBox
            }

            outputTextBox.Invoke(() =>
            {
                outputTextBox.AppendText(line + Environment.NewLine);
            });
        }
    }
}

