from pathlib import Path
import json
import re
import sys

ROOT = Path(__file__).resolve().parents[1]
required = [
    "Assets/HaikyuuGame/Runtime/Core/PlayableCoreBootstrap.cs",
    "Assets/HaikyuuGame/Runtime/Gameplay/AI/AiDifficultyRuntime.cs",
    "Assets/HaikyuuGame/Runtime/Gameplay/Match/RallyController.cs",
    "Assets/HaikyuuGame/Runtime/Gameplay/Match/TeamPossession.cs",
    "Assets/HaikyuuGame/Runtime/Gameplay/Match/TeamRotation.cs",
    "Assets/HaikyuuGame/Runtime/Gameplay/Character/HaikyuuRosterCatalog.cs",
    "Assets/HaikyuuGame/Runtime/Gameplay/Input/TouchInputRouter.cs",
    "Assets/HaikyuuGame/Runtime/Gameplay/Presentation/RuntimePresentationSettings.cs",
    "Assets/HaikyuuGame/Runtime/Persistence/SaveGameService.cs",
    "Assets/HaikyuuGame/Runtime/Story/StoryCampaignCatalog.cs",
    "Assets/HaikyuuGame/Runtime/Story/StoryPresentationController.cs",
    "Assets/HaikyuuGame/Runtime/Career/CareerService.cs",
    "Assets/HaikyuuGame/Runtime/Career/CareerProfileFactory.cs",
    "Assets/HaikyuuGame/Runtime/Meta/DreamTeamService.cs",
    "Assets/HaikyuuGame/Runtime/Meta/RuntimeSettingsOverlay.cs",
    "Assets/HaikyuuGame/Runtime/Progression/CompletionProgressController.cs",
    "Assets/HaikyuuGame/Runtime/Training/ChallengeController.cs",
    "Assets/HaikyuuGame/Runtime/Gameplay/Presentation/RallyReplayTrace.cs",
    "Assets/HaikyuuGame/Runtime/Gameplay/UI/MatchStatisticsOverlay.cs",
    "Assets/HaikyuuGame/Runtime/Gameplay/Ball/ContactTimingGrade.cs",
    "ProjectSettings/ProjectVersion.txt",
    "Packages/manifest.json",
]

errors = []
for rel in required:
    if not (ROOT / rel).exists():
        errors.append(f"missing required file: {rel}")

roster_path = ROOT / "Assets/HaikyuuGame/Runtime/Gameplay/Character/HaikyuuRosterCatalog.cs"
if roster_path.exists():
    roster = roster_path.read_text(encoding="utf-8")
    count = len(re.findall(r'^\s*P\("', roster, flags=re.MULTILINE))
    if count != 48:
        errors.append(f"expected 48 roster entries, found {count}")
    ids = re.findall(r'^\s*P\("([^"]+)"', roster, flags=re.MULTILINE)
    duplicates = sorted({item for item in ids if ids.count(item) > 1})
    if duplicates:
        errors.append(f"duplicate roster ids: {', '.join(duplicates)}")

manifest = ROOT / "Packages/manifest.json"
if manifest.exists():
    try:
        json.loads(manifest.read_text(encoding="utf-8"))
    except json.JSONDecodeError as exc:
        errors.append(f"invalid Packages/manifest.json: {exc}")

save_service = ROOT / "Assets/HaikyuuGame/Runtime/Persistence/SaveGameService.cs"
if save_service.exists():
    text = save_service.read_text(encoding="utf-8")
    if "CurrentVersion = 4" not in text:
        errors.append("save service is not pinned to version 4")

mode_file = ROOT / "Assets/HaikyuuGame/Runtime/Meta/GameMode.cs"
if mode_file.exists() and "Arcade3v3" not in mode_file.read_text(encoding="utf-8"):
    errors.append("Arcade3v3 mode is missing")

rotation_file = ROOT / "Assets/HaikyuuGame/Runtime/Gameplay/Match/TeamRotation.cs"
if rotation_file.exists():
    rotation = rotation_file.read_text(encoding="utf-8")
    if "SetActivePlayerCount" not in rotation:
        errors.append("TeamRotation is missing active-player format support")
    if "slot = 4; slot < 6" not in rotation:
        errors.append("libero replacement must skip the prototype service slot")

possession_file = ROOT / "Assets/HaikyuuGame/Runtime/Gameplay/Match/TeamPossession.cs"
if possession_file.exists():
    possession = possession_file.read_text(encoding="utf-8")
    if "Double contact" not in possession or "Four contacts" not in possession:
        errors.append("possession fault reasons are incomplete")

rally_file = ROOT / "Assets/HaikyuuGame/Runtime/Gameplay/Match/RallyController.cs"
if rally_file.exists():
    rally = rally_file.read_text(encoding="utf-8")
    for required_snippet in ("Back-row attack fault", "Libero illegal attack", "SetPlayersPerSide"):
        if required_snippet not in rally:
            errors.append(f"RallyController missing required rule/format hook: {required_snippet}")

ai_file = ROOT / "Assets/HaikyuuGame/Runtime/Gameplay/AI/AiDifficultyRuntime.cs"
if ai_file.exists():
    ai = ai_file.read_text(encoding="utf-8")
    for name in ("Rookie", "Normal", "Advanced", "Elite", "National", "Legend"):
        if name not in ai:
            errors.append(f"AI difficulty missing profile: {name}")
    if "Bind(VolleyballTuning tuning)" not in ai:
        errors.append("AI difficulty is not bound to runtime tuning")

settings_overlay = ROOT / "Assets/HaikyuuGame/Runtime/Meta/RuntimeSettingsOverlay.cs"
if settings_overlay.exists():
    settings = settings_overlay.read_text(encoding="utf-8")
    for hook in ("screenShake", "reducedCinematics", "masterVolume", "sfxVolume", "aiDifficulty"):
        if hook not in settings:
            errors.append(f"runtime settings overlay missing: {hook}")

gamepad_file = ROOT / "Assets/HaikyuuGame/Runtime/Gameplay/Input/TouchInputRouter.cs"
if gamepad_file.exists():
    gamepad = gamepad_file.read_text(encoding="utf-8")
    if "JoystickButton0" not in gamepad or "JoystickButton1" not in gamepad:
        errors.append("basic gamepad jump/context bridge is missing")

for path in (ROOT / "Assets/HaikyuuGame").rglob("*.cs"):
    text = path.read_text(encoding="utf-8")
    if "using System;" in text and re.search(r'(?<![\w.])Random\.(Range|value)', text):
        errors.append(f"ambiguous Random usage in {path.relative_to(ROOT)}; qualify UnityEngine.Random")
    if "<<<<<<<" in text or ">>>>>>>" in text or "=======" in text:
        errors.append(f"merge conflict marker in {path.relative_to(ROOT)}")
    if text.count("{") != text.count("}"):
        errors.append(f"brace mismatch in {path.relative_to(ROOT)}")

version_file = ROOT / "ProjectSettings/ProjectVersion.txt"
if version_file.exists() and "m_EditorVersion:" not in version_file.read_text(encoding="utf-8"):
    errors.append("ProjectVersion.txt is missing m_EditorVersion")

if errors:
    print("Repository validation failed:")
    for error in errors:
        print(f"- {error}")
    sys.exit(1)

print("Repository sanity validation passed.")
