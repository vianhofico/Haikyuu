namespace HaikyuuGame.Gameplay.AI
{
    public enum AiDifficulty
    {
        Rookie = 0,
        Normal = 1,
        Advanced = 2,
        Elite = 3,
        National = 4,
        Legend = 5
    }

    public static class AiDifficultyRuntime
    {
        public static AiDifficulty Current { get; private set; } = AiDifficulty.Normal;

        public static float DecisionDelayMultiplier
        {
            get
            {
                switch (Current)
                {
                    case AiDifficulty.Rookie: return 1.35f;
                    case AiDifficulty.Advanced: return 0.90f;
                    case AiDifficulty.Elite: return 0.80f;
                    case AiDifficulty.National: return 0.70f;
                    case AiDifficulty.Legend: return 0.62f;
                    default: return 1f;
                }
            }
        }

        public static float ServeQualityMultiplier
        {
            get
            {
                switch (Current)
                {
                    case AiDifficulty.Rookie: return 0.55f;
                    case AiDifficulty.Advanced: return 1.15f;
                    case AiDifficulty.Elite: return 1.30f;
                    case AiDifficulty.National: return 1.45f;
                    case AiDifficulty.Legend: return 1.60f;
                    default: return 1f;
                }
            }
        }

        public static void Set(AiDifficulty difficulty)
        {
            Current = difficulty;
        }

        public static void SetFromInt(int value)
        {
            if (value < 0) value = 0;
            if (value > 5) value = 5;
            Current = (AiDifficulty)value;
        }

        public static AiDifficulty Cycle()
        {
            int next = ((int)Current + 1) % 6;
            Current = (AiDifficulty)next;
            return Current;
        }
    }
}
