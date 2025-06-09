// Downloader.cs
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace YtDlpGuiApp
{
    public class Downloader
    {
        public static string YtDlpPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.exe");

        public static bool CheckFfmpegInstalled()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = "-version",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit(2000);
                return output.Contains("ffmpeg version");
            }
            catch
            {
                return false;
            }
        }

        public static void EnsureYtDlpExistsOrUpdate(bool checkUpdate = false)
        {
            if (!File.Exists(YtDlpPath) || checkUpdate)
            {
                DialogResult result = MessageBox.Show(
                    checkUpdate ? "Check for yt-dlp update?" : "yt-dlp.exe not found. Download now?",
                    "yt-dlp",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            client.DownloadFile("https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe", YtDlpPath);
                            MessageBox.Show("yt-dlp.exe successfully downloaded.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to download yt-dlp.exe:\n" + ex.Message);
                    }
                }
            }
        }

        public static void RunYtDlp(string url, string downloadFolder, string formatArgs, string filenameTemplate, TextBox outputTextBox, Label progressLabel, ProgressBar bar)
        {
            string sanitizedFilename = Regex.Replace(filenameTemplate, "[\\/:*?\"<>|]", "_");
            string outputTemplate = Path.Combine(downloadFolder, sanitizedFilename);
            string args = $"{formatArgs} -o \"{outputTemplate}\" {url}";

            var psi = new ProcessStartInfo
            {
                FileName = YtDlpPath,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = psi };
            process.OutputDataReceived += (s, e) => Helpers.AppendOutputSafe(e.Data, outputTextBox, progressLabel, bar);
            process.ErrorDataReceived += (s, e) => Helpers.AppendOutputSafe(e.Data, outputTextBox, progressLabel, bar);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }
    }
}