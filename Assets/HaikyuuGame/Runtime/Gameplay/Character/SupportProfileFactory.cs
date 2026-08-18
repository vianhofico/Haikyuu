using HaikyuuGame.Gameplay.Player;

namespace HaikyuuGame.Gameplay.Character
{
    public static class SupportProfileFactory
    {
        public static RuntimeCharacterProfile Create(string school, VolleyballRole role, int index, int strength = 72)
        {
            int s = Clamp(strength);
            CharacterStats stats = new CharacterStats
            {
                attack = role == VolleyballRole.OutsideHitter || role == VolleyballRole.Opposite ? Clamp(s + 5) : s,
                serve = s,
                set = role == VolleyballRole.Setter ? Clamp(s + 12) : Clamp(s - 8),
                receive = role == VolleyballRole.Libero ? Clamp(s + 15) : s,
                block = role == VolleyballRole.MiddleBlocker ? Clamp(s + 13) : Clamp(s - 4),
                jump = role == VolleyballRole.MiddleBlocker ? Clamp(s + 7) : s,
                speed = s,
                stamina = Clamp(s + 4),
                technique = s,
                mental = s
            };

            CharacterArchetype archetype;
            switch (role)
            {
                case VolleyballRole.Setter: archetype = CharacterArchetype.PrecisionSetter; break;
                case VolleyballRole.MiddleBlocker: archetype = CharacterArchetype.PowerBlocker; break;
                case VolleyballRole.Libero: archetype = CharacterArchetype.GuardianLibero; break;
                default: archetype = CharacterArchetype.AllRounder; break;
            }

            string id = $"support_{school.ToLowerInvariant().Replace(" ", "_")}_{role.ToString().ToLowerInvariant()}_{index}";
            return new RuntimeCharacterProfile(id, $"{school} Support {index}", school, role, archetype, stats);
        }

        private static int Clamp(int value)
        {
            if (value < 1) return 1;
            return value > 100 ? 100 : value;
        }
    }
}
