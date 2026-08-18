namespace HaikyuuGame.Gameplay.Ball
{
    public enum ContactTimingGrade
    {
        Miss = 0,
        Early = 1,
        Good = 2,
        Perfect = 3,
        Late = 4
    }

    public static class ContactTiming
    {
        public static ContactTimingGrade Evaluate(float distance, float reach, UnityEngine.Vector3 ballVelocity)
        {
            if (reach <= 0.001f || distance > reach)
            {
                return ContactTimingGrade.Miss;
            }

            float ratio = distance / reach;
            if (ratio <= 0.38f)
            {
                return ContactTimingGrade.Perfect;
            }

            if (ratio <= 0.67f)
            {
                return ContactTimingGrade.Good;
            }

            return ballVelocity.y > 0.15f ? ContactTimingGrade.Early : ContactTimingGrade.Late;
        }

        public static float PowerMultiplier(ContactTimingGrade grade)
        {
            switch (grade)
            {
                case ContactTimingGrade.Perfect: return 1.14f;
                case ContactTimingGrade.Good: return 1f;
                case ContactTimingGrade.Early:
                case ContactTimingGrade.Late: return 0.86f;
                default: return 0f;
            }
        }

        public static float ControlMultiplier(ContactTimingGrade grade)
        {
            switch (grade)
            {
                case ContactTimingGrade.Perfect: return 1.12f;
                case ContactTimingGrade.Good: return 1f;
                case ContactTimingGrade.Early:
                case ContactTimingGrade.Late: return 0.90f;
                default: return 0f;
            }
        }
    }
}
