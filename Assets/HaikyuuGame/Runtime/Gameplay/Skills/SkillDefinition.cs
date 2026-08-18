using UnityEngine;

namespace HaikyuuGame.Gameplay.Skills
{
    public enum SkillCategory
    {
        Passive,
        Active,
        Reaction,
        Combo,
        Ultimate
    }

    [CreateAssetMenu(menuName = "Haikyuu/Skill Definition", fileName = "SkillDefinition")]
    public sealed class SkillDefinition : ScriptableObject
    {
        [SerializeField] private string skillId;
        [SerializeField] private string displayName;
        [TextArea] [SerializeField] private string description;
        [SerializeField] private SkillCategory category;
        [Min(0f)] [SerializeField] private float cooldown;

        public string SkillId => skillId;
        public string DisplayName => displayName;
        public string Description => description;
        public SkillCategory Category => category;
        public float Cooldown => cooldown;
    }
}
