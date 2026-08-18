using System;
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
        private readonly TeamPossession _possession = new TeamPossession();
        private readonly TeamMomentum _momentum = new TeamMomentum();
        private readonly List<PlayerActor> _players = new List<PlayerActor>();

        private VolleyballBall _ball;
        private VolleyballTuning _tuning;
        private PlayableCoreHud _hud;
        private TeamRotation _leftRotation;
        private TeamRotation _rightRotation;
        private TeamSide _servingTeam = TeamSide.Left;
        private bool _rallyActive;
        private bool _resolvingPoint;

        public event Action<TeamSide> MatchCompleted;

        public MatchScore Score => _score;
        public TeamPossession Possession => _possession;
        public TeamMomentum Momentum => _momentum;
        public TeamSide ServingTeam => _servingTeam;
        public bool RallyActive => _rallyActive;
        public PlayerActor HumanPlayer { get; private set; }

        public void Initialize(
            VolleyballBall ball,
            IEnumerable<PlayerActor> players,
            VolleyballTuning tuning,
            PlayableCoreHud hud,
            TeamRotation leftRotation,
            TeamRotation rightRotation)
        {
            _ball = ball;
            _tuning = tuning;
            _hud = hud;
            _leftRotation = leftRotation;
            _rightRotation = rightRotation;
            _players.AddRange(players);

            for (int i = 0; i < _players.Count; i++)
            {
                _players[i].BindMatch(this);
                if (_players[i].IsHuman)
                {
                    HumanPlayer = _players[i];
                }
            }

            _ball.Contacted += OnBallContact;
            _hud.Bind(this);
            StartNewRally();
        }

        private void OnDestroy()
        {
            if (_ball != null)
            {
                _ball.Contacted -= OnBallContact;
            }
        }

        private void Update()
        {
            if (_ball == null || _resolvingPoint)
            {
                return;
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.R))
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

        private void OnBallContact(BallContact contact)
        {
            if (!_rallyActive && contact.Type != BallContactType.Serve)
            {
                return;
            }

            PossessionUpdate update = _possession.Register(contact);
            string actor = contact.Player != null ? contact.Player.DisplayName : "Server";
            _hud.SetMessage($"{contact.Team} {actor}: {contact.Type} | touches {_possession.CountedTouches}/3");

            if (contact.Type == BallContactType.Attack)
            {
                _momentum.Add(contact.Team, 1.5f);
            }
            else if (contact.Type == BallContactType.Block)
            {
                _momentum.Add(contact.Team, 4f);
            }
            else if (contact.Type == BallContactType.Dig)
            {
                _momentum.Add(contact.Team, 1f);
            }

            if (update.Fault)
            {
                ResolvePoint(Opposite(update.FaultingTeam), "Four contacts");
            }
        }

        private void ResolvePoint(TeamSide scorer, string reason)
        {
            if (_resolvingPoint || scorer == TeamSide.None)
            {
                return;
            }

            _resolvingPoint = true;
            _rallyActive = false;
            _momentum.PointTo(scorer);

            bool sideOut = scorer != _servingTeam;
            if (sideOut)
            {
                TeamRotation rotation = scorer == TeamSide.Left ? _leftRotation : _rightRotation;
                rotation?.RotateClockwise();
            }

            _servingTeam = scorer;
            ScoreUpdate update = _score.AddPoint(
                scorer,
                _tuning.quickMatch,
                _tuning.quickSetTargetScore,
                _tuning.standardSetTargetScore,
                _tuning.decidingSetTargetScore,
                _tuning.setsToWin);

            if (update.MatchWon)
            {
                _hud.SetMessage($"{update.Winner} wins the match! Restarting...");
                MatchCompleted?.Invoke(update.Winner);
                StartCoroutine(RestartMatchAfterDelay());
                return;
            }

            if (update.SetWon)
            {
                _momentum.ResetSet();
                _hud.SetMessage($"Set won by {scorer}. Next set starts shortly.");
                StartCoroutine(ResetAfterSet());
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

        private IEnumerator ResetAfterSet()
        {
            yield return new WaitForSeconds(_tuning.rallyResetDelay * 1.6f);
            StartNewRally();
        }

        private IEnumerator RestartMatchAfterDelay()
        {
            yield return new WaitForSeconds(_tuning.rallyResetDelay * 2f);
            _score.ResetMatch();
            _momentum.ResetMatch();
            _leftRotation?.ResetRotation();
            _rightRotation?.ResetRotation();
            _servingTeam = TeamSide.Left;
            StartNewRally();
        }

        private void StartNewRally()
        {
            StopAllCoroutines();
            _resolvingPoint = false;
            _rallyActive = false;
            _possession.Reset();

            _leftRotation?.ResetPlayersToHome();
            _rightRotation?.ResetPlayersToHome();

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
            Vector3 serveVelocity = new Vector3(direction * 8.8f, 5.8f, Random.Range(-0.75f, 0.75f));
            _rallyActive = true;
            _ball.WakeAndServe(_servingTeam, serveVelocity);
            _hud.SetMessage("Rally!");
        }

        public static TeamSide Opposite(TeamSide team)
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
