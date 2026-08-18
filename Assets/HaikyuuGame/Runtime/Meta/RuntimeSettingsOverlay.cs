using HaikyuuGame.Gameplay.AI;
using HaikyuuGame.Gameplay.Presentation;
using HaikyuuGame.Persistence;
using UnityEngine;

namespace HaikyuuGame.Meta
{
    public sealed class RuntimeSettingsOverlay : MonoBehaviour
    {
        private GameShellController _shell;
        private bool _appliedInitial;
        private bool _visible;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Ensure()
        {
            if (FindFirstObjectByType<RuntimeSettingsOverlay>() != null)
            {
                return;
            }

            new GameObject("RuntimeSettingsOverlay").AddComponent<RuntimeSettingsOverlay>();
        }

        private void Update()
        {
            if (_shell == null)
            {
                _shell = FindFirstObjectByType<GameShellController>();
            }

            if (!_appliedInitial && _shell != null && _shell.Save != null)
            {
                _appliedInitial = true;
                ApplyRuntime(_shell.Save.settings);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.F10))
            {
                _visible = !_visible;
            }
        }

        private void OnGUI()
        {
            if (!_visible || _shell == null || _shell.Save == null)
            {
                return;
            }

            GameSettingsSaveData settings = _shell.Save.settings;
            float width = 460f;
            float height = 270f;
            float x = (Screen.width - width) * 0.5f;
            float y = (Screen.height - height) * 0.5f;
            GUI.Box(new Rect(x, y, width, height), "SETTINGS — F10 to close");

            float row = y + 42f;
            GUI.Label(new Rect(x + 24f, row, 220f, 26f), $"AI Difficulty: {AiDifficultyRuntime.Current}");
            if (GUI.Button(new Rect(x + width - 148f, row - 3f, 124f, 30f), "Next Difficulty"))
            {
                settings.aiDifficulty = (int)AiDifficultyRuntime.Cycle();
                ApplyAndSave(settings);
            }

            row += 42f;
            GUI.Label(new Rect(x + 24f, row, 220f, 26f), $"Screen Shake: {(settings.screenShake ? "ON" : "OFF")}");
            if (GUI.Button(new Rect(x + width - 148f, row - 3f, 124f, 30f), "Toggle"))
            {
                settings.screenShake = !settings.screenShake;
                ApplyAndSave(settings);
            }

            row += 42f;
            GUI.Label(new Rect(x + 24f, row, 220f, 26f), $"Reduced Cinematics: {(settings.reducedCinematics ? "ON" : "OFF")}");
            if (GUI.Button(new Rect(x + width - 148f, row - 3f, 124f, 30f), "Toggle"))
            {
                settings.reducedCinematics = !settings.reducedCinematics;
                ApplyAndSave(settings);
            }

            row += 42f;
            DrawVolumeRow(x, row, width, "Master Volume", settings.masterVolume, value =>
            {
                settings.masterVolume = value;
                ApplyAndSave(settings);
            });

            row += 42f;
            DrawVolumeRow(x, row, width, "SFX Volume", settings.sfxVolume, value =>
            {
                settings.sfxVolume = value;
                ApplyAndSave(settings);
            });
        }

        private static void DrawVolumeRow(float x, float y, float width, string label, float value, System.Action<float> set)
        {
            GUI.Label(new Rect(x + 24f, y, 220f, 26f), $"{label}: {Mathf.RoundToInt(value * 100f)}%");
            if (GUI.Button(new Rect(x + width - 148f, y - 3f, 54f, 30f), "-"))
            {
                set(Mathf.Clamp01(value - 0.1f));
            }
            if (GUI.Button(new Rect(x + width - 82f, y - 3f, 54f, 30f), "+"))
            {
                set(Mathf.Clamp01(value + 0.1f));
            }
        }

        private void ApplyAndSave(GameSettingsSaveData settings)
        {
            ApplyRuntime(settings);
            _shell.CommitSave();
        }

        private static void ApplyRuntime(GameSettingsSaveData settings)
        {
            AiDifficultyRuntime.SetFromInt(settings.aiDifficulty);
            RuntimePresentationSettings.Apply(settings);
        }
    }
}
