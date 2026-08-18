# Haikyuu Volleyball Game

Offline-first anime volleyball fan prototype built with Unity 6 + C#. Gameplay is data-driven so the volleyball engine is separated from character/IP data.

> **IP note:** Haikyuu names and character references in this repository are for fan-prototype/gameplay research. A commercial release should replace protected names, character designs, school branding, dialogue, story and artwork with original IP unless appropriately licensed.

## Current milestone — Playable Alpha 0.6

Implemented:

- 6v6 rally gameplay with serve, receive/dig, set, spike and block
- three-contact possession rules; block does not consume a team contact
- side-out rotation and automatic libero replacement
- quick 11-point matches plus best-of-three 25/25/15 scoring
- manual serve timing when the controlled player reaches the service slot
- Early / Good / Perfect / Late contact grades that affect control and power
- local role-aware AI for receivers, setters, attackers and blockers
- ball trajectory prediction and ballistic sets/receives
- Flow/momentum system
- character synergy (including precision-setter/speed-decoy and twin-style quick pairings)
- signature gameplay modifiers such as southpaw spin, flexible-wrist spin, guess block and bad-set attack
- **48 playable Haikyuu fan-prototype character profiles** with stats, roles, archetypes and skill IDs
- team presets and story-specific opponents, with role-correct support profiles where the 48-character launch roster does not contain a full six
- Quick Match, Story, Career, Tournament, Training, Challenge and Dream Team session modes
- 10-chapter story progression data
- career weeks, training points and stat development data
- deterministic local tournament bracket generation
- XP, coins, wins and character unlock progression
- versioned local JSON save + backup/fallback load
- Vietnamese and English localization foundation
- keyboard and basic mobile touch controls
- procedural hit sounds; no paid audio assets required for the prototype
- camera punch, brief anime hit-stop, ball trail, PERFECT contact overlay and impact streak VFX
- procedural stylized player visuals, team palettes, arena stands/crowd/lights
- one-click editor build commands for Windows development build and Android APK
- project/roster validator and repository sanity script

## Open and play

1. Install the Unity version pinned in `ProjectSettings/ProjectVersion.txt` (Unity 6.3 LTS family).
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
| F / J | Context action |
| Z | Receive / Dig |
| X | Set |
| C | Spike; serve while serving |
| V | Block |
| R | Reset rally |
| M / Esc | Mode menu |
| L | Toggle VI / EN |

On mobile, the prototype provides a left movement area plus **JUMP** and **ACTION** touch zones.

## Build

From Unity Editor:

- **Haikyuu > Build > Windows Development** → `Builds/Windows/HaikyuuPrototype.exe`
- **Haikyuu > Build > Android Development APK** → `Builds/Android/HaikyuuPrototype.apk`

Android requires the Android Build Support module installed in Unity Hub.

## Gameplay architecture

```text
BallContact -> TeamPossession -> RallyController -> MatchScore
     |                                  |
     v                                  v
ContactTiming                       Rotation/Libero
     |                                  |
     v                                  v
PlayerActor -> Archetype/Stats -> TeamMomentum
     |                |
     v                v
AI decision       CharacterSynergy

GameSessionState -> ModeMatchupDirector -> TeamPreset -> MatchRosterController
SaveGameService  -> Progression / Story / Career
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

See `Docs/ARCHITECTURE.md` and `Docs/ROADMAP.md` for the production design. The remaining release-critical validation is Unity Editor compile/Play Mode/device testing because the automation environment used to author this repository does not contain the Unity Editor.
