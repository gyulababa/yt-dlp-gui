// MainForm.cs
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace YtDlpGuiApp
{
    public class MainForm : Form
    {
        private TextBox urlTextBox;
        private Button downloadButton;
        private TextBox outputTextBox;
        private Label downloadProgressLabel;
        private ProgressBar progressBar;
        private Label folderDisplayLabel;
        private ComboBox formatSelector;
        private CheckBox includeChannelCheckbox;
        private FolderBrowserDialog folderDialog;
        private string downloadFolder;

        public MainForm()
        {
            Text = "yt-dlp GUI";
            Size = new Size(570, 615);
            Font = new Font("Segoe UI", 12, FontStyle.Bold);

            urlTextBox = new TextBox { Multiline = true, ScrollBars = ScrollBars.Vertical, Height = 60, Top = 10, Left = 10, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 11), Width = ClientSize.Width - 20 };

            formatSelector = new ComboBox { Left = 10, Top = 80, Width = 200, Font = new Font("Segoe UI", 11) };
            formatSelector.Items.AddRange(new[] { "Audio (MP3)", "Video (MP4)" });
            formatSelector.SelectedIndex = 0;

            includeChannelCheckbox = new CheckBox { Text = "Csatornanév hozzáadása", Top = 80, Left = 220, Width = 220 };

            Button folderButton = new Button { Text = "Mappa", Top = 75, Left = 440, Width = 80, Height = 40 };
            folderButton.Click += (s, e) => ChooseDownloadFolder();

            folderDisplayLabel = new Label { Top = 115, Left = 10, Height = 25, Text = "", AutoEllipsis = true, Anchor = AnchorStyles.Left | AnchorStyles.Right, Width = ClientSize.Width - 20 };

            downloadButton = new Button { Text = "Download", Top = 150, Left = 10, Height = 40, Anchor = AnchorStyles.Left | AnchorStyles.Right, Width = ClientSize.Width - 20 };
            downloadButton.Click += (s, e) => StartDownload();

            outputTextBox = new TextBox { Multiline = true, ScrollBars = ScrollBars.Vertical, Height = 280, Top = 200, Left = 10, Font = new Font("Consolas", 11), Anchor = AnchorStyles.Left | AnchorStyles.Right, Width = ClientSize.Width - 20 };

            downloadProgressLabel = new Label { Height = 30, Top = 485, Left = 10, Font = new Font("Consolas", 11), AutoEllipsis = true, BorderStyle = BorderStyle.FixedSingle, Anchor = AnchorStyles.Left | AnchorStyles.Right, Width = ClientSize.Width - 20 };

            progressBar = new ProgressBar { Top = 520, Left = 10, Height = 10, Anchor = AnchorStyles.Left | AnchorStyles.Right, Width = ClientSize.Width - 20 };

            Controls.Add(urlTextBox);
            Controls.Add(formatSelector);
            Controls.Add(includeChannelCheckbox);
            Controls.Add(folderButton);
            Controls.Add(folderDisplayLabel);
            Controls.Add(downloadButton);
            Controls.Add(outputTextBox);
            Controls.Add(downloadProgressLabel);
            Controls.Add(progressBar);

            folderDialog = new FolderBrowserDialog();
            downloadFolder = AppDomain.CurrentDomain.BaseDirectory;
            folderDisplayLabel.Text = "Download folder: " + downloadFolder;

            Downloader.EnsureYtDlpExistsOrUpdate();
            bool ffmpegInPath = Downloader.CheckFfmpegInstalled();

            Label ffmpegStatusLabel = new Label
            {
                Text = ffmpegInPath ? "✅ FFmpeg found" : "❌ FFmpeg not found",
                ForeColor = ffmpegInPath ? Color.Green : Color.Red,
                AutoSize = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Left = ClientSize.Width - 200,
                Top = ClientSize.Height - 30
            };
            Controls.Add(ffmpegStatusLabel);

            Resize += (s, e) =>
            {
                ffmpegStatusLabel.Left = ClientSize.Width - ffmpegStatusLabel.Width - 10;
                ffmpegStatusLabel.Top = ClientSize.Height - ffmpegStatusLabel.Height - 10;
            };
        }

        private void ChooseDownloadFolder()
        {
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                downloadFolder = folderDialog.SelectedPath;
                folderDisplayLabel.Text = "Download folder: " + downloadFolder;
            }
        }

        private void StartDownload()
        {
            outputTextBox.Clear();
            downloadProgressLabel.Text = "";
            progressBar.Value = 0;

            string[] urls = urlTextBox.Text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            if (urls.Length == 0)
            {
                MessageBox.Show("Please enter at least one link.");
                return;
            }

            foreach (string url in urls)
            {
                string cleanUrl = url.Trim();
                if (string.IsNullOrWhiteSpace(cleanUrl)) continue;

                bool isPlaylist = cleanUrl.Contains("list=");
                if (isPlaylist)
                {
                    DialogResult result = MessageBox.Show(
                        $"The link appears to be part of a playlist:\n{cleanUrl}\n\nDownload entire playlist?",
                        "Playlist Detected",
                        MessageBoxButtons.YesNoCancel);

                    if (result == DialogResult.Cancel) return;
                    if (result == DialogResult.No) cleanUrl += " --no-playlist";
                }

                string formatArgs = formatSelector.SelectedIndex == 0 ? "-x --audio-format mp3" : "--recode-video mp4";
                string filenameTemplate = formatSelector.SelectedIndex == 0
                    ? includeChannelCheckbox.Checked ? "%(title)s [%(uploader)s].mp3" : "%(title)s.mp3"
                    : includeChannelCheckbox.Checked ? "%(title)s [%(uploader)s].mp4" : "%(title)s.mp4";

                Downloader.RunYtDlp(cleanUrl, downloadFolder, formatArgs, filenameTemplate, outputTextBox, downloadProgressLabel, progressBar);
            }
        }
    }
}