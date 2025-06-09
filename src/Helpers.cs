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

            outputTextBox.Invoke(() =>
            {
                outputTextBox.AppendText(line + Environment.NewLine);

                if (line.Contains("[download]") && line.Contains("%"))
                {
                    progressLabel.Text = line;
                    var match = Regex.Match(line, @"\s+(\d{1,3}\.\d+)%");
                    if (match.Success && double.TryParse(match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double percent))
                    {
                        int value = Math.Min((int)percent, 100);
                        bar.Value = value;
                    }
                }
                else if (line.Contains("100%"))
                {
                    bar.Value = 100;
                }
            });
        }
    }
}
