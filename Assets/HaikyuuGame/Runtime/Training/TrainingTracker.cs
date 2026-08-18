using HaikyuuGame.Gameplay.Ball;
using HaikyuuGame.Gameplay.Match;
using HaikyuuGame.Meta;
using UnityEngine;

namespace HaikyuuGame.Training
{
    public sealed class TrainingTracker : MonoBehaviour
    {
        private RallyController _rally;
        private int _revision = -1;
        private int _receives;
        private int _sets;
        private int _attacks;
        private int _blocks;

        public void Initialize(RallyController rally)
        {
            _rally = rally;
            _rally.ContactProcessed += OnContact;
            ResetProgress();
        }

        private void OnDestroy()
        {
            if (_rally != null)
            {
                _rally.ContactProcessed -= OnContact;
            }
        }

        private void Update()
        {
            if (_revision != GameSessionState.SessionRevision)
            {
                ResetProgress();
            }
        }

        private void OnContact(BallContact contact)
        {
            if (GameSessionState.CurrentMode != GameMode.Training
                && GameSessionState.CurrentMode != GameMode.Challenge)
            {
                return;
            }

            if (contact.Player == null || !contact.Player.IsHuman)
            {
                return;
            }

            switch (contact.Type)
            {
                case BallContactType.Receive:
                case BallContactType.Dig:
                    _receives++;
                    break;
                case BallContactType.Set:
                    _sets++;
                    break;
                case BallContactType.Attack:
                    _attacks++;
                    break;
                case BallContactType.Block:
                    _blocks++;
                    break;
            }
        }

        private void ResetProgress()
        {
            _revision = GameSessionState.SessionRevision;
            _receives = 0;
            _sets = 0;
            _attacks = 0;
            _blocks = 0;
        }

        private void OnGUI()
        {
            if (GameSessionState.CurrentMode != GameMode.Training
                && GameSessionState.CurrentMode != GameMode.Challenge)
            {
                return;
            }

            float x = 16f;
            float y = Screen.height - 156f;
            GUI.Box(new Rect(x, y, 270f, 140f), GameSessionState.CurrentMode == GameMode.Training ? "TRAINING" : "CHALLENGE");
            GUI.Label(new Rect(x + 16f, y + 28f, 240f, 22f), $"Receive / Dig: {_receives}/5");
            GUI.Label(new Rect(x + 16f, y + 50f, 240f, 22f), $"Set: {_sets}/3");
            GUI.Label(new Rect(x + 16f, y + 72f, 240f, 22f), $"Attack: {_attacks}/5");
            GUI.Label(new Rect(x + 16f, y + 94f, 240f, 22f), $"Block: {_blocks}/2");

            if (_receives >= 5 && _sets >= 3 && _attacks >= 5 && _blocks >= 2)
            {
                GUI.Label(new Rect(x + 16f, y + 116f, 240f, 22f), "COMPLETE!");
            }
        }
    }
}
