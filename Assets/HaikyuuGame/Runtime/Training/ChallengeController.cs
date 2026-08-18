using HaikyuuGame.Core;
using HaikyuuGame.Gameplay.Ball;
using HaikyuuGame.Gameplay.Match;
using HaikyuuGame.Meta;
using HaikyuuGame.Persistence;
using UnityEngine;

namespace HaikyuuGame.Training
{
    public sealed class ChallengeController : MonoBehaviour
    {
        private RallyController _rally;
        private int _lastRevision = -1;
        private int _perfectContacts;
        private bool _completed;
        private string _message = "Land 3 PERFECT contacts and win the match.";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Ensure()
        {
            if (FindFirstObjectByType<ChallengeController>() != null)
            {
                return;
            }

            new GameObject("ChallengeController").AddComponent<ChallengeController>();
        }

        private void Update()
        {
            EnsureBound();
            if (GameSessionState.CurrentMode == GameMode.Challenge
                && _lastRevision != GameSessionState.SessionRevision)
            {
                _lastRevision = GameSessionState.SessionRevision;
                _perfectContacts = 0;
                _completed = false;
                _message = "Land 3 PERFECT contacts and win the match.";
            }
        }

        private void OnDestroy()
        {
            Unbind();
        }

        private void EnsureBound()
        {
            if (_rally != null)
            {
                return;
            }

            _rally = FindFirstObjectByType<RallyController>();
            if (_rally == null)
            {
                return;
            }

            _rally.ContactProcessed += OnContact;
            _rally.MatchCompleted += OnMatchCompleted;
        }

        private void Unbind()
        {
            if (_rally == null)
            {
                return;
            }

            _rally.ContactProcessed -= OnContact;
            _rally.MatchCompleted -= OnMatchCompleted;
            _rally = null;
        }

        private void OnContact(BallContact contact)
        {
            if (GameSessionState.CurrentMode != GameMode.Challenge || _completed)
            {
                return;
            }

            if (_rally.HumanPlayer != null
                && contact.Team == _rally.HumanPlayer.Team
                && contact.Timing == ContactTimingGrade.Perfect)
            {
                _perfectContacts++;
            }
        }

        private void OnMatchCompleted(TeamSide winner)
        {
            if (GameSessionState.CurrentMode != GameMode.Challenge || _completed)
            {
                return;
            }

            bool humanWon = _rally.HumanPlayer != null && winner == _rally.HumanPlayer.Team;
            if (humanWon && _perfectContacts >= 3)
            {
                _completed = true;
                _message = "Challenge complete! +100 coins";
                SaveGameService saveService = new SaveGameService();
                SaveGameData save = saveService.Load();
                save.coins += 100;
                saveService.Save();
            }
            else
            {
                _message = humanWon
                    ? "Win achieved, but you still need 3 PERFECT contacts."
                    : "Challenge failed — win the match after 3 PERFECT contacts.";
            }
        }

        private void OnGUI()
        {
            if (GameSessionState.CurrentMode != GameMode.Challenge)
            {
                return;
            }

            float width = 420f;
            float x = (Screen.width - width) * 0.5f;
            GUI.Box(new Rect(x, 70f, width, 78f), "CHALLENGE — PERFECT PRESSURE");
            GUI.Label(new Rect(x + 18f, 98f, width - 36f, 22f), $"PERFECT contacts: {Mathf.Min(_perfectContacts, 3)}/3");
            GUI.Label(new Rect(x + 18f, 120f, width - 36f, 22f), _message);
        }
    }
}
