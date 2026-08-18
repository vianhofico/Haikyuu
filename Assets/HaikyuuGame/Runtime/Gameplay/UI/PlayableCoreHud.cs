using HaikyuuGame.Gameplay.Match;
using UnityEngine;

namespace HaikyuuGame.Gameplay.UI
{
    public sealed class PlayableCoreHud : MonoBehaviour
    {
        private RallyController _rally;
        private string _message = "Initializing...";

        public void Bind(RallyController rally)
        {
            _rally = rally;
        }

        public void SetMessage(string message)
        {
            _message = message;
        }

        private void OnGUI()
        {
            GUI.Box(new Rect(16f, 16f, 610f, 220f), "Playable Core 0.5 - Timing + Manual Serve");

            if (_rally != null)
            {
                GUI.Label(new Rect(32f, 44f, 570f, 24f), $"SETS {_rally.Score.LeftSets}-{_rally.Score.RightSets} | POINTS {_rally.Score.Left}-{_rally.Score.Right} | Serve: {_rally.ServingTeam}");
                GUI.Label(new Rect(32f, 68f, 570f, 24f), $"Possession: {_rally.Possession.Team} | Touches: {_rally.Possession.CountedTouches}/3 | Last: {_rally.Possession.LastContactType}");
                GUI.Label(new Rect(32f, 92f, 570f, 24f), $"FLOW Left {_rally.Momentum.Left:0} | Right {_rally.Momentum.Right:0}");

                if (_rally.HumanPlayer != null)
                {
                    GUI.Label(new Rect(32f, 116f, 570f, 24f), $"YOU: {_rally.HumanPlayer.DisplayName} | {_rally.HumanPlayer.BaseRole} | slot {_rally.HumanPlayer.CourtSlot + 1}");
                }

                if (_rally.AwaitingHumanServe)
                {
                    DrawServeMeter(_rally.ServeMeter01);
                }
            }

            GUI.Label(new Rect(32f, 164f, 570f, 24f), _message);
            GUI.Label(new Rect(32f, 188f, 570f, 24f), "Move WASD | Jump Space | Context F/J | Z Receive | X Set | C Spike/Serve | V Block | R Reset");
        }

        private static void DrawServeMeter(float value)
        {
            const float x = 32f;
            const float y = 142f;
            const float width = 290f;
            const float height = 14f;
            GUI.Box(new Rect(x, y, width, height), GUIContent.none);
            float markerX = x + Mathf.Clamp01(value) * width;
            GUI.Box(new Rect(markerX - 3f, y - 3f, 6f, height + 6f), GUIContent.none);
            float perfectX = x + 0.82f * width;
            GUI.Label(new Rect(perfectX - 28f, y - 22f, 90f, 20f), "PERFECT");
        }
    }
}
