using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
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
        private Label ffmpegStatusLabel;
        private Button optionsButton;
    
        // Class-level constants for layout
        private readonly int margin = 10;
        private readonly int controlSpacing = 8;
    
        public MainForm()
        {
            Text = "yt-dlp GUI";
            Size = new Size(570, 615);
            Font = new Font("Segoe UI", 12, FontStyle.Bold);
    
            urlTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 11),
                Height = 60,
                Left = margin,
                Top = margin,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
    
            formatSelector = new ComboBox
            {
                Left = margin,
                Width = 200,
                Height = 30,
                Font = new Font("Segoe UI", 11),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            formatSelector.Items.AddRange(new[] { "Audio (MP3)", "Video (MP4)" });
            formatSelector.SelectedIndex = 0;
    
            includeChannelCheckbox = new CheckBox
            {
                Text = Localization.T("include_channel"),
                Height = 30,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
    
            folderDisplayLabel = new Label
            {
                Height = 25,
                Text = Localization.T("download_folder") + ": " + downloadFolder,
                AutoEllipsis = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
    
            downloadButton = new Button
            {
                Text = Localization.T("download"),
                Height = 40,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            downloadButton.Click += async (s, e) => await StartDownload();
    
            outputTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 11),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
    
            downloadProgressLabel = new Label
            {
                Height = 30,
                Font = new Font("Consolas", 11),
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
    
            progressBar = new ProgressBar
            {
                Height = 10,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
    
            optionsButton = new Button
            {
                Text = "⚙",
                Width = 35,
                Height = 35,
                Font = new Font("Segoe UI Symbol", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            optionsButton.Click += (s, e) => ShowOptionsDialog();
    
            ffmpegStatusLabel = new Label
            {
                ForeColor = Color.Red,
                AutoSize = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
    
            Controls.AddRange(new Control[]
            {
                urlTextBox,
                formatSelector,
                includeChannelCheckbox,
                folderDisplayLabel,
                downloadButton,
                outputTextBox,
                downloadProgressLabel,
                progressBar,
                optionsButton,
                ffmpegStatusLabel
            });
    
            Resize += (s, e) => UpdateLayout();
            UpdateLayout(); // Initial layout
        }
    
        private void UpdateLayout()
        {
            int padBetween = 5;
    
            urlTextBox.Width = ClientSize.Width - 2 * margin;
    
            formatSelector.Top = urlTextBox.Bottom + controlSpacing;
    
            includeChannelCheckbox.Left = formatSelector.Right + margin;
            includeChannelCheckbox.Top = formatSelector.Top + 3;
            includeChannelCheckbox.Width = ClientSize.Width - formatSelector.Right - 2 * margin;
    
            folderDisplayLabel.Top = formatSelector.Bottom + controlSpacing;
            folderDisplayLabel.Left = margin;
            folderDisplayLabel.Width = ClientSize.Width - 2 * margin;
    
            downloadButton.Top = folderDisplayLabel.Bottom + controlSpacing;
            downloadButton.Left = margin;
            downloadButton.Width = ClientSize.Width - 2 * margin;
    
            outputTextBox.Top = downloadButton.Bottom + controlSpacing;
            outputTextBox.Left = margin;
            outputTextBox.Width = ClientSize.Width - 2 * margin;
            outputTextBox.Height = ClientSize.Height
                                 - outputTextBox.Top
                                 - controlSpacing
                                 - downloadProgressLabel.Height
                                 - padBetween
                                 - progressBar.Height
                                 - padBetween
                                 - optionsButton.Height
                                 - margin;
    
            downloadProgressLabel.Top = outputTextBox.Bottom + padBetween;
            downloadProgressLabel.Left = margin;
            downloadProgressLabel.Width = ClientSize.Width - 2 * margin;
    
            progressBar.Top = downloadProgressLabel.Bottom + padBetween;
            progressBar.Left = margin;
            progressBar.Width = ClientSize.Width - 2 * margin;
    
            optionsButton.Top = progressBar.Bottom + padBetween;
            optionsButton.Left = margin;
    
            ffmpegStatusLabel.Top = optionsButton.Top + (optionsButton.Height - ffmpegStatusLabel.Height);
            ffmpegStatusLabel.Left = ClientSize.Width - ffmpegStatusLabel.Width - margin;
        }   

        private void ChooseDownloadFolder()
        {
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                downloadFolder = folderDialog.SelectedPath;
                folderDisplayLabel.Text = "Download folder: " + downloadFolder;
            }
        }

        private async Task StartDownload()
        {
            outputTextBox.Clear();
            downloadProgressLabel.Text = string.Empty;
            progressBar.Value = 0;

            string[] urls = urlTextBox.Text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            if (urls.Length == 0)
            {
                MessageBox.Show(Localization.T("invalid_input"));
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
                        string.Format(Localization.T("playlist_question"), cleanUrl),
                        Localization.T("playlist_title"),
                        MessageBoxButtons.YesNoCancel);

                    if (result == DialogResult.Cancel) return;
                    if (result == DialogResult.No) cleanUrl += " --no-playlist";
                }

                string formatArgs = formatSelector.SelectedIndex == 0 ? "-x --audio-format mp3" : "--recode-video mp4";
                string filenameTemplate = formatSelector.SelectedIndex == 0
                    ? includeChannelCheckbox.Checked ? "%(title)s [%(uploader)s].mp3" : "%(title)s.mp3"
                    : includeChannelCheckbox.Checked ? "%(title)s [%(uploader)s].mp4" : "%(title)s.mp4";

                progressBar.Value = 0;
                downloadProgressLabel.Text = string.Empty;

                await Downloader.RunYtDlp(cleanUrl, downloadFolder, formatArgs, filenameTemplate, outputTextBox, downloadProgressLabel, progressBar);
            }
        }

        private void ShowOptionsDialog()
        {
            int margin = 20;
            int spacing = 10;
            int comboBoxWidth = 120;
            int buttonWidth = 100;
            int dialogWidth = 2 * comboBoxWidth + 3 * margin;
            int dialogHeight = 160;
        
            var dialog = new Form
            {
                Text = Localization.T("options"),
                Size = new Size(dialogWidth, dialogHeight),
                StartPosition = FormStartPosition.CenterParent
            };
        
            // Language Dropdown
            var languageBox = new ComboBox
            {
                Left = margin,
                Top = margin,
                Width = comboBoxWidth
            };
            languageBox.Items.AddRange(new[] { "English", "Magyar" });
            languageBox.SelectedIndex = (int)Localization.CurrentLanguage;
        
            // Theme Dropdown
            var themeBox = new ComboBox
            {
                Left = languageBox.Right + spacing,
                Top = margin,
                Width = comboBoxWidth
            };
            themeBox.Items.AddRange(new[] { "Light", "Dark" });
            themeBox.SelectedIndex = (int)Theme.Current;
        
            // Folder select button
            var folderButton = new Button
            {
                Text = Localization.T("select_folder"),
                Left = margin,
                Top = languageBox.Bottom + spacing,
                Width = dialog.ClientSize.Width - 2 * margin
            };
            folderButton.Click += (s, e) =>
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    downloadFolder = folderDialog.SelectedPath;
                    folderDisplayLabel.Text = $"{Localization.T("download_folder")}: {downloadFolder}";
                }
            };
        
            // OK button
            var okButton = new Button
            {
                Text = "OK",
                Width = buttonWidth,
                Left = (dialog.ClientSize.Width - buttonWidth) / 2,
                Top = folderButton.Bottom + spacing
            };
            okButton.Click += (s, e) =>
            {
                Localization.CurrentLanguage = (Localization.Language)languageBox.SelectedIndex;
                Theme.Current = (Theme.Mode)themeBox.SelectedIndex;
                ApplyTheme();
                ApplyLocalization();
                dialog.Close();
            };
        
            dialog.Controls.Add(languageBox);
            dialog.Controls.Add(themeBox);
            dialog.Controls.Add(folderButton);
            dialog.Controls.Add(okButton);
        
            dialog.ShowDialog();
        }
        

        private void ApplyLocalization()
        {
            downloadButton.Text = Localization.T("download");
            includeChannelCheckbox.Text = Localization.T("include_channel");
            folderDisplayLabel.Text = $"{Localization.T("download_folder")}: {downloadFolder}";
        }

        private void ApplyTheme()
        {
            BackColor = Theme.BackColor;
            ForeColor = Theme.ForeColor;
            foreach (Control c in Controls)
            {
                c.BackColor = Theme.BackColor;
                c.ForeColor = Theme.ForeColor;
            }
        }
    }
}

