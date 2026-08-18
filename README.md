# Haikyuu Volleyball Game

Offline-first anime volleyball fan prototype built with Unity 6 + C#. The volleyball engine is data-driven and separated from character/IP data so the gameplay layer can later be reused with licensed or original content.

> **IP note:** Haikyuu names and character references in this repository are fan-prototype/gameplay-research data. A commercial release must use appropriately licensed IP or replace protected names, character designs, school branding, dialogue, story and artwork with original content.

## Current milestone — Feature-Complete Offline Alpha / RC 0.9

### Volleyball gameplay

- 6v6 serve → receive/dig → set → spike → block rally loop
- Arcade 3v3 using the same physics, AI and scoring core
- three-contact possession rules; block does not consume a team contact
- double-contact and four-contact faults
- back-row attack and libero attack-height restrictions
- side-out rotation with libero replacement that skips the prototype service slot
- quick 11-point matches plus best-of-three 25/25/15 scoring
- manual serve timing when the controlled player reaches the service slot
- Early / Good / Perfect / Late timing grades affecting control and power
- trajectory prediction and ballistic receive/set targeting
- local role-aware AI for receivers, setters, attackers and blockers
- Flow/momentum system
- character archetypes, signature modifiers and pair synergy

### Characters and teams

- **48 playable Haikyuu fan-prototype character profiles**
- stats, positions, archetypes and skill IDs per profile
- major-school team presets plus role-correct support profiles where the 48-character roster does not contain a full six
- setter/attacker synergy, twin-style quick pairing, southpaw spin, flexible-wrist spin, guess block, bad-set attack and other signature gameplay hooks

### Modes

- Quick Match 6v6
- Arcade 3v3
- Story — 10 chapters, opponent progression, chapter/objective intro and final completion reward
- Career — custom runtime player, role switching, seven trainable stats, weeks/seasons and training points
- Tournament — deterministic bracket/opponent progression, championship completion and reward
- Training — live technique/action counters
- Challenge — objective/progress tracking and persistent completion reward
- Dream Team — persistent seven-slot role-filtered team builder (six court players + libero)

### Meta and presentation

- versioned local JSON save (**v3**) with backup/fallback migration
- XP, coins, wins, unlock progression and completion counters
- Vietnamese and English localization foundation
- keyboard and mobile touch controls
- procedural hit sounds; no paid audio assets required for this prototype
- camera tracking/punch, pause-safe anime hit-stop, ball trail, PERFECT feedback and impact streak VFX
- stylized procedural player visuals, team palettes, arena stands/crowd/lights
- `F2` live match statistics overlay
- `P` last-rally trajectory replay trace
- Windows and Android development build menu commands
- Unity project-data validator plus standalone repository sanity validator

## Open and play

1. Install the Unity version pinned in `ProjectSettings/ProjectVersion.txt`.
2. Open the repository folder from Unity Hub.
3. Run **Haikyuu > Validation > Validate Project Data**.
4. Run **Haikyuu > Setup > Generate Playable Core Scene**.
5. Open `Assets/Scenes/PlayableCore.unity` and press Play.
6. Select a mode from the in-game menu.

## Controls

| Input | Action |
|---|---|
| WASD / Arrow keys | Move |
| Space | Jump |
| F or J | Context action |
| Z | Receive / Dig |
| X | Set |
| C | Spike; serve while serving |
| V | Block |
| R | Reset rally |
| M / Esc | Mode menu / resume |
| L | Toggle VI / EN |
| F2 | Match statistics |
| P | Last-rally trajectory replay |
| 1–7 in Career menu | Train Attack / Serve / Set / Receive / Block / Jump / Speed |
| Tab in Career menu | Change Career role |

On mobile the prototype provides a movement touch area plus **JUMP** and **ACTION** zones.

## Mode notes

### Career

The Career player is built at runtime from the save file. Training modifies the same stats used by the normal gameplay/AI systems, so progression does not use a separate combat model.

### Dream Team

Open Dream Team, then use the `<` / `>` controls beside each role slot. The seven selected character IDs are persisted in the local save and reapplied on subsequent sessions.

### Story

Each chapter displays its title, opponent and objective. Winning progresses to the next opponent. Clearing the final chapter records campaign completion and awards a one-time completion reward.

### Challenge

The current challenge requires three PERFECT contacts and a match win. Completion is persisted and rewards coins through the same runtime save owner as the rest of progression.

## Build

From Unity Editor:

- **Haikyuu > Build > Windows Development** → `Builds/Windows/HaikyuuPrototype.exe`
- **Haikyuu > Build > Android Development APK** → `Builds/Android/HaikyuuPrototype.apk`

Android requires Android Build Support installed in Unity Hub.

## Architecture

```text
BallContact -> TeamPossession -> RallyController -> MatchScore
     |                    |                |
     v                    v                v
ContactTiming      Rules/Faults       Rotation/Libero
     |                                     |
     v                                     v
PlayerActor -> Stats/Archetype -> TeamMomentum
     |                |
     v                v
AI decision       CharacterSynergy

GameSessionState -> ModeMatchupDirector -> MatchRosterController
       |                    |                     |
       v                    v                     v
Story/Career         TeamPreset             Custom Lineups
DreamTeam/3v3        Progression             Runtime Profiles

SaveGameService -> XP / Career / Dream Team / Story / Tournament / Challenge
```

## Repository layout

```text
Assets/HaikyuuGame/
  Runtime/
    Core/
    Gameplay/
      Ball/
      Character/
      Input/
      Match/
      Player/
      Presentation/
      Skills/
      Teams/
      UI/
    Career/
    Localization/
    Meta/
    Persistence/
    Progression/
    Story/
    Tournament/
    Training/
  Editor/
Docs/
Tools/
Packages/
ProjectSettings/
```

## Release gate

The codebase is at a feature-complete offline-alpha/release-candidate stage, **not a validated store release**. Before PR #1 can leave draft status, the following must pass in an environment with the Unity Editor and target devices:

- Unity import/compile with no errors
- `Haikyuu > Validation > Validate Project Data`
- Play Mode regression across all eight modes
- repeated 3v3 ↔ 6v6 switching and rotations
- libero/service-slot and hit-stop/menu-pause regression
- save v1/v2 → v3 migration validation
- Windows development build and runtime test
- Android APK build, installation and touch-input test on a physical device
- FPS/memory/readability checks on representative mobile hardware

See GitHub release-gate issues and `Docs/ROADMAP.md` for the remaining validation/production work.
