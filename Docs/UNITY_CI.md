# Unity CI / Headless Smoke Test

The repository contains `.github/workflows/unity-ci.yml`, which has two layers:

1. `repository-sanity` runs `python3 Tools/validate_repo.py` without requiring Unity.
2. `unity-linux-smoke` builds the project with Unity `6000.3.0f1`, launches the generated Linux player with `-batchmode -nographics -ciSmoke`, waits for the runtime bootstrap/rally/ball to initialize, and requires `CI_SMOKE_PASS` in the player log.

## Required GitHub Actions secrets

GameCI requires an activated Unity license for Unity-based build/test actions.

### Unity Personal

Create these repository Actions secrets:

- `UNITY_LICENSE` — full contents of your activated `.ulf` license file
- `UNITY_EMAIL` — Unity account email
- `UNITY_PASSWORD` — Unity account password

For a current Personal license, activate Unity locally with Unity Hub first, then copy the generated `.ulf` file into the `UNITY_LICENSE` secret.

### Unity Pro

Create:

- `UNITY_SERIAL`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

`UNITY_LICENSE` can be omitted for the Pro path.

## What the smoke build does

`HaikyuuGame.Editor.CiBuild.BuildLinuxSmoke` performs these operations inside Unity:

1. Runs `ProjectValidator.ValidateProjectData()`.
2. Generates `Assets/Scenes/PlayableCore.unity` using the same editor path documented for local play.
3. Builds a Development Linux player to `build/StandaloneLinux64/HaikyuuSmoke.x86_64`.

The workflow then executes:

```bash
build/StandaloneLinux64/HaikyuuSmoke.x86_64 -batchmode -nographics -logFile smoke-player.log -ciSmoke
```

`CiSmokeProbe` keeps the player alive for eight realtime seconds, fails on runtime exceptions/asserts, checks that `PlayableCoreBootstrap`, `RallyController`, and `VolleyballBall` exist, and exits with code 0 only after writing `CI_SMOKE_PASS`.

## Artifacts

When Unity licensing is configured and the smoke run passes, Actions uploads:

- `unity-smoke-log`
- `Haikyuu-Linux-Smoke`

Both are retained for seven days.

## If Unity secrets are missing

The Unity build portion is intentionally skipped with a warning instead of producing a misleading activation failure. Repository sanity still runs. Add the required secrets and rerun the workflow to perform the real Unity compile/build/runtime smoke test.
