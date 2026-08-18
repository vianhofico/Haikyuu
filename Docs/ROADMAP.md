# Production Roadmap / Completion Matrix

This document tracks **code completion separately from runtime validation and final-asset production**.

## Phase 0 — Foundation — CODE COMPLETE

- [x] Unity project skeleton and Git-friendly layout
- [x] Runtime-generated playable scene/bootstrap
- [x] Data-driven character, team and skill/profile definitions
- [x] Modular gameplay folders and architecture documentation
- [x] Save/version migration framework
- [x] Static repository validator
- [x] Unity editor project-data validator
- [ ] Unity import/compile verification on a machine with Unity Editor

Gate remaining: open/import cleanly and run project validator in Unity.

## Phase 1 — Volleyball Physics — CODE COMPLETE / TUNING REQUIRED

- [x] Court, net and boundaries
- [x] Player movement and jump
- [x] Rigidbody ball wrapper
- [x] Trajectory prediction
- [x] Ballistic receive/set targeting
- [x] Serve / receive / set / spike / block / dig contacts
- [x] Float-serve/topspin/southpaw/flexible-wrist spin hooks
- [x] Directional spike control
- [x] Early / Good / Perfect / Late contact timing
- [ ] Device/playtest tuning for serve, receive and spike feel
- [ ] Final physics/balance constants after repeated Play Mode sessions

## Phase 2 — Rally Core — CODE COMPLETE

- [x] 6v6 formation
- [x] Arcade 3v3 formation using same core
- [x] Rally lifecycle/reset
- [x] contextual and explicit controls
- [x] setter targeting and attacker selection
- [x] spike direction aiming
- [x] block timing
- [x] receive/dig actions
- [x] manual human serving
- [x] match and set scoring
- [ ] soak-test repeated rallies in Unity for soft locks

## Phase 3 — Volleyball Rules — CODE COMPLETE / RUNTIME VERIFICATION REQUIRED

- [x] three-contact enforcement
- [x] block excluded from contact count
- [x] double-contact fault
- [x] four-contact fault
- [x] side-out rotation
- [x] service order
- [x] front/back-row representation
- [x] back-row attack fault
- [x] libero replacement
- [x] libero attack-height restriction
- [x] libero excluded from prototype service slot
- [x] best-of-three 25/25/15 scoring
- [x] 3v3 ↔ 6v6 format switching
- [ ] net-touch fault (requires reliable player/net collider production setup)
- [ ] general substitution UI beyond automatic libero replacement
- [ ] Play Mode regression through multiple rotations and format switches

## Phase 4 — AI — FUNCTIONAL ALPHA COMPLETE

- [x] trajectory-aware receiver ownership
- [x] setter decision/targeting
- [x] attack selection
- [x] front-row blocking decisions
- [x] read/guess blocker archetype modifiers
- [x] role-aware positioning
- [x] stats/technique affect reaction/decision timing
- [x] character/team synergy priorities
- [x] six named runtime difficulty profiles exposed through F10 settings
- [x] difficulty changes local AI decision timing immediately
- [ ] funnel-defense specialization pass
- [ ] balance with real playtest telemetry

## Phase 5 — Game Feel / Presentation — FUNCTIONAL ALPHA COMPLETE

- [x] keyboard controls
- [x] basic legacy-axis gamepad movement + Jump/Action bridge
- [x] mobile touch move/JUMP/ACTION
- [x] hit-stop with pause ownership protection
- [x] reduced-cinematics toggle disables hit-stop
- [x] screen-shake toggle applies at runtime
- [x] master/SFX volume runtime settings
- [x] camera follow and impact punch
- [x] ball trail
- [x] PERFECT feedback
- [x] impact streak VFX
- [x] procedural hit audio
- [x] procedural stylized player presentation
- [x] team palettes
- [x] procedural arena/crowd/lights
- [x] last-rally trajectory replay trace
- [x] match statistics overlay
- [ ] final animation library
- [ ] authored facial/celebration animations
- [ ] authored music/voice/SFX mix
- [ ] haptics verified on physical Android hardware

## Phase 6 — Character Framework — CODE COMPLETE FOR LAUNCH ROSTER

- [x] 48 runtime character profiles
- [x] 10-stat model
- [x] archetype modifiers
- [x] signature skill IDs/hooks
- [x] momentum/Flow
- [x] pair chemistry/synergy
- [x] school/team presets
- [x] role-correct support profiles for incomplete canonical sixes
- [x] custom runtime profile support
- [ ] final per-character authored models/rigs/animations
- [ ] complete balance pass after Unity playtesting

## Phase 7 — Game Modes — CODE COMPLETE FOR OFFLINE ALPHA

- [x] Quick Match 6v6
- [x] Arcade 3v3
- [x] Story — 10 chapters, intro/objectives, progression and completion
- [x] Career — custom player, role choice, training, seasons/weeks
- [x] Tournament — opponent progression and championship completion
- [x] Training — live action counters
- [x] Challenge — objective/progress/reward
- [x] Dream Team — persistent role-filtered seven-slot builder
- [x] replay-lite trajectory trace
- [x] statistics overlay
- [ ] authored story dialogue/cutscene production
- [ ] additional challenge variants/content packs

## Phase 8 — Persistence / Localization / Build — CODE COMPLETE / BUILD VALIDATION REQUIRED

- [x] local JSON save
- [x] backup/fallback load
- [x] migration through save v4
- [x] Story/Tournament/Challenge completion counters
- [x] persistent Career and Dream Team state
- [x] persistent AI/accessibility/audio settings
- [x] VI/EN localization foundation
- [x] Windows development build command
- [x] Android development APK build command
- [x] repository sanity workflow definition
- [ ] Unity Windows build execution
- [ ] Unity Android build execution
- [ ] physical-device install/startup/save test
- [ ] final full localization pass for every current hard-coded debug/alpha string

## Phase 9 — Commercial Production — EXTERNAL / ASSET / LICENSE GATE

The current repository is a fan-prototype/offline alpha. A commercial store release requires work that cannot be completed purely by gameplay code in this environment:

- [ ] license Haikyuu IP **or** replace Haikyuu names/designs/branding/story with original IP
- [ ] final production-quality character models/sprites, rigs and signature animations
- [ ] final courts, menus, icons, splash/store assets and music/audio
- [ ] accessibility/device UI polish across target aspect ratios
- [ ] performance profiling and optimization on low/mid/high Android devices
- [ ] crash/ANR monitoring plan and store compliance work
- [ ] release signing, package identifiers, privacy/store metadata

## Release Candidate Exit Gate

PR #1 stays draft until all of these pass:

1. Unity import/compile: **0 errors**.
2. `Haikyuu > Validation > Validate Project Data`: **pass**.
3. Quick/Story/Career/Tournament/Training/Challenge/DreamTeam/Arcade3v3: **playable without exceptions/soft locks**.
4. Repeated 3v3 ↔ 6v6 switching: **all correct active players and rotations**.
5. Libero never serves; pause stays paused through hit-stop.
6. Save v1/v2/v3 → v4 migration: **progress retained**.
7. F10 AI/presentation/audio settings persist and apply immediately.
8. Keyboard, gamepad and mobile touch controls pass regression.
9. Windows development build launches and completes a match.
10. Android APK installs on a physical device; touch control usable.
11. Representative Android device: record FPS/memory and resolve blocking performance issues.

Once the code/runtime gates pass, the repository is a validated offline fan-game alpha. Store/commercial release still depends on the Phase 9 asset/IP/release gates.
