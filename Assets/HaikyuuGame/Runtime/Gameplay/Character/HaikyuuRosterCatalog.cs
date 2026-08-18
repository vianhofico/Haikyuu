using System.Collections.Generic;
using HaikyuuGame.Gameplay.Player;

namespace HaikyuuGame.Gameplay.Character
{
    // Fan-prototype data only. No copyrighted artwork or dialogue is stored here.
    public static class HaikyuuRosterCatalog
    {
        private static readonly RuntimeCharacterProfile[] Profiles =
        {
            P("hinata_shoyo", "Shoyo Hinata", "Karasuno", VolleyballRole.MiddleBlocker, CharacterArchetype.SpeedDecoy, 78, 58, 45, 64, 68, 98, 96, 92, 82, 96, "freak_quick", "greatest_decoy", "boom_jump"),
            P("kageyama_tobio", "Tobio Kageyama", "Karasuno", VolleyballRole.Setter, CharacterArchetype.PrecisionSetter, 82, 92, 99, 77, 82, 88, 88, 92, 98, 86, "kings_toss", "minus_tempo", "pinpoint_serve"),
            P("tsukishima_kei", "Kei Tsukishima", "Karasuno", VolleyballRole.MiddleBlocker, CharacterArchetype.ReadBlocker, 72, 70, 48, 62, 94, 79, 70, 76, 92, 88, "read_block", "one_touch"),
            P("yamaguchi_tadashi", "Tadashi Yamaguchi", "Karasuno", VolleyballRole.MiddleBlocker, CharacterArchetype.FloatServer, 62, 91, 50, 61, 67, 72, 72, 80, 82, 88, "jump_float", "pressure_server"),
            P("tanaka_ryunosuke", "Ryunosuke Tanaka", "Karasuno", VolleyballRole.OutsideHitter, CharacterArchetype.MomentumAce, 88, 80, 54, 74, 70, 84, 82, 96, 82, 99, "sharp_cross", "straight_shot", "mental_monster"),
            P("asahi_azumane", "Asahi Azumane", "Karasuno", VolleyballRole.OutsideHitter, CharacterArchetype.PowerAce, 95, 88, 48, 74, 76, 86, 70, 90, 84, 82, "heavy_spike", "block_crusher"),
            P("nishinoya_yu", "Yu Nishinoya", "Karasuno", VolleyballRole.Libero, CharacterArchetype.GuardianLibero, 35, 40, 76, 99, 30, 78, 97, 96, 94, 98, "rolling_thunder", "guardian_deity", "foot_save"),
            P("daichi_sawamura", "Daichi Sawamura", "Karasuno", VolleyballRole.OutsideHitter, CharacterArchetype.ReliableCaptain, 78, 72, 55, 94, 72, 75, 78, 98, 87, 99, "captain", "reliable_receive", "cover"),
            P("sugawara_koshi", "Koshi Sugawara", "Karasuno", VolleyballRole.Setter, CharacterArchetype.TeamAmplifierSetter, 60, 70, 91, 76, 58, 72, 77, 87, 90, 98, "team_sync", "setter_switch"),

            P("kenma_kozume", "Kenma Kozume", "Nekoma", VolleyballRole.Setter, CharacterArchetype.StrategistSetter, 55, 58, 96, 82, 55, 66, 70, 62, 99, 92, "analyze", "cage"),
            P("kuroo_tetsuro", "Tetsuro Kuroo", "Nekoma", VolleyballRole.MiddleBlocker, CharacterArchetype.ReadBlocker, 84, 84, 64, 80, 96, 84, 82, 91, 96, 96, "funnel_block", "scheming_captain"),
            P("yaku_morisuke", "Morisuke Yaku", "Nekoma", VolleyballRole.Libero, CharacterArchetype.GuardianLibero, 30, 35, 70, 99, 25, 72, 94, 94, 96, 98, "perfect_positioning", "invisible_defense"),
            P("lev_haiba", "Lev Haiba", "Nekoma", VolleyballRole.MiddleBlocker, CharacterArchetype.HighReach, 86, 62, 45, 48, 89, 93, 86, 78, 66, 76, "long_reach", "high_contact"),
            P("yamamoto_taketora", "Taketora Yamamoto", "Nekoma", VolleyballRole.OutsideHitter, CharacterArchetype.MomentumAce, 87, 79, 48, 88, 68, 82, 81, 95, 80, 96, "ace_grit", "power_receive"),
            P("kai_nobuyuki", "Nobuyuki Kai", "Nekoma", VolleyballRole.OutsideHitter, CharacterArchetype.ReliableCaptain, 75, 70, 54, 91, 66, 72, 76, 94, 84, 96, "stable_receive", "connect"),

            P("oikawa_toru", "Toru Oikawa", "Aoba Johsai", VolleyballRole.Setter, CharacterArchetype.TeamAmplifierSetter, 77, 99, 97, 80, 76, 84, 86, 94, 98, 96, "talent_extraction", "demon_serve", "adapt"),
            P("iwaizumi_hajime", "Hajime Iwaizumi", "Aoba Johsai", VolleyballRole.OutsideHitter, CharacterArchetype.PowerAce, 92, 85, 50, 86, 73, 84, 82, 96, 88, 98, "reliable_ace", "power_cross"),
            P("kyotani_kentaro", "Kentaro Kyotani", "Aoba Johsai", VolleyballRole.OutsideHitter, CharacterArchetype.WildCard, 95, 83, 45, 66, 70, 89, 92, 91, 76, 80, "mad_dog", "wild_approach"),
            P("kindaichi_yutaro", "Yutaro Kindaichi", "Aoba Johsai", VolleyballRole.MiddleBlocker, CharacterArchetype.HighReach, 82, 70, 45, 62, 84, 88, 79, 84, 79, 82, "high_quick"),
            P("kunimi_akira", "Akira Kunimi", "Aoba Johsai", VolleyballRole.OutsideHitter, CharacterArchetype.EnergySaver, 78, 72, 58, 82, 68, 75, 77, 94, 90, 90, "energy_conservation", "late_game"),
            P("yahaba_shigeru", "Shigeru Yahaba", "Aoba Johsai", VolleyballRole.Setter, CharacterArchetype.AllRounder, 62, 72, 85, 70, 58, 70, 76, 84, 84, 90, "stable_set"),

            P("aone_takanobu", "Takanobu Aone", "Date Tech", VolleyballRole.MiddleBlocker, CharacterArchetype.PowerBlocker, 84, 72, 42, 64, 99, 90, 78, 96, 88, 94, "lock_on", "iron_wall"),
            P("futakuchi_kenji", "Kenji Futakuchi", "Date Tech", VolleyballRole.OutsideHitter, CharacterArchetype.AllRounder, 84, 81, 48, 76, 88, 80, 79, 91, 84, 90, "block_attack_hybrid"),
            P("koganegawa_kanji", "Kanji Koganegawa", "Date Tech", VolleyballRole.Setter, CharacterArchetype.HighReach, 70, 74, 78, 66, 91, 92, 81, 86, 70, 88, "high_point_set", "setter_block"),
            P("sakunami_kosuke", "Kosuke Sakunami", "Date Tech", VolleyballRole.Libero, CharacterArchetype.GuardianLibero, 25, 30, 66, 90, 20, 68, 89, 89, 86, 92, "stable_dig"),

            P("ushijima_wakatoshi", "Wakatoshi Ushijima", "Shiratorizawa", VolleyballRole.Opposite, CharacterArchetype.PowerAce, 100, 96, 45, 84, 87, 94, 84, 100, 94, 99, "southpaw", "cannon_spike", "relentless_ace"),
            P("tendou_satori", "Satori Tendo", "Shiratorizawa", VolleyballRole.MiddleBlocker, CharacterArchetype.GuessBlocker, 80, 72, 48, 62, 97, 85, 84, 89, 96, 94, "guess_block", "baki_baki"),
            P("goshiki_tsutomu", "Tsutomu Goshiki", "Shiratorizawa", VolleyballRole.OutsideHitter, CharacterArchetype.TechnicalAce, 88, 82, 48, 75, 68, 88, 85, 92, 90, 93, "sharp_straight", "future_ace"),
            P("shirabu_kenjiro", "Kenjiro Shirabu", "Shiratorizawa", VolleyballRole.Setter, CharacterArchetype.PrecisionSetter, 55, 66, 91, 72, 61, 70, 73, 90, 91, 94, "ace_first", "stable_system"),
            P("semi_eita", "Eita Semi", "Shiratorizawa", VolleyballRole.Setter, CharacterArchetype.DualServerSetter, 72, 91, 88, 70, 62, 78, 80, 88, 89, 91, "power_serve", "aggressive_set"),
            P("ohira_reon", "Reon Ohira", "Shiratorizawa", VolleyballRole.OutsideHitter, CharacterArchetype.AllRounder, 86, 82, 52, 88, 75, 80, 78, 94, 86, 95, "all_round_wing"),

            P("bokuto_kotaro", "Kotaro Bokuto", "Fukurodani", VolleyballRole.OutsideHitter, CharacterArchetype.MomentumAce, 99, 91, 52, 84, 75, 96, 90, 97, 96, 86, "hey_hey_hey", "extreme_cross", "ace_awakening"),
            P("akaashi_keiji", "Keiji Akaashi", "Fukurodani", VolleyballRole.Setter, CharacterArchetype.StrategistSetter, 64, 73, 96, 79, 63, 76, 82, 90, 98, 98, "bokuto_management", "precision", "reset_ace"),
            P("konoha_akinori", "Akinori Konoha", "Fukurodani", VolleyballRole.OutsideHitter, CharacterArchetype.AllRounder, 80, 78, 80, 84, 75, 79, 83, 92, 90, 93, "jack_of_all_trades"),
            P("washio_tatsuki", "Tatsuki Washio", "Fukurodani", VolleyballRole.MiddleBlocker, CharacterArchetype.PowerBlocker, 84, 76, 44, 64, 93, 86, 75, 91, 84, 91, "power_block"),
            P("komi_haruki", "Haruki Komi", "Fukurodani", VolleyballRole.Libero, CharacterArchetype.GuardianLibero, 28, 34, 68, 94, 22, 70, 91, 91, 90, 94, "owl_guardian"),

            P("miya_atsumu", "Atsumu Miya", "Inarizaki", VolleyballRole.Setter, CharacterArchetype.DualServerSetter, 75, 99, 99, 80, 72, 88, 90, 95, 99, 91, "dual_serve", "twin_quick", "reckless_genius"),
            P("miya_osamu", "Osamu Miya", "Inarizaki", VolleyballRole.Opposite, CharacterArchetype.FlexibleHitter, 90, 84, 86, 82, 73, 86, 86, 94, 94, 94, "twin_quick", "emergency_set", "fake_twin"),
            P("suna_rintaro", "Rintaro Suna", "Inarizaki", VolleyballRole.MiddleBlocker, CharacterArchetype.TechnicalAce, 88, 74, 46, 68, 88, 87, 84, 88, 98, 86, "torso_spike", "angle_shift"),
            P("aran_ojiro", "Aran Ojiro", "Inarizaki", VolleyballRole.OutsideHitter, CharacterArchetype.PowerAce, 97, 90, 50, 84, 78, 93, 83, 98, 92, 96, "power_spike", "high_contact"),
            P("kita_shinsuke", "Shinsuke Kita", "Inarizaki", VolleyballRole.OutsideHitter, CharacterArchetype.ReliableCaptain, 75, 76, 58, 94, 70, 72, 76, 100, 94, 100, "perfect_routine", "captain_presence"),
            P("akagi_michinari", "Michinari Akagi", "Inarizaki", VolleyballRole.Libero, CharacterArchetype.GuardianLibero, 28, 32, 68, 96, 20, 72, 94, 94, 92, 96, "elite_reaction"),
            P("omimi_ren", "Ren Omimi", "Inarizaki", VolleyballRole.MiddleBlocker, CharacterArchetype.HighReach, 84, 76, 44, 65, 92, 88, 76, 92, 84, 91, "tower_block"),

            P("hoshiumi_korai", "Korai Hoshiumi", "Kamomedai", VolleyballRole.OutsideHitter, CharacterArchetype.TechnicalAce, 94, 94, 75, 96, 84, 100, 97, 96, 99, 99, "boom_jump", "tool_block", "little_giant"),
            P("hirugami_sachiro", "Sachiro Hirugami", "Kamomedai", VolleyballRole.MiddleBlocker, CharacterArchetype.ReadBlocker, 82, 86, 48, 75, 99, 89, 84, 96, 99, 100, "calm_read", "complete_block", "no_pressure"),
            P("gao_hakuba", "Gao Hakuba", "Kamomedai", VolleyballRole.Opposite, CharacterArchetype.HighReach, 94, 80, 42, 62, 94, 95, 72, 96, 78, 90, "height_dominance", "high_contact"),

            P("sakusa_kiyoomi", "Kiyoomi Sakusa", "Itachiyama", VolleyballRole.OutsideHitter, CharacterArchetype.TechnicalAce, 98, 94, 62, 96, 79, 92, 88, 98, 100, 99, "flexible_wrist", "precision_spike", "elite_receive"),
            P("kiryu_wakatsu", "Wakatsu Kiryu", "Mujinazaka", VolleyballRole.OutsideHitter, CharacterArchetype.PowerAce, 99, 92, 55, 88, 80, 94, 84, 100, 97, 98, "bad_set_killer", "heavy_spike", "adapted_approach")
        };

