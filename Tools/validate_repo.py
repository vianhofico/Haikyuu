from pathlib import Path
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

for path in (ROOT / "Assets/HaikyuuGame").rglob("*.cs"):
    text = path.read_text(encoding="utf-8")
    if text.count("{") != text.count("}"):
        errors.append(f"brace mismatch: {path.relative_to(ROOT)}")

if errors:
    print("Repository validation failed:")
    for error in errors:
        print(f"- {error}")
    sys.exit(1)

print("Repository sanity validation passed.")
