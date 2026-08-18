using HaikyuuGame.Gameplay.Ball;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Presentation
{
    public sealed class PerfectContactFeedback : MonoBehaviour
    {
        private VolleyballBall _ball;
        private string _label;
        private string _speedLabel;
        private float _until;

        public void Initialize(VolleyballBall ball)
        {
            _ball = ball;
            _ball.Contacted += OnContact;
        }

        private void OnDestroy()
        {
            if (_ball != null)
            {
                _ball.Contacted -= OnContact;
            }
        }

        private void OnContact(BallContact contact)
        {
            if (contact.Timing != ContactTimingGrade.Perfect)
            {
                return;
            }

            _label = contact.Type == BallContactType.Attack
                ? "PERFECT SPIKE"
                : $"PERFECT {contact.Type.ToString().ToUpperInvariant()}";
            float kmh = contact.Velocity.magnitude * 3.6f;
            _speedLabel = contact.Type == BallContactType.Attack || contact.Type == BallContactType.Serve
                ? $"{kmh:0} KM/H"
                : string.Empty;
            _until = Time.unscaledTime + 0.45f;
        }

        private void OnGUI()
        {
            if (Time.unscaledTime >= _until)
            {
                return;
            }

            float width = 360f;
            float x = (Screen.width - width) * 0.5f;
            GUI.Box(new Rect(x, Screen.height * 0.22f, width, 68f), _label);
            if (!string.IsNullOrEmpty(_speedLabel))
            {
                GUI.Label(new Rect(x + 135f, Screen.height * 0.22f + 34f, 180f, 26f), _speedLabel);
            }
        }
    }
}