        private static readonly Dictionary<string, RuntimeCharacterProfile> ById = BuildIndex();

        public static IReadOnlyList<RuntimeCharacterProfile> All => Profiles;

        public static RuntimeCharacterProfile Get(string id)
        {
            return id != null && ById.TryGetValue(id, out RuntimeCharacterProfile profile) ? profile : null;
        }

        private static Dictionary<string, RuntimeCharacterProfile> BuildIndex()
        {
            Dictionary<string, RuntimeCharacterProfile> index = new Dictionary<string, RuntimeCharacterProfile>();
            for (int i = 0; i < Profiles.Length; i++)
            {
                index[Profiles[i].Id] = Profiles[i];
            }

            return index;
        }

        private static RuntimeCharacterProfile P(
            string id,
            string name,
            string school,
            VolleyballRole role,
            CharacterArchetype archetype,
            int attack,
            int serve,
            int set,
            int receive,
            int block,
            int jump,
            int speed,
            int stamina,
            int technique,
            int mental,
            params string[] skills)
        {
            CharacterStats stats = new CharacterStats
            {
                attack = attack,
                serve = serve,
                set = set,
                receive = receive,
                block = block,
                jump = jump,
                speed = speed,
                stamina = stamina,
                technique = technique,
                mental = mental
            };

            return new RuntimeCharacterProfile(id, name, school, role, archetype, stats, skills);
        }
    }
}
