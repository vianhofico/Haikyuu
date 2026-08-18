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
        private float _serveMeterStartedAt;

        public event Action<TeamSide> MatchCompleted;
        public event Action<TeamSide> PointScored;
        public event Action<BallContact> ContactProcessed;

        public MatchScore Score => _score;
        public TeamPossession Possession => _possession;
        public TeamMomentum Momentum => _momentum;
        public TeamSide ServingTeam => _servingTeam;
        public bool RallyActive => _rallyActive;
        public bool AwaitingHumanServe { get; private set; }
        public PlayerActor CurrentServer { get; private set; }
        public PlayerActor HumanPlayer { get; private set; }

        public float ServeMeter01
        {
            get
            {
                if (!AwaitingHumanServe)
                {
                    return 0f;
                }

                float elapsed = Time.time - _serveMeterStartedAt;
                return (Mathf.Sin(elapsed * 3.25f - (Mathf.PI * 0.5f)) + 1f) * 0.5f;
            }
        }

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

            if (!_rallyActive)
            {
                return;
            }

            Vector3 ballPosition = _ball.transform.position;
            bool overCourt = Mathf.Abs(ballPosition.x) <= _tuning.halfCourtLength + 0.15f
                && Mathf.Abs(ballPosition.z) <= _tuning.halfCourtWidth + 0.15f;

            if (overCourt && ballPosition.y <= 0.23f)
            {
                TeamSide scorer = ballPosition.x < 0f ? TeamSide.Right : TeamSide.Left;
                ResolvePoint(scorer, "Ball grounded");
            }
            else if (!overCourt && ballPosition.y <= -0.35f)
            {
                TeamSide scorer = Opposite(_ball.LastTouchTeam);
                if (scorer == TeamSide.None)
                {
                    scorer = Opposite(_servingTeam);
                }

                ResolvePoint(scorer, "Out");
            }
        }

        public void RestartMatchNow()
        {
            StopAllCoroutines();
            ResetMatchState();
            StartNewRally();
        }

        public bool RequestHumanServe(PlayerActor server)
        {
            if (!AwaitingHumanServe || server == null || server != CurrentServer)
            {
                return false;
            }

            float meter = ServeMeter01;
            const float ideal = 0.82f;
            float error = Mathf.Abs(meter - ideal);
            ContactTimingGrade timing;

            if (error <= 0.075f)
            {
                timing = ContactTimingGrade.Perfect;
            }
            else if (error <= 0.22f)
            {
                timing = ContactTimingGrade.Good;
            }
            else
            {
                timing = meter < ideal ? ContactTimingGrade.Early : ContactTimingGrade.Late;
            }

            AwaitingHumanServe = false;
            ExecuteServe(server, timing);
            return true;
        }

        private void OnBallContact(BallContact contact)
        {
            if (!_rallyActive && contact.Type != BallContactType.Serve)
            {
                return;
            }

            PossessionUpdate update = _possession.Register(contact);
            ContactProcessed?.Invoke(contact);
            string actor = contact.Player != null ? contact.Player.DisplayName : "Server";
            _hud.SetMessage($"{contact.Team} {actor}: {contact.Type} [{contact.Timing}] | touches {_possession.CountedTouches}/3");

            if (contact.Type == BallContactType.Attack)
            {
                _momentum.Add(contact.Team, contact.Timing == ContactTimingGrade.Perfect ? 3f : 1.5f);
            }
            else if (contact.Type == BallContactType.Block)
            {
                _momentum.Add(contact.Team, contact.Timing == ContactTimingGrade.Perfect ? 6f : 4f);
            }
            else if (contact.Type == BallContactType.Dig)
            {
                _momentum.Add(contact.Team, contact.Timing == ContactTimingGrade.Perfect ? 2f : 1f);
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
            AwaitingHumanServe = false;
            _momentum.PointTo(scorer);
            PointScored?.Invoke(scorer);

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
            ResetMatchState();
            StartNewRally();
        }

        private void ResetMatchState()
        {
            _score.ResetMatch();
            _momentum.ResetMatch();
            _leftRotation?.ResetRotation();
            _rightRotation?.ResetRotation();
            _servingTeam = TeamSide.Left;
        }

        private void StartNewRally()
        {
            StopAllCoroutines();
            _resolvingPoint = false;
            _rallyActive = false;
            AwaitingHumanServe = false;
            _possession.Reset();

            _leftRotation?.ResetPlayersToHome();
            _rightRotation?.ResetPlayersToHome();

            TeamRotation servingRotation = _servingTeam == TeamSide.Left ? _leftRotation : _rightRotation;
            CurrentServer = servingRotation?.CurrentServer;
            float serveX = _servingTeam == TeamSide.Left ? -9.25f : 9.25f;
            float serveZ = CurrentServer != null ? CurrentServer.transform.position.z : 0f;
            Vector3 servePosition = new Vector3(serveX, 2.05f, serveZ);
            _ball.ResetBall(servePosition);

            if (CurrentServer != null && CurrentServer.IsHuman)
            {
                AwaitingHumanServe = true;
                _serveMeterStartedAt = Time.time;
                _hud.SetMessage($"{CurrentServer.DisplayName} to serve - press ACTION near the peak");
            }
            else
            {
                _hud.SetMessage($"{(CurrentServer != null ? CurrentServer.DisplayName : _servingTeam.ToString())} to serve");
                StartCoroutine(AutoServe());
            }
        }

        private IEnumerator AutoServe()
        {
            yield return new WaitForSeconds(_tuning.autoServeDelay);
            int serveStat = CurrentServer != null && CurrentServer.Profile != null ? CurrentServer.Profile.Stats.serve : 50;
            float perfectChance = Mathf.Lerp(0.06f, 0.24f, serveStat / 100f);
            ContactTimingGrade timing = UnityEngine.Random.value <= perfectChance
                ? ContactTimingGrade.Perfect
                : ContactTimingGrade.Good;
            ExecuteServe(CurrentServer, timing);
        }

        private void ExecuteServe(PlayerActor server, ContactTimingGrade timing)
        {
            float direction = _servingTeam == TeamSide.Left ? 1f : -1f;
            int serveStat = server != null && server.Profile != null ? server.Profile.Stats.serve : 50;
            float statScale = Mathf.Lerp(0.90f, 1.16f, serveStat / 100f);
            float timingScale = ContactTiming.PowerMultiplier(timing);
            bool floatServe = server != null && server.HasSkill("jump_float");
            bool dualServe = server != null && server.HasSkill("dual_serve");

            if (dualServe)
            {
                floatServe = UnityEngine.Random.value > 0.5f;
            }

            float forward = 8.8f * statScale * timingScale;
            float up = floatServe ? 5.2f : 5.8f;
            float lateral = UnityEngine.Random.Range(-0.65f, 0.65f);

            if (server != null && server.HasSkill("demon_serve"))
            {
                forward *= 1.10f;
            }

            Vector3 velocity = new Vector3(direction * forward, up, lateral);
            Vector3 spin;
            if (floatServe)
            {
                spin = Vector3.zero;
            }
            else
            {
                float spinDirection = server != null && server.HasSkill("southpaw") ? direction : -direction;
                spin = new Vector3(0f, 0f, spinDirection * 11f);
            }

            _rallyActive = true;
            _ball.WakeAndServe(_servingTeam, server, velocity, spin, timing);
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
