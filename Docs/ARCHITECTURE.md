# Architecture

## Principles

1. Gameplay code never depends on protected character names or school names.
2. Character identity is data-driven through archetypes, stats and skills.
3. Match rules, ball simulation, player control, AI, presentation and meta systems remain separate modules.
4. Offline gameplay is authoritative. Online services can be layered on later without rewriting the match core.
5. Placeholder visuals are acceptable until ball feel and rally flow pass the vertical-slice gate.

## Runtime layers

```text
Bootstrap
  |
  +-- Match / Rally state
  |     +-- score
  |     +-- serving team
  |     +-- rally reset
  |
  +-- Ball
  |     +-- velocity / contact
  |     +-- last-touch ownership
  |     +-- trajectory prediction
  |
  +-- Players
  |     +-- human input
  |     +-- local AI
  |     +-- movement / jump
  |     +-- contextual contact
  |
  +-- Character Data
        +-- archetype
        +-- base stats
        +-- skills
        +-- animation/VFX/audio profiles (future)
```

## IP-safe character model

Do not write code such as `if (characterName == "Hinata")`.

Use a generic gameplay identity instead:

```text
SpeedDecoy + PrecisionSetter -> ZeroTempo-style combo
PowerAce                     -> high-force spike profile
ReadBlocker                  -> prediction/block timing profile
GuardianLibero               -> expanded emergency receive profile
StrategistSetter             -> pattern-analysis set selection
```

A fan data pack may map those profiles to recognizable prototype characters. A commercial data pack can map the exact same gameplay code to original characters.

## Planned modules

- `Volley.Core`
- `Volley.Gameplay`
- `Volley.AI`
- `Volley.Characters`
- `Volley.UI`
- `Volley.Story`
- `Volley.Career`
- `Volley.Persistence`
- `Volley.Audio`
- `Volley.Tests`

Assembly definitions will be introduced once the playable core settles enough that module boundaries are not changing every iteration.

## Save architecture (future)

Versioned local save:

```text
SaveData
  Profile
  Story
  Characters
  Career
  Teams
  Settings
  Achievements
  MatchHistory
```

Every schema change must have a migration path.
