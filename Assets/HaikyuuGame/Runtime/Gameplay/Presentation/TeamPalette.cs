using UnityEngine;

namespace HaikyuuGame.Gameplay.Presentation
{
    public readonly struct TeamColors
    {
        public TeamColors(Color primary, Color secondary, Color accent)
        {
            Primary = primary;
            Secondary = secondary;
            Accent = accent;
        }

        public Color Primary { get; }
        public Color Secondary { get; }
        public Color Accent { get; }
    }

    public static class TeamPalette
    {
        public static TeamColors Get(string school)
        {
            switch (school)
            {
                case "Karasuno": return new TeamColors(new Color(0.08f, 0.08f, 0.1f), new Color(0.95f, 0.34f, 0.03f), Color.white);
                case "Nekoma": return new TeamColors(new Color(0.12f, 0.05f, 0.05f), new Color(0.72f, 0.08f, 0.08f), Color.white);
                case "Aoba Johsai": return new TeamColors(new Color(0.88f, 0.92f, 0.9f), new Color(0.16f, 0.55f, 0.48f), new Color(0.12f, 0.22f, 0.2f));
                case "Date Tech": return new TeamColors(new Color(0.1f, 0.25f, 0.24f), new Color(0.68f, 0.78f, 0.75f), Color.white);
                case "Shiratorizawa": return new TeamColors(new Color(0.28f, 0.08f, 0.34f), new Color(0.92f, 0.9f, 0.88f), new Color(0.42f, 0.2f, 0.5f));
                case "Fukurodani": return new TeamColors(new Color(0.1f, 0.1f, 0.12f), new Color(0.85f, 0.72f, 0.18f), Color.white);
                case "Inarizaki": return new TeamColors(new Color(0.12f, 0.12f, 0.13f), new Color(0.9f, 0.88f, 0.82f), new Color(0.55f, 0.1f, 0.12f));
                case "Kamomedai": return new TeamColors(new Color(0.18f, 0.38f, 0.45f), new Color(0.88f, 0.92f, 0.93f), new Color(0.12f, 0.2f, 0.24f));
                default: return new TeamColors(new Color(0.15f, 0.25f, 0.45f), new Color(0.9f, 0.9f, 0.92f), Color.white);
            }
        }
    }
}
