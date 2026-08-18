using System.Collections;
using System.Collections.Generic;
using HaikyuuGame.Core;
using HaikyuuGame.Gameplay.Ball;
using HaikyuuGame.Gameplay.Player;
using HaikyuuGame.Gameplay.UI;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Match
{
    public sealed class RallyController : MonoBehaviour
    {
        private readonly MatchScore _score = new MatchScore();
        private readonly List<PlayerActor> _players = new List<PlayerActor>();

        private VolleyballBall _ball;
        private VolleyballTuning _tuning;
        private PlayableCoreHud _hud;
        private TeamSide _servingTeam = TeamSide.Left;
        private bool _rallyActive;
        private bool _resolvingPoint;

        public MatchScore Score => _score;
        public TeamSide ServingTeam => _servingTeam;

        public void Initialize(
            VolleyballBall ball,
            IEnumerable<PlayerActor> players,
            VolleyballTuning tuning,
            PlayableCoreHud hud)
        {
            _ball = ball;
            _tuning = tuning;
            _hud = hud;
            _players.AddRange(players);
            _hud.Bind(this);
            StartNewRally();
        }

        private void Update()
        {
            if (_ball == null || _resolvingPoint)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                StartNewRally();
                return;
            }

            Vector3 ballPosition = _ball.transform.position;
            bool overCourt = Mathf.Abs(ballPosition.x) <= _tuning.halfCourtLength + 0.15f
                && Mathf.Abs(ballPosition.z) <= _tuning.halfCourtWidth + 0.15f;

            if (_rallyActive && overCourt && ballPosition.y <= 0.23f)
            {
                TeamSide scorer = ballPosition.x < 0f ? TeamSide.Right : TeamSide.Left;
                ResolvePoint(scorer, "Ball grounded");
            }
            else if (_rallyActive && !overCourt && ballPosition.y <= -0.35f)
            {
                TeamSide scorer = Opposite(_ball.LastTouchTeam);
                if (scorer == TeamSide.None)
                {
                    scorer = Opposite(_servingTeam);
                }

                ResolvePoint(scorer, "Out");
            }
        }

        private void ResolvePoint(TeamSide scorer, string reason)
        {
            if (_resolvingPoint)
            {
                return;
            }

            _resolvingPoint = true;
            _rallyActive = false;
            _score.AddPoint(scorer);
            _servingTeam = scorer;

            if (_score.HasWinner(_tuning.quickSetTargetScore, out TeamSide winner))
            {
                _hud.SetMessage($"{winner} wins the prototype set! Press R or wait for restart.");
                StartCoroutine(RestartMatchAfterDelay());
                return;
            }

            _hud.SetMessage($"Point: {scorer} ({reason})");
            StartCoroutine(ResetAfterPoint());
        }

        private IEnumerator ResetAfterPoint()
        {
            yield return new WaitForSeconds(_tuning.rallyResetDelay);
            StartNewRally();
        }

        private IEnumerator RestartMatchAfterDelay()
        {
            yield return new WaitForSeconds(_tuning.rallyResetDelay * 2f);
            _score.Reset();
            StartNewRally();
        }

        private void StartNewRally()
        {
            StopAllCoroutines();
            _resolvingPoint = false;
            _rallyActive = false;

            for (int i = 0; i < _players.Count; i++)
            {
                _players[i].ResetToHome();
            }

            float serveX = _servingTeam == TeamSide.Left ? -7.2f : 7.2f;
            Vector3 servePosition = new Vector3(serveX, 2.05f, 0f);
            _ball.ResetBall(servePosition);
            _hud.SetMessage($"{_servingTeam} to serve");
            StartCoroutine(AutoServe());
        }

        private IEnumerator AutoServe()
        {
            yield return new WaitForSeconds(_tuning.autoServeDelay);
            float direction = _servingTeam == TeamSide.Left ? 1f : -1f;
            Vector3 serveVelocity = new Vector3(direction * 8.4f, 5.6f, Random.Range(-0.65f, 0.65f));
            _ball.WakeAndServe(_servingTeam, serveVelocity);
            _rallyActive = true;
            _hud.SetMessage("Rally!");
        }

        private static TeamSide Opposite(TeamSide team)
        {
            if (team == TeamSide.Left)
            {
                return TeamSide.Right;
            }

            if (team == TeamSide.Right)
            {
                return TeamSide.Left;
            }

            return TeamSide.None;
        }
    }
}
