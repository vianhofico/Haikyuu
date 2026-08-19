using System.Collections.Generic;
using HaikyuuGame.Gameplay.Player;

namespace HaikyuuGame.Gameplay.Teams
{
    public sealed class TeamPreset
    {
        public TeamPreset(string id, string displayName, string[] startingSix, string liberoId, int supportStrength = 72)
        {
            Id = id;
            DisplayName = displayName;
            StartingSix = startingSix;
            LiberoId = liberoId;
            SupportStrength = supportStrength;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public string[] StartingSix { get; }
        public string LiberoId { get; }
        public int SupportStrength { get; }
    }

    public static class TeamPresetCatalog
    {
        private static readonly Dictionary<string, TeamPreset> Presets = new Dictionary<string, TeamPreset>
        {
            { "karasuno", new TeamPreset("karasuno", "Karasuno", new[] { "kageyama_tobio", "asahi_azumane", "hinata_shoyo", "daichi_sawamura", "tanaka_ryunosuke", "tsukishima_kei" }, "nishinoya_yu", 78) },
            { "inarizaki", new TeamPreset("inarizaki", "Inarizaki", new[] { "miya_atsumu", "aran_ojiro", "suna_rintaro", "miya_osamu", "kita_shinsuke", "omimi_ren" }, "akagi_michinari", 88) },
            { "aoba_johsai", new TeamPreset("aoba_johsai", "Aoba Johsai", new[] { "oikawa_toru", "iwaizumi_hajime", "kindaichi_yutaro", "kyotani_kentaro", "kunimi_akira", "yahaba_shigeru" }, null, 80) },
            { "nekoma", new TeamPreset("nekoma", "Nekoma", new[] { "kenma_kozume", "yamamoto_taketora", "kuroo_tetsuro", "kai_nobuyuki", "lev_haiba", null }, "yaku_morisuke", 79) },
            { "date_tech", new TeamPreset("date_tech", "Date Tech", new[] { "koganegawa_kanji", "futakuchi_kenji", "aone_takanobu", null, null, null }, "sakunami_kosuke", 78) },
            { "shiratorizawa", new TeamPreset("shiratorizawa", "Shiratorizawa", new[] { "shirabu_kenjiro", "goshiki_tsutomu", "tendou_satori", "ushijima_wakatoshi", "ohira_reon", "semi_eita" }, null, 88) },
            { "fukurodani", new TeamPreset("fukurodani", "Fukurodani", new[] { "akaashi_keiji", "bokuto_kotaro", "washio_tatsuki", "konoha_akinori", null, null }, "komi_haruki", 85) },
            { "kamomedai", new TeamPreset("kamomedai", "Kamomedai", new[] { null, "hoshiumi_korai", "hirugami_sachiro", "gao_hakuba", null, null }, null, 90) },
            { "all_star", new TeamPreset("all_star", "National All-Star", new[] { "miya_atsumu", "sakusa_kiyoomi", "hirugami_sachiro", "ushijima_wakatoshi", "kiryu_wakatsu", "suna_rintaro" }, "yaku_morisuke", 94) },
            { "dream_team", new TeamPreset("dream_team", "Dream Team", new[] { "kageyama_tobio", "bokuto_kotaro", "hinata_shoyo", "ushijima_wakatoshi", "hoshiumi_korai", "tsukishima_kei" }, "nishinoya_yu", 93) },
            { "training", new TeamPreset("training", "Training Squad", new string[] { null, null, null, null, null, null }, null, 63) }
        };

        public static TeamPreset Get(string id)
        {
            return id != null && Presets.TryGetValue(id, out TeamPreset preset) ? preset : Presets["karasuno"];
        }

        public static string StoryOpponentForChapter(int chapter)
        {
            switch (chapter)
            {
                case 1: return "date_tech";
                case 2: return "aoba_johsai";
                case 3: return "shiratorizawa";
                case 4: return "fukurodani";
                case 5: return "inarizaki";
                case 6: return "nekoma";
                case 7: return "kamomedai";
                case 8:
                case 9: return "all_star";
                default: return "training";
            }
        }

        public static VolleyballRole SupportRoleForSlot(int slot)
        {
            switch (slot)
            {
                case 0: return VolleyballRole.Setter;
                case 1: return VolleyballRole.OutsideHitter;
                case 2: return VolleyballRole.MiddleBlocker;
                case 3: return VolleyballRole.Opposite;
                case 4: return VolleyballRole.OutsideHitter;
                default: return VolleyballRole.MiddleBlocker;
            }
        }
    }
}
