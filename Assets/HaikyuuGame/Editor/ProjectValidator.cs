#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using HaikyuuGame.Gameplay.Character;
using UnityEditor;
using UnityEngine;

namespace HaikyuuGame.Editor
{
    public static class ProjectValidator
    {
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

            Debug.Log($"Haikyuu project validation passed: {roster.Count} characters across {schools.Count} schools/groups.");
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
