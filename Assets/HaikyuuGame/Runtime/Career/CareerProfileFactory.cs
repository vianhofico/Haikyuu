using System;
using HaikyuuGame.Gameplay.Character;
using HaikyuuGame.Gameplay.Player;
using HaikyuuGame.Persistence;

namespace HaikyuuGame.Career
{
    public static class CareerProfileFactory
    {
        public static RuntimeCharacterProfile Create(CareerSaveData career)
        {
            if (career == null)
            {
                career = new CareerSaveData();
            }

            if (!Enum.TryParse(career.role, out VolleyballRole role))
            {
                role = VolleyballRole.OutsideHitter;
            }

            CharacterArchetype archetype = ArchetypeForRole(role);
            CharacterStats stats = new CharacterStats
            {
                attack = Clamp(career.attack),
                serve = Clamp(career.serve),
                set = Clamp(career.set),
                receive = Clamp(career.receive),
                block = Clamp(career.block),
                jump = Clamp(career.jump),
                speed = Clamp(career.speed),
                stamina = Clamp(70 + career.season * 2),
                technique = Clamp(65 + career.season * 2),
                mental = Clamp(68 + career.season * 2)
            };

            return new RuntimeCharacterProfile(
                "career_player",
                string.IsNullOrWhiteSpace(career.playerName) ? "Rookie" : career.playerName,
                "Career",
                role,
                archetype,
                stats,
                SkillsForRole(role));
        }

        private static CharacterArchetype ArchetypeForRole(VolleyballRole role)
        {
            switch (role)
            {
                case VolleyballRole.Setter: return CharacterArchetype.PrecisionSetter;
                case VolleyballRole.MiddleBlocker: return CharacterArchetype.ReadBlocker;
                case VolleyballRole.Libero: return CharacterArchetype.GuardianLibero;
                case VolleyballRole.Opposite: return CharacterArchetype.PowerAce;
                default: return CharacterArchetype.AllRounder;
            }
        }

        private static string[] SkillsForRole(VolleyballRole role)
        {
            switch (role)
            {
                case VolleyballRole.Setter: return new[] { "career_precision_set" };
                case VolleyballRole.MiddleBlocker: return new[] { "career_read_block" };
                case VolleyballRole.Libero: return new[] { "career_guardian" };
                case VolleyballRole.Opposite: return new[] { "career_power_attack" };
                default: return new[] { "career_all_round" };
            }
        }

        private static int Clamp(int value)
        {
            if (value < 1) return 1;
            return value > 100 ? 100 : value;
        }
    }
}
