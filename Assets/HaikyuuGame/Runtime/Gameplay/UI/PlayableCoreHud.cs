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
            GUI.Box(new Rect(16f, 16f, 420f, 120f), "Playable Core 0.1");

            if (_rally != null)
            {
                GUI.Label(new Rect(32f, 44f, 380f, 24f), $"LEFT {_rally.Score.Left}   :   {_rally.Score.Right} RIGHT");
            }

            GUI.Label(new Rect(32f, 68f, 380f, 24f), _message);
            GUI.Label(new Rect(32f, 92f, 390f, 24f), "Move WASD/Arrows | Jump Space | Action F/J | Reset R");
        }
    }
}
