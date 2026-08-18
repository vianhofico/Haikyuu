from pathlib import Path
import json
import re
import sys

ROOT = Path(__file__).resolve().parents[1]
required = [
    "Assets/HaikyuuGame/Runtime/Core/PlayableCoreBootstrap.cs",
    "Assets/HaikyuuGame/Runtime/Gameplay/Match/RallyController.cs",
    "Assets/HaikyuuGame/Runtime/Gameplay/Match/TeamPossession.cs",
    "Assets/HaikyuuGame/Runtime/Gameplay/Match/TeamRotation.cs",
    "Assets/HaikyuuGame/Runtime/Gameplay/Character/HaikyuuRosterCatalog.cs",
    "Assets/HaikyuuGame/Runtime/Persistence/SaveGameService.cs",
    "Assets/HaikyuuGame/Runtime/Story/StoryCampaignCatalog.cs",
    "Assets/HaikyuuGame/Runtime/Career/CareerService.cs",
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

for path in (ROOT / "Assets/HaikyuuGame").rglob("*.cs"):
    text = path.read_text(encoding="utf-8")
    if "using System;" in text and re.search(r'(?<![\w.])Random\.(Range|value)', text):
        errors.append(f"ambiguous Random usage in {path.relative_to(ROOT)}; qualify UnityEngine.Random")
    if "<<<<<<<" in text or ">>>>>>>" in text or "=======" in text:
        errors.append(f"merge conflict marker in {path.relative_to(ROOT)}")

version_file = ROOT / "ProjectSettings/ProjectVersion.txt"
if version_file.exists() and "m_EditorVersion:" not in version_file.read_text(encoding="utf-8"):
    errors.append("ProjectVersion.txt is missing m_EditorVersion")

if errors:
    print("Repository validation failed:")
    for error in errors:
        print(f"- {error}")
    sys.exit(1)

print("Repository sanity validation passed.")
