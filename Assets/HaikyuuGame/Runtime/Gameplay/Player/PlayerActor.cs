using System.Collections.Generic;
using HaikyuuGame.Core;
using HaikyuuGame.Gameplay.Ball;
using HaikyuuGame.Gameplay.Match;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Player
{
    public sealed class PlayerActor : MonoBehaviour
    {
        private static readonly List<PlayerActor> Registry = new List<PlayerActor>();

        private VolleyballTuning _tuning;
        private VolleyballBall _ball;
        private RallyController _rally;
        private Vector3 _homePosition;
        private float _verticalVelocity;
        private float _nextActionTime;
        private float _nextAiDecisionTime;
        private Vector3 _lastMoveInput;

        public TeamSide Team { get; private set; }
        public bool IsHuman { get; private set; }
        public VolleyballRole BaseRole { get; private set; }
        public int CourtSlot { get; private set; } = -1;
        public bool IsFrontRow => CourtSlot >= 0 && CourtSlot <= 2;
        public Vector3 HomePosition => _homePosition;

        private void OnEnable()
        {
            if (!Registry.Contains(this))
            {
                Registry.Add(this);
            }
        }

        private void OnDisable()
        {
            Registry.Remove(this);
        }

        public void Initialize(
            TeamSide team,
            bool isHuman,
            VolleyballRole role,
            Vector3 homePosition,
            VolleyballBall ball,
            VolleyballTuning tuning)
        {
            Team = team;
            IsHuman = isHuman;
            BaseRole = role;
            _homePosition = homePosition;
            _ball = ball;
            _tuning = tuning;
            ResetToHome();
        }

        public void BindMatch(RallyController rally)
        {
            _rally = rally;
        }

        public void SetCourtAssignment(int slot, Vector3 homePosition, bool active)
        {
            CourtSlot = slot;
            _homePosition = homePosition;

            if (gameObject.activeSelf != active)
            {
                gameObject.SetActive(active);
            }

            if (active)
            {
                ResetToHome();
            }
        }

        private void Update()
        {
            if (_ball == null || _tuning == null || _rally == null)
            {
                return;
            }

            if (IsHuman)
            {
                TickHuman();
            }
            else
            {
                TickAi();
            }
        }

        public void ResetToHome()
        {
            transform.position = _homePosition;
            _verticalVelocity = 0f;
        }

        private void TickHuman()
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            Vector3 move = new Vector3(x, 0f, z).normalized;
            _lastMoveInput = move;

            bool jump = Input.GetKeyDown(KeyCode.Space);
            TickMovement(move, jump);

            if (Input.GetKeyDown(KeyCode.Z))
            {
                TryPerform(BallContactType.Receive);
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                TryPerform(BallContactType.Set);
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                TryPerform(BallContactType.Attack);
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                TryPerform(BallContactType.Block);
            }
            else if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.J))
            {
                TryPerform(ChooseContextAction());
            }
        }

        private void TickAi()
        {
            Vector3 target = _homePosition;
            Vector3 landing = BallTrajectoryPredictor.EstimateLandingPoint(_ball);
            bool ballOnMyHalf = Team == TeamSide.Left ? landing.x < -0.1f : landing.x > 0.1f;
            bool opponentPossession = _rally.Possession.Team != Team;
            bool blockOpportunity = IsFrontRow
                && opponentPossession
                && Mathf.Abs(_ball.transform.position.x) < 1.7f
                && _ball.transform.position.y > 1.8f;

            bool isSetterTurn = _rally.Possession.Team == Team
                && _rally.Possession.CountedTouches == 1
                && BaseRole == VolleyballRole.Setter;

            bool isAttackTurn = _rally.Possession.Team == Team
                && _rally.Possession.CountedTouches >= 2
                && IsFrontRow
                && BaseRole != VolleyballRole.Setter
                && BaseRole != VolleyballRole.Libero;

            if (blockOpportunity)
            {
                target = new Vector3(
                    Team == TeamSide.Left ? -0.75f : 0.75f,
                    _homePosition.y,
                    Mathf.Clamp(_ball.transform.position.z, -3.7f, 3.7f));
            }
            else if (isSetterTurn || isAttackTurn || (ballOnMyHalf && ShouldChase(landing)))
            {
                target = new Vector3(
                    Mathf.Clamp(landing.x, Team == TeamSide.Left ? -8.2f : 0.65f, Team == TeamSide.Left ? -0.65f : 8.2f),
                    _homePosition.y,
                    Mathf.Clamp(landing.z, -4.0f, 4.0f));
            }
            else if (_rally.Possession.Team == Team && _rally.Possession.CountedTouches == 1 && IsFrontRow)
            {
                target = new Vector3(
                    Team == TeamSide.Left ? -1.75f : 1.75f,
                    _homePosition.y,
                    _homePosition.z);
            }

            Vector3 delta = target - transform.position;
            delta.y = 0f;
            Vector3 move = delta.sqrMagnitude > 0.05f ? delta.normalized : Vector3.zero;
            _lastMoveInput = move;

            bool nearBall = Vector3.Distance(transform.position + Vector3.up * 0.8f, _ball.transform.position) <= _tuning.actionReach;
            bool shouldJump = nearBall
                && _ball.transform.position.y > 1.7f
                && (blockOpportunity || isAttackTurn)
                && Time.time >= _nextAiDecisionTime;

            TickMovement(move, shouldJump);

            if (nearBall && Time.time >= _nextAiDecisionTime)
            {
                _nextAiDecisionTime = Time.time + Random.Range(_tuning.aiDecisionMin, _tuning.aiDecisionMax);
                TryPerform(ChooseContextAction());
            }
        }

        private bool ShouldChase(Vector3 landing)
        {
            float myDistance = HorizontalDistanceSquared(transform.position, landing);

            for (int i = 0; i < Registry.Count; i++)
            {
                PlayerActor other = Registry[i];
                if (other == this || other.Team != Team)
                {
                    continue;
                }

                if (_rally.Possession.Team == Team
                    && _rally.Possession.CountedTouches == 1
                    && BaseRole == VolleyballRole.Setter)
                {
                    return true;
                }

                if (HorizontalDistanceSquared(other.transform.position, landing) + 0.12f < myDistance)
                {
                    return false;
                }
            }

            return true;
        }

        private static float HorizontalDistanceSquared(Vector3 a, Vector3 b)
        {
            float dx = a.x - b.x;
            float dz = a.z - b.z;
            return (dx * dx) + (dz * dz);
        }

        private void TickMovement(Vector3 move, bool jumpRequested)
        {
            const float groundY = 1f;
            bool grounded = transform.position.y <= groundY + 0.01f;

            if (grounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = 0f;
                Vector3 groundedPosition = transform.position;
                groundedPosition.y = groundY;
                transform.position = groundedPosition;
            }

            if (grounded && jumpRequested)
            {
                _verticalVelocity = _tuning.jumpVelocity;
            }

            _verticalVelocity += Physics.gravity.y * Time.deltaTime;

            Vector3 next = transform.position;
            next += move * (_tuning.moveSpeed * Time.deltaTime);
            next.y += _verticalVelocity * Time.deltaTime;

            float minX = Team == TeamSide.Left ? -8.35f : 0.6f;
            float maxX = Team == TeamSide.Left ? -0.6f : 8.35f;
            next.x = Mathf.Clamp(next.x, minX, maxX);
            next.z = Mathf.Clamp(next.z, -4.05f, 4.05f);
            next.y = Mathf.Max(groundY, next.y);

            transform.position = next;
        }

        private BallContactType ChooseContextAction()
        {
            bool airborne = transform.position.y > 1.08f;
            bool nearNet = Mathf.Abs(transform.position.x) < 2.0f;
            bool opponentLastTouch = _ball.LastTouchTeam != Team && _ball.LastTouchTeam != TeamSide.None;

            if (opponentLastTouch && nearNet && IsFrontRow && airborne && _ball.transform.position.y > 1.8f)
            {
                return BallContactType.Block;
            }

            if (_rally.Possession.Team == Team)
            {
                if (_rally.Possession.CountedTouches == 1)
                {
                    return BaseRole == VolleyballRole.Setter ? BallContactType.Set : BallContactType.Set;
                }

                if (_rally.Possession.CountedTouches >= 2)
                {
                    return BallContactType.Attack;
                }
            }

            return _ball.transform.position.y < 1.25f ? BallContactType.Dig : BallContactType.Receive;
        }

        private void TryPerform(BallContactType action)
        {
            if (Time.time < _nextActionTime || !_rally.RallyActive)
            {
                return;
            }

            Vector3 contactOrigin = transform.position + Vector3.up * 0.75f;
            Vector3 toBall = _ball.transform.position - contactOrigin;
            if (toBall.magnitude > _tuning.actionReach)
            {
                return;
            }

            _nextActionTime = Time.time + _tuning.actionCooldown;

            switch (action)
            {
                case BallContactType.Set:
                    PerformSet();
                    break;
                case BallContactType.Attack:
                    PerformAttack();
                    break;
                case BallContactType.Block:
                    PerformBlock();
                    break;
                case BallContactType.Dig:
                    PerformReceive(true);
                    break;
                default:
                    PerformReceive(false);
                    break;
            }
        }

        private void PerformReceive(bool dig)
        {
            PlayerActor setter = FindBestTeammate(VolleyballRole.Setter, false);
            Vector3 target = setter != null
                ? setter.transform.position + Vector3.up * _tuning.receiveTargetHeight
                : new Vector3(Team == TeamSide.Left ? -2.3f : 2.3f, 2.3f, 0f);

            float flightTime = dig ? _tuning.digFlightTime : _tuning.receiveFlightTime;
            Vector3 velocity = BallTrajectoryPredictor.SolveBallisticVelocity(_ball.transform.position, target, flightTime);
            _ball.Contact(
                Team,
                this,
                dig ? BallContactType.Dig : BallContactType.Receive,
                velocity,
                Vector3.zero);
        }

        private void PerformSet()
        {
            PlayerActor attacker = FindAttackTarget();
            float attackDirection = Team == TeamSide.Left ? 1f : -1f;
            Vector3 target = attacker != null
                ? attacker.transform.position + Vector3.up * _tuning.setTargetHeight + new Vector3(attackDirection * 0.35f, 0f, 0f)
                : new Vector3(Team == TeamSide.Left ? -1.6f : 1.6f, _tuning.setTargetHeight, 0f);

            bool quick = attacker != null && attacker.BaseRole == VolleyballRole.MiddleBlocker;
            float flightTime = quick ? _tuning.quickSetFlightTime : _tuning.setFlightTime;
            Vector3 velocity = BallTrajectoryPredictor.SolveBallisticVelocity(_ball.transform.position, target, flightTime);
            _ball.Contact(Team, this, BallContactType.Set, velocity, Vector3.zero);
        }

        private void PerformAttack()
        {
            float attackDirection = Team == TeamSide.Left ? 1f : -1f;
            float aimZ = IsHuman
                ? Mathf.Clamp(_lastMoveInput.z * 3.2f, -3.2f, 3.2f)
                : Mathf.Clamp(-transform.position.z * 0.45f, -2.8f, 2.8f);

            float downSpeed = _ball.transform.position.y > _tuning.netHeight
                ? -_tuning.spikeDownSpeed
                : -1.2f;

            Vector3 velocity = new Vector3(
                attackDirection * _tuning.spikeForwardSpeed,
                downSpeed,
                aimZ);

            _ball.Contact(
                Team,
                this,
                BallContactType.Attack,
                velocity,
                new Vector3(0f, 0f, -attackDirection * 20f));
        }

        private void PerformBlock()
        {
            float attackDirection = Team == TeamSide.Left ? 1f : -1f;
            Vector3 incoming = _ball.Body.linearVelocity;
            Vector3 velocity = new Vector3(
                attackDirection * Mathf.Max(_tuning.blockForwardSpeed, Mathf.Abs(incoming.x) * 0.72f),
                _tuning.blockUpSpeed,
                -incoming.z * 0.35f);

            _ball.Contact(Team, this, BallContactType.Block, velocity, Vector3.zero);
        }

        private PlayerActor FindAttackTarget()
        {
            PlayerActor best = null;
            float bestScore = float.MinValue;

            for (int i = 0; i < Registry.Count; i++)
            {
                PlayerActor candidate = Registry[i];
                if (candidate == this
                    || candidate.Team != Team
                    || !candidate.IsFrontRow
                    || candidate.BaseRole == VolleyballRole.Setter
                    || candidate.BaseRole == VolleyballRole.Libero)
                {
                    continue;
                }

                float score = candidate.BaseRole == VolleyballRole.MiddleBlocker ? 2.0f : 1.0f;
                score -= Mathf.Abs(candidate.transform.position.z - transform.position.z) * 0.04f;

                if (score > bestScore)
                {
                    bestScore = score;
                    best = candidate;
                }
            }

            return best;
        }

        private PlayerActor FindBestTeammate(VolleyballRole role, bool frontOnly)
        {
            PlayerActor best = null;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < Registry.Count; i++)
            {
                PlayerActor candidate = Registry[i];
                if (candidate == this || candidate.Team != Team || candidate.BaseRole != role)
                {
                    continue;
                }

                if (frontOnly && !candidate.IsFrontRow)
                {
                    continue;
                }

                float distance = HorizontalDistanceSquared(transform.position, candidate.transform.position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = candidate;
                }
            }

            return best;
        }
    }
}
