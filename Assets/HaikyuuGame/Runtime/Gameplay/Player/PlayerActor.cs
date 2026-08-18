using System.Collections.Generic;
using HaikyuuGame.Core;
using HaikyuuGame.Gameplay.Ball;
using HaikyuuGame.Gameplay.Character;
using HaikyuuGame.Gameplay.Input;
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
        public RuntimeCharacterProfile Profile { get; private set; }
        public int CourtSlot { get; private set; } = -1;
        public bool IsFrontRow => CourtSlot >= 0 && CourtSlot <= 2;
        public Vector3 HomePosition => _homePosition;
        public string DisplayName => Profile != null ? Profile.DisplayName : BaseRole.ToString();

        private CharacterStats Stats => Profile != null ? Profile.Stats : CharacterStats.Default;
        private CharacterArchetype Archetype => Profile != null ? Profile.Archetype : CharacterArchetype.AllRounder;

        private void OnEnable()
        {
            if (!Registry.Contains(this)) Registry.Add(this);
        }

        private void OnDisable()
        {
            Registry.Remove(this);
        }

        public void Initialize(TeamSide team, bool isHuman, RuntimeCharacterProfile profile, Vector3 homePosition, VolleyballBall ball, VolleyballTuning tuning)
        {
            Team = team;
            IsHuman = isHuman;
            _homePosition = homePosition;
            _ball = ball;
            _tuning = tuning;
            AssignProfile(profile);
            ResetToHome();
        }

        public void AssignProfile(RuntimeCharacterProfile profile)
        {
            Profile = profile;
            BaseRole = profile != null ? profile.Role : VolleyballRole.OutsideHitter;
            if (profile != null)
            {
                string prefix = IsHuman ? "Player_Human" : $"Player_{Team}";
                gameObject.name = $"{prefix}_{profile.DisplayName.Replace(" ", "_")}";
            }
        }

        public void BindMatch(RallyController rally)
        {
            _rally = rally;
        }

        public bool HasSkill(string skillId)
        {
            if (Profile == null || Profile.SkillIds == null) return false;
            for (int i = 0; i < Profile.SkillIds.Length; i++)
            {
                if (Profile.SkillIds[i] == skillId) return true;
            }
            return false;
        }

        public void SetCourtAssignment(int slot, Vector3 homePosition, bool active)
        {
            CourtSlot = slot;
            _homePosition = homePosition;
            if (gameObject.activeSelf != active) gameObject.SetActive(active);
            if (active) ResetToHome();
        }

        public void ResetToHome()
        {
            transform.position = _homePosition;
            _verticalVelocity = 0f;
        }

        private void Update()
        {
            if (_ball == null || _tuning == null || _rally == null) return;
            if (IsHuman) TickHuman(); else TickAi();
        }

        private void TickHuman()
        {
            float x = UnityEngine.Input.GetAxisRaw("Horizontal");
            float z = UnityEngine.Input.GetAxisRaw("Vertical");
            TouchInputRouter touch = TouchInputRouter.Instance;
            if (touch != null && touch.Move.sqrMagnitude > 0.01f)
            {
                x = touch.Move.x;
                z = touch.Move.y;
            }

            Vector3 move = new Vector3(x, 0f, z).normalized;
            _lastMoveInput = move;
            bool jump = UnityEngine.Input.GetKeyDown(KeyCode.Space) || (touch != null && touch.ConsumeJump());
            bool context = UnityEngine.Input.GetKeyDown(KeyCode.F) || UnityEngine.Input.GetKeyDown(KeyCode.J) || (touch != null && touch.ConsumeContextAction());
            TickMovement(move, jump);

            if (_rally.AwaitingHumanServe && _rally.CurrentServer == this)
            {
                if (context || UnityEngine.Input.GetKeyDown(KeyCode.C)) _rally.RequestHumanServe(this);
                return;
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Z)) TryPerform(BallContactType.Receive);
            else if (UnityEngine.Input.GetKeyDown(KeyCode.X)) TryPerform(BallContactType.Set);
            else if (UnityEngine.Input.GetKeyDown(KeyCode.C)) TryPerform(BallContactType.Attack);
            else if (UnityEngine.Input.GetKeyDown(KeyCode.V)) TryPerform(BallContactType.Block);
            else if (context) TryPerform(ChooseContextAction());
        }

        private void TickAi()
        {
            Vector3 target = _homePosition;
            Vector3 landing = BallTrajectoryPredictor.EstimateLandingPoint(_ball);
            bool ballOnMyHalf = Team == TeamSide.Left ? landing.x < -0.1f : landing.x > 0.1f;
            bool opponentPossession = _rally.Possession.Team != Team;
            float blockReadRange = Archetype == CharacterArchetype.ReadBlocker || Archetype == CharacterArchetype.GuessBlocker ? 2.1f : 1.7f;
            bool blockOpportunity = IsFrontRow && opponentPossession && Mathf.Abs(_ball.transform.position.x) < blockReadRange && _ball.transform.position.y > 1.75f;
            bool isSetterTurn = _rally.Possession.Team == Team && _rally.Possession.CountedTouches == 1 && BaseRole == VolleyballRole.Setter;
            bool isAttackTurn = _rally.Possession.Team == Team && _rally.Possession.CountedTouches >= 2 && IsFrontRow && BaseRole != VolleyballRole.Setter && BaseRole != VolleyballRole.Libero;

            if (blockOpportunity)
            {
                target = new Vector3(Team == TeamSide.Left ? -0.75f : 0.75f, _homePosition.y, Mathf.Clamp(_ball.transform.position.z, -3.7f, 3.7f));
            }
            else if (isSetterTurn || isAttackTurn || (ballOnMyHalf && ShouldChase(landing)))
            {
                target = new Vector3(
                    Mathf.Clamp(landing.x, Team == TeamSide.Left ? -8.2f : 0.65f, Team == TeamSide.Left ? -0.65f : 8.2f),
                    _homePosition.y,
                    Mathf.Clamp(landing.z, -4f, 4f));
            }
            else if (_rally.Possession.Team == Team && _rally.Possession.CountedTouches == 1 && IsFrontRow)
            {
                target = new Vector3(Team == TeamSide.Left ? -1.75f : 1.75f, _homePosition.y, _homePosition.z);
            }

            Vector3 delta = target - transform.position;
            delta.y = 0f;
            Vector3 move = delta.sqrMagnitude > 0.05f ? delta.normalized : Vector3.zero;
            _lastMoveInput = move;
            BallContactType contextAction = ChooseContextAction();
            bool nearBall = Vector3.Distance(transform.position + Vector3.up * 0.8f, _ball.transform.position) <= GetActionReach(contextAction);
            bool shouldJump = nearBall && _ball.transform.position.y > 1.65f && (blockOpportunity || isAttackTurn) && Time.time >= _nextAiDecisionTime;
            TickMovement(move, shouldJump);

            if (nearBall && Time.time >= _nextAiDecisionTime)
            {
                float technique = Mathf.Clamp01(Stats.technique / 100f);
                float reactionScale = Mathf.Lerp(1.14f, 0.72f, technique);
                _nextAiDecisionTime = Time.time + UnityEngine.Random.Range(_tuning.aiDecisionMin, _tuning.aiDecisionMax) * reactionScale;
                TryPerform(contextAction);
            }
        }

        private bool ShouldChase(Vector3 landing)
        {
            float myDistance = HorizontalDistanceSquared(transform.position, landing);
            for (int i = 0; i < Registry.Count; i++)
            {
                PlayerActor other = Registry[i];
                if (other == this || other.Team != Team) continue;
                if (_rally.Possession.Team == Team && _rally.Possession.CountedTouches == 1 && BaseRole == VolleyballRole.Setter) return true;
                if (HorizontalDistanceSquared(other.transform.position, landing) + 0.12f < myDistance) return false;
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
                _verticalVelocity = _tuning.jumpVelocity * StatScale(Stats.jump) * ArchetypeModifiers.Jump(Archetype);
            }

            _verticalVelocity += Physics.gravity.y * Time.deltaTime;
            Vector3 next = transform.position;
            float moveSpeed = _tuning.moveSpeed * StatScale(Stats.speed) * ArchetypeModifiers.Move(Archetype);
            next += move * (moveSpeed * Time.deltaTime);
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
            bool nearNet = Mathf.Abs(transform.position.x) < 2f;
            bool opponentLastTouch = _ball.LastTouchTeam != Team && _ball.LastTouchTeam != TeamSide.None;
            if (opponentLastTouch && nearNet && IsFrontRow && airborne && _ball.transform.position.y > 1.75f) return BallContactType.Block;
            if (_rally.Possession.Team == Team)
            {
                if (_rally.Possession.CountedTouches == 1) return BallContactType.Set;
                if (_rally.Possession.CountedTouches >= 2) return BallContactType.Attack;
            }
            return _ball.transform.position.y < 1.25f ? BallContactType.Dig : BallContactType.Receive;
        }

        private void TryPerform(BallContactType action)
        {
            if (Time.time < _nextActionTime || !_rally.RallyActive) return;
            Vector3 contactOrigin = transform.position + Vector3.up * 0.75f;
            Vector3 toBall = _ball.transform.position - contactOrigin;
            float reach = GetActionReach(action);
            if (toBall.magnitude > reach) return;

            ContactTimingGrade timing = ContactTiming.Evaluate(toBall.magnitude, reach, _ball.Body.linearVelocity);
            _nextActionTime = Time.time + _tuning.actionCooldown;
            switch (action)
            {
                case BallContactType.Set: PerformSet(timing); break;
                case BallContactType.Attack: PerformAttack(timing); break;
                case BallContactType.Block: PerformBlock(timing); break;
                case BallContactType.Dig: PerformReceive(true, timing); break;
                default: PerformReceive(false, timing); break;
            }
        }

        private void PerformReceive(bool dig, ContactTimingGrade timing)
        {
            PlayerActor setter = FindBestTeammate(VolleyballRole.Setter, false);
            Vector3 target = setter != null ? setter.transform.position + Vector3.up * _tuning.receiveTargetHeight : new Vector3(Team == TeamSide.Left ? -2.3f : 2.3f, 2.3f, 0f);
            float quality = Mathf.Lerp(0.92f, 1.08f, Stats.receive / 100f) * ContactTiming.ControlMultiplier(timing);
            float flightTime = (dig ? _tuning.digFlightTime : _tuning.receiveFlightTime) / Mathf.Max(0.72f, quality);
            Vector3 velocity = BallTrajectoryPredictor.SolveBallisticVelocity(_ball.transform.position, target, flightTime);
            if (timing == ContactTimingGrade.Early || timing == ContactTimingGrade.Late) velocity.z += UnityEngine.Random.Range(-1.1f, 1.1f);
            _ball.Contact(Team, this, dig ? BallContactType.Dig : BallContactType.Receive, velocity, Vector3.zero, timing);
        }

        private void PerformSet(ContactTimingGrade timing)
        {
            PlayerActor attacker = FindAttackTarget();
            float attackDirection = Team == TeamSide.Left ? 1f : -1f;
            Vector3 target = attacker != null ? attacker.transform.position + Vector3.up * _tuning.setTargetHeight + new Vector3(attackDirection * 0.35f, 0f, 0f) : new Vector3(Team == TeamSide.Left ? -1.6f : 1.6f, _tuning.setTargetHeight, 0f);
            float synergy = attacker != null ? CharacterSynergyCatalog.SetTargetPriority(Profile, attacker.Profile) : 0f;
            bool quick = attacker != null && (attacker.BaseRole == VolleyballRole.MiddleBlocker || attacker.ArchetypeIs(CharacterArchetype.SpeedDecoy) || synergy >= 2.5f);
            float control = StatScale(Stats.set) * ArchetypeModifiers.SetControl(Archetype) * ContactTiming.ControlMultiplier(timing);
            float flightTime = (quick ? _tuning.quickSetFlightTime : _tuning.setFlightTime) / Mathf.Clamp(control, 0.74f, 1.22f);
            Vector3 velocity = BallTrajectoryPredictor.SolveBallisticVelocity(_ball.transform.position, target, flightTime);
            if (timing == ContactTimingGrade.Early || timing == ContactTimingGrade.Late) velocity.z += UnityEngine.Random.Range(-0.8f, 0.8f);
            _ball.Contact(Team, this, BallContactType.Set, velocity, Vector3.zero, timing);
        }

        private void PerformAttack(ContactTimingGrade timing)
        {
            PlayerActor previousSetter = _ball.LastContact.Player;
            RuntimeCharacterProfile setterProfile = previousSetter != null ? previousSetter.Profile : null;
            float attackDirection = Team == TeamSide.Left ? 1f : -1f;
            float technique = Stats.technique / 100f;
            float aimRange = Mathf.Lerp(2.2f, 3.6f, technique);
            float aimZ = IsHuman ? Mathf.Clamp(_lastMoveInput.z * aimRange, -aimRange, aimRange) : Mathf.Clamp(-transform.position.z * 0.45f, -aimRange, aimRange);
            float downSpeed = _ball.transform.position.y > _tuning.netHeight ? -_tuning.spikeDownSpeed : -1.2f;
            float attackMultiplier = StatScale(Stats.attack) * ArchetypeModifiers.Attack(Archetype) * CharacterSynergyCatalog.AttackMultiplier(setterProfile, Profile) * ContactTiming.PowerMultiplier(timing);
            if (Archetype == CharacterArchetype.MomentumAce)
            {
                float normalizedFlow = (_rally.Momentum.Get(Team) + 100f) / 200f;
                attackMultiplier *= Mathf.Lerp(0.93f, 1.14f, normalizedFlow);
            }
            if (HasSkill("bad_set_killer") && _ball.LastContact.Type != BallContactType.Set) attackMultiplier *= 1.10f;

            Vector3 velocity = new Vector3(attackDirection * _tuning.spikeForwardSpeed * attackMultiplier, downSpeed * Mathf.Lerp(0.9f, 1.08f, technique), aimZ);
            float spinDirection = HasSkill("southpaw") ? attackDirection : -attackDirection;
            Vector3 spin = new Vector3(0f, HasSkill("flexible_wrist") ? 15f : 0f, spinDirection * 20f);
            _ball.Contact(Team, this, BallContactType.Attack, velocity, spin, timing);
        }

        private void PerformBlock(ContactTimingGrade timing)
        {
            float attackDirection = Team == TeamSide.Left ? 1f : -1f;
            Vector3 incoming = _ball.Body.linearVelocity;
            float blockMultiplier = StatScale(Stats.block) * ArchetypeModifiers.Block(Archetype) * ContactTiming.PowerMultiplier(timing);
            if (Archetype == CharacterArchetype.GuessBlocker)
            {
                float readChance = Mathf.Lerp(0.58f, 0.86f, Stats.technique / 100f);
                blockMultiplier *= UnityEngine.Random.value <= readChance ? 1.22f : 0.78f;
            }
            Vector3 velocity = new Vector3(attackDirection * Mathf.Max(_tuning.blockForwardSpeed, Mathf.Abs(incoming.x) * 0.72f) * blockMultiplier, _tuning.blockUpSpeed, -incoming.z * 0.35f);
            _ball.Contact(Team, this, BallContactType.Block, velocity, Vector3.zero, timing);
        }

        private PlayerActor FindAttackTarget()
        {
            PlayerActor best = null;
            float bestScore = float.MinValue;
            for (int i = 0; i < Registry.Count; i++)
            {
                PlayerActor candidate = Registry[i];
                if (candidate == this || candidate.Team != Team || !candidate.IsFrontRow || candidate.BaseRole == VolleyballRole.Setter || candidate.BaseRole == VolleyballRole.Libero) continue;
                float attackStat = candidate.Profile != null ? candidate.Profile.Stats.attack : 50f;
                float score = attackStat * 0.02f;
                if (candidate.BaseRole == VolleyballRole.MiddleBlocker) score += 0.7f;
                if (candidate.ArchetypeIs(CharacterArchetype.SpeedDecoy)) score += 0.5f;
                score += CharacterSynergyCatalog.SetTargetPriority(Profile, candidate.Profile);
                score -= Mathf.Abs(candidate.transform.position.z - transform.position.z) * 0.04f;
                if (score > bestScore) { bestScore = score; best = candidate; }
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
                if (candidate == this || candidate.Team != Team || candidate.BaseRole != role) continue;
                if (frontOnly && !candidate.IsFrontRow) continue;
                float distance = HorizontalDistanceSquared(transform.position, candidate.transform.position);
                if (distance < bestDistance) { bestDistance = distance; best = candidate; }
            }
            return best;
        }

        private float GetActionReach(BallContactType action)
        {
            float reach = _tuning.actionReach * Mathf.Lerp(0.94f, 1.08f, Stats.technique / 100f);
            if (action == BallContactType.Receive || action == BallContactType.Dig) reach *= ArchetypeModifiers.ReceiveReach(Archetype);
            else if (action == BallContactType.Block) reach *= ArchetypeModifiers.Block(Archetype);
            return reach;
        }

        private bool ArchetypeIs(CharacterArchetype archetype)
        {
            return Archetype == archetype;
        }

        private static float StatScale(int stat)
        {
            return Mathf.Lerp(0.86f, 1.14f, Mathf.Clamp01((stat - 1f) / 99f));
        }
    }
}
