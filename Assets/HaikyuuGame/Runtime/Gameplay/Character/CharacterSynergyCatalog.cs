namespace HaikyuuGame.Gameplay.Character
{
    public static class CharacterSynergyCatalog
    {
        public static float SetTargetPriority(RuntimeCharacterProfile setter, RuntimeCharacterProfile attacker)
        {
            if (setter == null || attacker == null)
            {
                return 0f;
            }

            if (Pair(setter, attacker, "kageyama_tobio", "hinata_shoyo")) return 2.8f;
            if (Pair(setter, attacker, "miya_atsumu", "miya_osamu")) return 2.6f;
            if (Pair(setter, attacker, "akaashi_keiji", "bokuto_kotaro")) return 2.2f;
            if (Pair(setter, attacker, "oikawa_toru", "iwaizumi_hajime")) return 1.8f;
            if (Pair(setter, attacker, "shirabu_kenjiro", "ushijima_wakatoshi")) return 1.7f;
            return 0f;
        }

        public static float AttackMultiplier(RuntimeCharacterProfile setter, RuntimeCharacterProfile attacker)
        {
            if (setter == null || attacker == null)
            {
                return 1f;
            }

            if (Pair(setter, attacker, "kageyama_tobio", "hinata_shoyo")) return 1.16f;
            if (Pair(setter, attacker, "miya_atsumu", "miya_osamu")) return 1.13f;
            if (Pair(setter, attacker, "akaashi_keiji", "bokuto_kotaro")) return 1.10f;
            if (Pair(setter, attacker, "oikawa_toru", "iwaizumi_hajime")) return 1.08f;
            if (Pair(setter, attacker, "shirabu_kenjiro", "ushijima_wakatoshi")) return 1.08f;
            return 1f;
        }

        private static bool Pair(RuntimeCharacterProfile a, RuntimeCharacterProfile b, string first, string second)
        {
            return (a.Id == first && b.Id == second) || (a.Id == second && b.Id == first);
        }
    }
}
