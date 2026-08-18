using HaikyuuGame.Core;
using HaikyuuGame.Gameplay.Ball;
using HaikyuuGame.Gameplay.Match;
using UnityEngine;

namespace HaikyuuGame.Gameplay.UI
{
    public sealed class MatchStatisticsOverlay : MonoBehaviour
    {
        private sealed class Line
        {
            public int points;
            public int attacks;
            public int blocks;
            public int digs;
            public int serves;
            public int perfects;

            public void Reset()
            {
                points = 0;
                attacks = 0;
                blocks = 0;
                digs = 0;
                serves = 0;
                perfects = 0;
            }
        }

        private readonly Line _left = new Line();
        private readonly Line _right = new Line();
        private RallyController _rally;
        private bool _visible;
        private string _lastMatchSummary = string.Empty;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Ensure()
        {
            if (FindFirstObjectByType<MatchStatisticsOverlay>() != null)
            {
                return;
            }

            new GameObject("MatchStatisticsOverlay").AddComponent<MatchStatisticsOverlay>();
        }

        private void Update()
        {
            if (_rally == null)
            {
                Bind(FindFirstObjectByType<RallyController>());
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.F2))
            {
                _visible = !_visible;
            }
        }

        private void OnDestroy()
        {
            Unbind();
        }

        private void Bind(RallyController rally)
        {
            if (rally == null || rally == _rally)
            {
                return;
            }

            Unbind();
            _rally = rally;
            _rally.ContactProcessed += OnContact;
            _rally.PointScored += OnPoint;
            _rally.MatchCompleted += OnMatchCompleted;
        }

        private void Unbind()
        {
            if (_rally == null)
            {
                return;
            }

            _rally.ContactProcessed -= OnContact;
            _rally.PointScored -= OnPoint;
            _rally.MatchCompleted -= OnMatchCompleted;
            _rally = null;
        }

        private void OnContact(BallContact contact)
        {
            Line line = For(contact.Team);
            if (line == null) return;

            switch (contact.Type)
            {
                case BallContactType.Serve: line.serves++; break;
                case BallContactType.Attack: line.attacks++; break;
                case BallContactType.Block: line.blocks++; break;
                case BallContactType.Dig:
                case BallContactType.Receive: line.digs++; break;
            }

            if (contact.Timing == ContactTimingGrade.Perfect)
            {
                line.perfects++;
            }
        }

        private void OnPoint(TeamSide scorer)
        {
            Line line = For(scorer);
            if (line != null) line.points++;
        }

        private void OnMatchCompleted(TeamSide winner)
        {
            _lastMatchSummary = $"Last match: {winner} | L {_left.points} pts/{_left.attacks} atk/{_left.blocks} blk/{_left.perfects} perfect — "
                + $"R {_right.points} pts/{_right.attacks} atk/{_right.blocks} blk/{_right.perfects} perfect";
            _left.Reset();
            _right.Reset();
        }

        private Line For(TeamSide side)
        {
            if (side == TeamSide.Left) return _left;
            if (side == TeamSide.Right) return _right;
            return null;
        }

        private void OnGUI()
        {
            if (!_visible || _rally == null)
            {
                return;
            }

            const float width = 520f;
            float x = 16f;
            float y = Screen.height - 196f;
            GUI.Box(new Rect(x, y, width, 180f), "MATCH STATISTICS — F2 to close");
            GUI.Label(new Rect(x + 16f, y + 34f, width - 32f, 22f), "TEAM      PTS   SERVE   ATTACK   BLOCK   RECEIVE/DIG   PERFECT");
            DrawLine(x, y + 60f, "LEFT", _left);
            DrawLine(x, y + 84f, "RIGHT", _right);
            GUI.Label(new Rect(x + 16f, y + 114f, width - 32f, 22f), $"Scoreboard: {_rally.Score.Left}-{_rally.Score.Right} | Flow L {_rally.Momentum.Get(TeamSide.Left):0} / R {_rally.Momentum.Get(TeamSide.Right):0}");
            if (!string.IsNullOrEmpty(_lastMatchSummary))
            {
                GUI.Label(new Rect(x + 16f, y + 140f, width - 32f, 34f), _lastMatchSummary);
            }
        }

        private static void DrawLine(float x, float y, string name, Line line)
        {
            GUI.Label(
                new Rect(x + 16f, y, 490f, 22f),
                $"{name,-8} {line.points,3}   {line.serves,5}   {line.attacks,6}   {line.blocks,5}   {line.digs,11}   {line.perfects,7}");
        }
    }
}
