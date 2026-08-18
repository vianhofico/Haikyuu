# Haikyuu Volleyball Game

Anime volleyball game prototype built with Unity 6 + C#. The repository starts from a gameplay-first architecture: ball feel, movement, receive/set/spike/block flow, match rules, AI and character archetypes before large-scale content production.

> IP note: this repository can be used as a Haikyuu fan prototype for gameplay research. A commercial release should replace protected names, character designs, school branding, story and artwork with original IP unless properly licensed.

## Current milestone — Playable Core 0.1

Implemented foundation:

- Unity 6 project structure
- Runtime-generated volleyball court for fast iteration
- 6v6 placeholder teams
- One human-controlled player + lightweight local AI
- Ball physics wrapper and trajectory prediction
- Jump + contextual volleyball action
- Rally lifecycle and scoring
- Local 11-point quick set for prototype iteration
- Character archetype and skill data framework
- Editor command that generates a playable scene
- Architecture and production roadmap docs

This milestone deliberately uses primitive placeholder graphics. Final character art, animation, VFX, audio, story, roster and advanced volleyball rules come after the rally feels good.

## Open the project

1. Install Unity 6.3 LTS (the project version is pinned in `ProjectSettings/ProjectVersion.txt`).
2. Open the repository folder in Unity Hub.
3. In Unity choose **Haikyuu > Setup > Generate Playable Core Scene**.
4. Open `Assets/Scenes/PlayableCore.unity` if it is not opened automatically.
5. Press Play.

## Prototype controls

| Input | Action |
|---|---|
| WASD / Arrow keys | Move controlled player |
| Space | Jump |
| F or J | Context action: bump/set/spike |
| R | Reset current rally |

The controlled player is the highlighted left-side player. The current contextual action is intentionally simple; later milestones split it into receive, set, block, attack and skill inputs.

## Gameplay direction

Production target:

- Main mode: 6v6 volleyball
- Arcade mode: 3v3
- Story + Career + Dream Team + Tournament + Training + Challenge
- Character identity through archetypes, passives, active skills, combo skills and team chemistry
- Anime presentation through camera, hit-stop, speed lines, impact VFX and sound — not literal magic
- Offline-first architecture; online services are optional later
- Vietnamese + English localization
- Android + Windows first

## Repository layout

```text
Assets/HaikyuuGame/
  Runtime/
    Core/
    Gameplay/
      Ball/
      Character/
      Match/
      Player/
      Skills/
      UI/
  Editor/
Docs/
Packages/
ProjectSettings/
```

See `Docs/ARCHITECTURE.md` and `Docs/ROADMAP.md` for implementation details.
