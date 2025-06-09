namespace YtDlpGuiApp
{
    public static class Localization
    {
        public enum Language { English, Hungarian }

        public static Language CurrentLanguage = Language.Hungarian;

        public static string T(string key)
        {
            return CurrentLanguage switch
            {
                Language.Hungarian => hu(key),
                _ => en(key),
            };
        }

        private static string en(string key) => key switch
        {
            "download" => "Download",
            "folder" => "Folder",
            "include_channel" => "Include Channel Name",
            "invalid_input" => "Please enter at least one link.",
            "playlist_question" => "The link appears to be part of a playlist:\n{0}\n\nDownload entire playlist?",
            "playlist_title" => "Playlist Detected",
            "download_folder" => "Download folder",
            "select_folder" => "Choose folder",
            "options" => "⚙ Options",
            _ => key
        };
        
        private static string hu(string key) => key switch
        {
            "download" => "Letöltés",
            "folder" => "Mappa",
            "include_channel" => "Csatornanév hozzáadása",
            "invalid_input" => "Kérlek adj meg legalább egy linket.",
            "playlist_question" => "A link egy lejátszási lista része:\n{0}\n\nLetöltöd az egészet?",
            "playlist_title" => "Lejátszási lista",
            "download_folder" => "Letöltési mappa",
            "select_folder" => "Mappa kiválasztása",
            "options" => "⚙ Beállítások",
            _ => key
        };
        
    }
}
