using HaikyuuGame.Gameplay.Player;

namespace HaikyuuGame.Gameplay.Character
{
    public sealed class RuntimeCharacterProfile
    {
        public RuntimeCharacterProfile(
            string id,
            string displayName,
            string school,
            VolleyballRole role,
            CharacterArchetype archetype,
            CharacterStats stats,
            params string[] skillIds)
        {
            Id = id;
            DisplayName = displayName;
            School = school;
            Role = role;
            Archetype = archetype;
            Stats = stats;
            SkillIds = skillIds ?? new string[0];
        }

        public string Id { get; }
        public string DisplayName { get; }
        public string School { get; }
        public VolleyballRole Role { get; }
        public CharacterArchetype Archetype { get; }
        public CharacterStats Stats { get; }
        public string[] SkillIds { get; }
    }
}
