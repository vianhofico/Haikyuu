using System;
using System.Collections;
using HaikyuuGame.Gameplay.Ball;
using HaikyuuGame.Gameplay.Match;
using UnityEngine;

namespace HaikyuuGame.Diagnostics
{
    public sealed class CiSmokeProbe : MonoBehaviour
    {
        private const float SmokeDurationSeconds = 8f;
        private bool _sawFatalLog;
        private string _fatalMessage;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InstallWhenRequested()
        {
            string[] args = Environment.GetCommandLineArgs();
            bool requested = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (string.Equals(args[i], "-ciSmoke", StringComparison.OrdinalIgnoreCase))
                {
                    requested = true;
                    break;
                }
            }

            if (!requested)
            {
                return;
            }

            GameObject probe = new GameObject("CI Smoke Probe");
            DontDestroyOnLoad(probe);
            probe.AddComponent<CiSmokeProbe>();
        }

        private void OnEnable()
        {
            Application.logMessageReceived += OnLogMessage;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= OnLogMessage;
        }

        private IEnumerator Start()
        {
            Debug.Log("CI_SMOKE_START");
            float started = Time.realtimeSinceStartup;

            while (Time.realtimeSinceStartup - started < SmokeDurationSeconds)
            {
                if (_sawFatalLog)
                {
                    Fail(_fatalMessage);
                    yield break;
                }

                yield return null;
            }

            PlayableCoreBootstrap bootstrap = FindFirstObjectByType<PlayableCoreBootstrap>();
            RallyController rally = FindFirstObjectByType<RallyController>();
            VolleyballBall ball = FindFirstObjectByType<VolleyballBall>();

            if (bootstrap == null)
            {
                Fail("PlayableCoreBootstrap was not created.");
                yield break;
            }

            if (rally == null)
            {
                Fail("RallyController was not initialized.");
                yield break;
            }

            if (ball == null)
            {
                Fail("VolleyballBall was not initialized.");
                yield break;
            }

            Debug.Log(
                $"CI_SMOKE_PASS rallyActive={rally.RallyActive} servingTeam={rally.ServingTeam} "
                + $"score={rally.Score.Left}-{rally.Score.Right}");
            Application.Quit(0);
        }

        private void OnLogMessage(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Exception && type != LogType.Assert)
            {
                return;
            }

            _sawFatalLog = true;
            _fatalMessage = condition + "\n" + stackTrace;
        }

        private void Fail(string reason)
        {
            Debug.LogError("CI_SMOKE_FAIL: " + reason);
            Application.Quit(2);
        }
    }
}
