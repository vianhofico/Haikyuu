using System.Collections.Generic;
using HaikyuuGame.Core;
using HaikyuuGame.Gameplay.Ball;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Player
{
    public sealed class PlayerActor : MonoBehaviour
    {
        private static readonly List<PlayerActor> Registry = new List<PlayerActor>();

        private VolleyballTuning _tuning;
        private VolleyballBall _ball;
        private Vector3 _homePosition;
        private float _verticalVelocity;
        private float _nextActionTime;
        private float _nextAiDecisionTime;

        public TeamSide Team { get; private set; }
        public bool IsHuman { get; private set; }
        public Vector3 HomePosition => _homePosition;

        private void OnEnable()
        {
            Registry.Add(this);
        }

        private void OnDisable()
        {
            Registry.Remove(this);
        }

        public void Initialize(
            TeamSide team,
            bool isHuman,
            Vector3 homePosition,
            VolleyballBall ball,
            VolleyballTuning tuning)
        {
            Team = team;
            IsHuman = isHuman;
            _homePosition = homePosition;
            _ball = ball;
            _tuning = tuning;
            ResetToHome();
        }

        private void Update()
        {
            if (_ball == null || _tuning == null)
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

            bool jump = Input.GetKeyDown(KeyCode.Space);
            bool action = Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.J);

            TickMovement(move, jump);

            if (action)
            {
                PerformContextAction();
            }
        }

        private void TickAi()
        {
            Vector3 target = _homePosition;
            Vector3 landing = BallTrajectoryPredictor.EstimateLandingPoint(_ball);
            bool ballHeadingToTeam = Team == TeamSide.Left ? landing.x < -0.15f : landing.x > 0.15f;

            if (ballHeadingToTeam && ShouldChase(landing))
            {
                target = new Vector3(landing.x, _homePosition.y, Mathf.Clamp(landing.z, -4.0f, 4.0f));
            }

            Vector3 delta = target - transform.position;
            delta.y = 0f;
            Vector3 move = delta.sqrMagnitude > 0.05f ? delta.normalized : Vector3.zero;

            bool nearBall = Vector3.Distance(transform.position + Vector3.up * 0.8f, _ball.transform.position) <= _tuning.actionReach;
            bool shouldJump = nearBall && _ball.transform.position.y > 1.75f && Time.time >= _nextAiDecisionTime;

            TickMovement(move, shouldJump);

            if (nearBall && Time.time >= _nextAiDecisionTime)
            {
                _nextAiDecisionTime = Time.time + Random.Range(0.22f, 0.42f);
                PerformContextAction();
            }
        }

        private bool ShouldChase(Vector3 landing)
        {
            float myDistance = HorizontalDistanceSquared(transform.position, landing);

            for (int i = 0; i < Registry.Count; i++)
            {
                PlayerActor other = Registry[i];
                if (other == this || other.Team != Team || other.IsHuman)
                {
                    continue;
                }

                if (HorizontalDistanceSquared(other.transform.position, landing) + 0.15f < myDistance)
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

        private void PerformContextAction()
        {
            if (Time.time < _nextActionTime)
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

            float attackDirection = Team == TeamSide.Left ? 1f : -1f;
            bool airborne = transform.position.y > 1.08f;
            bool attackableHeight = _ball.transform.position.y > transform.position.y + 0.45f;

            Vector3 velocity;
            Vector3 spin;

            if (airborne && attackableHeight)
            {
                float lateral = Mathf.Clamp(toBall.z * 1.15f, -3.2f, 3.2f);
                velocity = new Vector3(
                    attackDirection * _tuning.spikeForwardSpeed,
                    -_tuning.spikeDownSpeed,
                    lateral);
                spin = new Vector3(0f, 0f, -attackDirection * 18f);
            }
            else
            {
                float correction = Mathf.Clamp(-toBall.z * 0.75f, -2f, 2f);
                velocity = new Vector3(
                    attackDirection * _tuning.receiveForwardSpeed,
                    _tuning.receiveUpSpeed,
                    correction);
                spin = Vector3.zero;
            }

            _ball.Body.WakeUp();
            _ball.Contact(Team, velocity, spin);
        }
    }
}
