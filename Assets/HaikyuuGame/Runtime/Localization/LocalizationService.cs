using System.Collections.Generic;

namespace HaikyuuGame.Localization
{
    public sealed class LocalizationService
    {
        private readonly Dictionary<string, string> _vi = new Dictionary<string, string>
        {
            { "shell.title", "Haikyuu Volleyball - Fan Prototype" },
            { "shell.level", "Cấp" },
            { "shell.coins", "Xu" },
            { "shell.wins", "Thắng" },
            { "shell.language", "Ngôn ngữ" },
            { "mode.quick", "Đấu nhanh" },
            { "mode.story", "Cốt truyện" },
            { "mode.career", "Sự nghiệp" },
            { "mode.tournament", "Giải đấu" },
            { "match.win", "Chiến thắng" },
            { "match.loss", "Thất bại" }
        };

        private readonly Dictionary<string, string> _en = new Dictionary<string, string>
        {
            { "shell.title", "Haikyuu Volleyball - Fan Prototype" },
            { "shell.level", "Level" },
            { "shell.coins", "Coins" },
            { "shell.wins", "Wins" },
            { "shell.language", "Language" },
            { "mode.quick", "Quick Match" },
            { "mode.story", "Story" },
            { "mode.career", "Career" },
            { "mode.tournament", "Tournament" },
            { "match.win", "Victory" },
            { "match.loss", "Defeat" }
        };

        public string Language { get; private set; } = "vi";

        public void SetLanguage(string language)
        {
            Language = language == "en" ? "en" : "vi";
        }

        public string T(string key)
        {
            Dictionary<string, string> table = Language == "en" ? _en : _vi;
            return table.TryGetValue(key, out string value) ? value : key;
        }
    }
}
