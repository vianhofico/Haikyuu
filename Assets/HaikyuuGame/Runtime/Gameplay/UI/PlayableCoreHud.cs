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
            GUI.Box(new Rect(16f, 16f, 590f, 198f), "Playable Core 0.4 - Anime Volleyball");

            if (_rally != null)
            {
                GUI.Label(
                    new Rect(32f, 44f, 550f, 24f),
                    $"SETS {_rally.Score.LeftSets}-{_rally.Score.RightSets}   |   POINTS {_rally.Score.Left}-{_rally.Score.Right}   |   Serve: {_rally.ServingTeam}");
                GUI.Label(
                    new Rect(32f, 68f, 550f, 24f),
                    $"Possession: {_rally.Possession.Team} | Touches: {_rally.Possession.CountedTouches}/3 | Last: {_rally.Possession.LastContactType}");
                GUI.Label(
                    new Rect(32f, 92f, 550f, 24f),
                    $"FLOW  Left {_rally.Momentum.Left:0}  |  Right {_rally.Momentum.Right:0}");

                if (_rally.HumanPlayer != null)
                {
                    GUI.Label(
                        new Rect(32f, 116f, 550f, 24f),
                        $"YOU: {_rally.HumanPlayer.DisplayName} | {_rally.HumanPlayer.BaseRole} | slot {_rally.HumanPlayer.CourtSlot + 1}");
                }
            }

            GUI.Label(new Rect(32f, 140f, 550f, 24f), _message);
            GUI.Label(new Rect(32f, 164f, 550f, 24f), "Move WASD | Jump Space | Context F/J | Z Receive | X Set | C Spike | V Block | R Reset");
        }
    }
}
