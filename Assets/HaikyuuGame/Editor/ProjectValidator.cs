#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using HaikyuuGame.Career;
using HaikyuuGame.Gameplay.Character;
using HaikyuuGame.Gameplay.Teams;
using HaikyuuGame.Meta;
using HaikyuuGame.Persistence;
using HaikyuuGame.Story;
using UnityEditor;
using UnityEngine;

namespace HaikyuuGame.Editor
{
    public static class ProjectValidator
    {
        private static readonly string[] PresetIds =
        {
            "karasuno", "inarizaki", "aoba_johsai", "nekoma", "date_tech",
            "shiratorizawa", "fukurodani", "kamomedai", "all_star", "dream_team", "training"
        };

        [MenuItem("Haikyuu/Validation/Validate Project Data")]
        public static void ValidateProjectData()
        {
            IReadOnlyList<RuntimeCharacterProfile> roster = HaikyuuRosterCatalog.All;
            if (roster.Count != 48)
            {
                throw new InvalidOperationException($"Expected 48 roster entries, found {roster.Count}.");
            }

            HashSet<string> ids = new HashSet<string>();
            HashSet<string> schools = new HashSet<string>();
            for (int i = 0; i < roster.Count; i++)
            {
                RuntimeCharacterProfile profile = roster[i];
                if (string.IsNullOrWhiteSpace(profile.Id) || !ids.Add(profile.Id))
                {
                    throw new InvalidOperationException($"Invalid or duplicate character id at index {i}: {profile.Id}");
                }

                if (string.IsNullOrWhiteSpace(profile.DisplayName))
                {
                    throw new InvalidOperationException($"Character {profile.Id} is missing a display name.");
                }

                schools.Add(profile.School);
                ValidateStat(profile.Id, "attack", profile.Stats.attack);
                ValidateStat(profile.Id, "serve", profile.Stats.serve);
                ValidateStat(profile.Id, "set", profile.Stats.set);
                ValidateStat(profile.Id, "receive", profile.Stats.receive);
                ValidateStat(profile.Id, "block", profile.Stats.block);
                ValidateStat(profile.Id, "jump", profile.Stats.jump);
                ValidateStat(profile.Id, "speed", profile.Stats.speed);
                ValidateStat(profile.Id, "stamina", profile.Stats.stamina);
                ValidateStat(profile.Id, "technique", profile.Stats.technique);
                ValidateStat(profile.Id, "mental", profile.Stats.mental);
            }

            for (int i = 0; i < PresetIds.Length; i++)
            {
                TeamPreset preset = TeamPresetCatalog.Get(PresetIds[i]);
                if (preset == null || preset.StartingSix == null || preset.StartingSix.Length != 6)
                {
                    throw new InvalidOperationException($"Invalid team preset: {PresetIds[i]}");
                }
            }

            if (StoryCampaignCatalog.All.Count != 10)
            {
                throw new InvalidOperationException($"Expected 10 story chapters, found {StoryCampaignCatalog.All.Count}.");
            }

            if (SaveGameService.CurrentVersion != 3)
            {
                throw new InvalidOperationException($"Expected save version 3, found {SaveGameService.CurrentVersion}.");
            }

            SaveGameData defaultSave = SaveGameData.CreateDefault();
            DreamTeamService dreamTeam = new DreamTeamService(defaultSave);
            if (defaultSave.dreamTeamCharacterIds == null || defaultSave.dreamTeamCharacterIds.Count != 7)
            {
                throw new InvalidOperationException("Dream Team must contain six court slots plus one libero slot.");
            }

            for (int slot = 0; slot < 7; slot++)
            {
                RuntimeCharacterProfile profile = dreamTeam.GetProfile(slot);
                if (profile == null || profile.Role != DreamTeamService.RoleForSlot(slot))
                {
                    throw new InvalidOperationException($"Invalid Dream Team role/profile at slot {slot}.");
                }
            }

            RuntimeCharacterProfile career = CareerProfileFactory.Create(defaultSave.career);
            if (career == null || career.Id != "career_player")
            {
                throw new InvalidOperationException("Career runtime profile factory failed.");
            }

            int gameModeCount = Enum.GetValues(typeof(GameMode)).Length;
            if (gameModeCount != 8)
            {
                throw new InvalidOperationException($"Expected 8 game modes, found {gameModeCount}.");
            }

            Debug.Log(
                $"Project validation passed: {roster.Count} playable characters, {schools.Count} schools/groups, "
                + $"{PresetIds.Length} team presets, {StoryCampaignCatalog.All.Count} story chapters, {gameModeCount} modes, save v{SaveGameService.CurrentVersion}.");
        }

        private static void ValidateStat(string id, string stat, int value)
        {
            if (value < 1 || value > 100)
            {
                throw new InvalidOperationException($"{id}.{stat} must be 1..100, got {value}.");
            }
        }
    }
}
#endif
