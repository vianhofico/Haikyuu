using HaikyuuGame.Persistence;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Presentation
{
    public static class RuntimePresentationSettings
    {
        public static bool ScreenShake { get; private set; } = true;
        public static bool ReducedCinematics { get; private set; }
        public static bool HighContrastUi { get; private set; }
        public static float MasterVolume { get; private set; } = 1f;
        public static float SfxVolume { get; private set; } = 1f;

        public static void Apply(GameSettingsSaveData settings)
        {
            if (settings == null)
            {
                return;
            }

            ScreenShake = settings.screenShake;
            ReducedCinematics = settings.reducedCinematics;
            HighContrastUi = settings.highContrastUi;
            MasterVolume = Mathf.Clamp01(settings.masterVolume);
            SfxVolume = Mathf.Clamp01(settings.sfxVolume);
            AudioListener.volume = MasterVolume;
        }
    }
}
