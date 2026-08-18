namespace HaikyuuGame.Gameplay.Character
{
    public static class ArchetypeModifiers
    {
        public static float Attack(CharacterArchetype archetype)
        {
            switch (archetype)
            {
                case CharacterArchetype.PowerAce: return 1.16f;
                case CharacterArchetype.MomentumAce: return 1.10f;
                case CharacterArchetype.TechnicalAce: return 1.07f;
                case CharacterArchetype.FlexibleHitter: return 1.06f;
                default: return 1f;
            }
        }

        public static float Jump(CharacterArchetype archetype)
        {
            switch (archetype)
            {
                case CharacterArchetype.SpeedDecoy: return 1.12f;
                case CharacterArchetype.HighReach: return 1.08f;
                case CharacterArchetype.TechnicalAce: return 1.05f;
                default: return 1f;
            }
        }

        public static float Move(CharacterArchetype archetype)
        {
            switch (archetype)
            {
                case CharacterArchetype.SpeedDecoy: return 1.12f;
                case CharacterArchetype.GuardianLibero: return 1.08f;
                case CharacterArchetype.WildCard: return 1.06f;
                default: return 1f;
            }
        }

        public static float ReceiveReach(CharacterArchetype archetype)
        {
            switch (archetype)
            {
                case CharacterArchetype.GuardianLibero: return 1.22f;
                case CharacterArchetype.ReliableCaptain: return 1.10f;
                case CharacterArchetype.AllRounder: return 1.05f;
                default: return 1f;
            }
        }

        public static float Block(CharacterArchetype archetype)
        {
            switch (archetype)
            {
                case CharacterArchetype.PowerBlocker: return 1.16f;
                case CharacterArchetype.ReadBlocker: return 1.11f;
                case CharacterArchetype.GuessBlocker: return 1.12f;
                case CharacterArchetype.HighReach: return 1.08f;
                default: return 1f;
            }
        }

        public static float SetControl(CharacterArchetype archetype)
        {
            switch (archetype)
            {
                case CharacterArchetype.PrecisionSetter: return 1.14f;
                case CharacterArchetype.StrategistSetter: return 1.12f;
                case CharacterArchetype.TeamAmplifierSetter: return 1.10f;
                case CharacterArchetype.DualServerSetter: return 1.08f;
                default: return 1f;
            }
        }
    }
}
