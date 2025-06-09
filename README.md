# yt-dlp GUI App

A simple Windows Forms application for downloading videos or audio using [yt-dlp](https://github.com/yt-dlp/yt-dlp).

## ✨ Features

* Download video or audio from YouTube and other sites
* Format selector: MP3 or MP4
* Optionally add channel name to filenames
* Supports playlists
* Detects and downloads `yt-dlp.exe` if missing
* Checks for `ffmpeg` availability

---

## 🛠 Requirements

* .NET 6 SDK or newer ([Download here](https://dotnet.microsoft.com/en-us/download))
* yt-dlp.exe (auto-downloaded by the app if missing)
* ffmpeg in system PATH (for audio extraction)

---

## 🚀 Build Instructions

### Option 1: Build with Runtime Included (Portable)

Creates a single `.exe` file that runs on any Windows system.

```bash
build-standalone.bat
```

➡ Output will be in `publish-standalone/`

### Option 2: Build Without Runtime (Smaller)

Creates a small `.exe` (requires .NET installed on target system).

```bash
build-trimmed.bat
```

➡ Output will be in `publish-trimmed/`

---

## 🗃 Project Structure

```
YtDlpGuiApp/
├── src/
│   ├── MainForm.cs
│   ├── Program.cs
│   ├── Downloader.cs
│   └── Helpers.cs
├── YtDlpGuiApp.csproj
├── build-standalone.bat
├── build-trimmed.bat
└── README.md
```

---

## 📦 Notes

* Place `yt-dlp.exe` in the same directory, or let the app auto-download it.
* `ffmpeg.exe` must be in your system PATH for MP3 downloads.
* Output folder defaults to the app's directory, but can be changed in the UI.

---

## 📄 License

This is a personal utility project. You are free to modify or distribute it.
