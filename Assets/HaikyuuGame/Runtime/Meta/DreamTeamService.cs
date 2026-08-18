using System.Collections.Generic;
using HaikyuuGame.Gameplay.Character;
using HaikyuuGame.Gameplay.Player;
using HaikyuuGame.Persistence;

namespace HaikyuuGame.Meta
{
    public sealed class DreamTeamService
    {
        private static readonly VolleyballRole[] SlotRoles =
        {
            VolleyballRole.Setter,
            VolleyballRole.OutsideHitter,
            VolleyballRole.MiddleBlocker,
            VolleyballRole.Opposite,
            VolleyballRole.OutsideHitter,
            VolleyballRole.MiddleBlocker,
            VolleyballRole.Libero
        };

        private static readonly string[] Defaults =
        {
            "kageyama_tobio",
            "bokuto_kotaro",
            "hinata_shoyo",
            "ushijima_wakatoshi",
            "hoshiumi_korai",
            "tsukishima_kei",
            "nishinoya_yu"
        };

        private readonly SaveGameData _save;

        public DreamTeamService(SaveGameData save)
        {
            _save = save;
            EnsureDefaults(_save);
        }

        public static void EnsureDefaults(SaveGameData save)
        {
            if (save.dreamTeamCharacterIds == null)
            {
                save.dreamTeamCharacterIds = new List<string>();
            }

            while (save.dreamTeamCharacterIds.Count < Defaults.Length)
            {
                save.dreamTeamCharacterIds.Add(Defaults[save.dreamTeamCharacterIds.Count]);
            }

            if (save.dreamTeamCharacterIds.Count > Defaults.Length)
            {
                save.dreamTeamCharacterIds.RemoveRange(Defaults.Length, save.dreamTeamCharacterIds.Count - Defaults.Length);
            }

            for (int i = 0; i < Defaults.Length; i++)
            {
                RuntimeCharacterProfile profile = HaikyuuRosterCatalog.Get(save.dreamTeamCharacterIds[i]);
                if (profile == null || profile.Role != SlotRoles[i])
                {
                    save.dreamTeamCharacterIds[i] = Defaults[i];
                }
            }
        }

        public RuntimeCharacterProfile GetProfile(int slot)
        {
            if (slot < 0 || slot >= SlotRoles.Length)
            {
                return null;
            }

            EnsureDefaults(_save);
            return HaikyuuRosterCatalog.Get(_save.dreamTeamCharacterIds[slot]);
        }

        public RuntimeCharacterProfile Cycle(int slot, int direction)
        {
            if (slot < 0 || slot >= SlotRoles.Length)
            {
                return null;
            }

            List<RuntimeCharacterProfile> candidates = CandidatesFor(SlotRoles[slot]);
            if (candidates.Count == 0)
            {
                return GetProfile(slot);
            }

            RuntimeCharacterProfile current = GetProfile(slot);
            int index = 0;
            for (int i = 0; i < candidates.Count; i++)
            {
                if (current != null && candidates[i].Id == current.Id)
                {
                    index = i;
                    break;
                }
            }

            int step = direction < 0 ? -1 : 1;
            index = (index + step + candidates.Count) % candidates.Count;
            RuntimeCharacterProfile selected = candidates[index];
            _save.dreamTeamCharacterIds[slot] = selected.Id;
            return selected;
        }

        public static VolleyballRole RoleForSlot(int slot)
        {
            return slot >= 0 && slot < SlotRoles.Length ? SlotRoles[slot] : VolleyballRole.OutsideHitter;
        }

        private static List<RuntimeCharacterProfile> CandidatesFor(VolleyballRole role)
        {
            List<RuntimeCharacterProfile> result = new List<RuntimeCharacterProfile>();
            IReadOnlyList<RuntimeCharacterProfile> all = HaikyuuRosterCatalog.All;
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].Role == role)
                {
                    result.Add(all[i]);
                }
            }

            return result;
        }
    }
}
