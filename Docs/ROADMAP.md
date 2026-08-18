# Production Roadmap

## Phase 0 — Foundation

- [x] Unity project skeleton
- [x] Git-friendly layout
- [x] Runtime bootstrap
- [x] Data-driven character/skill definitions
- [x] Architecture documentation
- [ ] Assembly definitions after boundaries stabilize
- [ ] Automated edit-mode tests

Gate: project opens cleanly and a playable scene can be generated.

## Phase 1 — Volleyball Sandbox

- [x] Court and net placeholders
- [x] Player movement
- [x] Jump
- [x] Ball Rigidbody wrapper
- [x] Basic contextual contact
- [x] Local ball ownership / last touch
- [x] Trajectory prediction
- [ ] Tuned serve physics
- [ ] Tuned spike physics
- [ ] Float/topspin/spin model

Gate: simply hitting the ball across the net is satisfying.

## Phase 2 — Rally Core

- [x] Basic 6v6 placeholder formation
- [x] Simple local AI
- [x] Point detection
- [x] Rally reset
- [x] Prototype scoring
- [ ] Explicit receive state
- [ ] Setter targeting
- [ ] Approach timing
- [ ] Spike direction aiming
- [ ] Block timing
- [ ] Dig/pancake states

Gate: repeated rallies do not soft-lock and controls are understandable.

## Phase 3 — Full Volleyball Rules

- [ ] 3-contact enforcement
- [ ] Rotation
- [ ] Front/back row restrictions
- [ ] Libero replacement
- [ ] Service order
- [ ] Net faults
- [ ] Back-row attack faults
- [ ] Best-of-3 25/25/15 scoring
- [ ] Substitution framework

## Phase 4 — AI

- [ ] Team tactical brain
- [ ] Receiver ownership
- [ ] Setter decision tree
- [ ] Attack selection
- [ ] Read/commit blocking
- [ ] Funnel defense
- [ ] Difficulty profiles based on decision quality, not stat cheating

## Phase 5 — Game Feel

- [ ] Basic/advanced mobile controls
- [ ] Controller support
- [ ] Hit stop
- [ ] Camera modes
- [ ] Screen shake
- [ ] Ball trails
- [ ] Impact rings
- [ ] Haptics
- [ ] Layered volleyball audio

Vertical-slice gate: external playtesters should want to replay rallies even with placeholder characters.

## Phase 6 — Character Framework

- [ ] Passive/active/reaction/combo/ultimate skill execution
- [ ] Momentum system
- [ ] Pair chemistry
- [ ] Team identity
- [ ] First 12 distinct characters

## Phase 7+ — Content and Product

- [ ] Final stylized 3D/cel-shaded art direction
- [ ] 48-character launch target
- [ ] 8+ teams
- [ ] 8+ courts
- [ ] Story campaign
- [ ] Career
- [ ] Dream Team
- [ ] Tournament
- [ ] Training
- [ ] Challenge
- [ ] Replay
- [ ] VI/EN localization
- [ ] Android/Windows release pipeline
- [ ] Performance/QA/balance pass
