using HaikyuuGame.Gameplay.Skills;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Character
{
    [CreateAssetMenu(menuName = "Haikyuu/Character Definition", fileName = "CharacterDefinition")]
    public sealed class CharacterDefinition : ScriptableObject
    {
        [SerializeField] private string characterId;
        [SerializeField] private string displayName;
        [SerializeField] private CharacterArchetype archetype;
        [SerializeField] private CharacterStats stats = CharacterStats.Default;
        [SerializeField] private SkillDefinition[] skills;

        public string CharacterId => characterId;
        public string DisplayName => displayName;
        public CharacterArchetype Archetype => archetype;
        public CharacterStats Stats => stats;
        public SkillDefinition[] Skills => skills;
    }

    [System.Serializable]
    public struct CharacterStats
    {
        [Range(1, 100)] public int attack;
        [Range(1, 100)] public int serve;
        [Range(1, 100)] public int set;
        [Range(1, 100)] public int receive;
        [Range(1, 100)] public int block;
        [Range(1, 100)] public int jump;
        [Range(1, 100)] public int speed;
        [Range(1, 100)] public int stamina;
        [Range(1, 100)] public int technique;
        [Range(1, 100)] public int mental;

        public static CharacterStats Default => new CharacterStats
        {
            attack = 50,
            serve = 50,
            set = 50,
            receive = 50,
            block = 50,
            jump = 50,
            speed = 50,
            stamina = 50,
            technique = 50,
            mental = 50
        };
    }
}
