using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HaikyuuGame.Persistence
{
    public sealed class SaveGameService
    {
        public const int CurrentVersion = 3;
        private const string FileName = "haikyuu_save.json";
        private const string BackupFileName = "haikyuu_save.backup.json";

        private static readonly string[] DefaultDreamTeam =
        {
            "kageyama_tobio",
            "bokuto_kotaro",
            "hinata_shoyo",
            "ushijima_wakatoshi",
            "hoshiumi_korai",
            "tsukishima_kei",
            "nishinoya_yu"
        };

        public SaveGameData Current { get; private set; }

        private string SavePath => Path.Combine(Application.persistentDataPath, FileName);
        private string BackupPath => Path.Combine(Application.persistentDataPath, BackupFileName);

        public SaveGameData Load()
        {
            Current = TryLoad(SavePath) ?? TryLoad(BackupPath) ?? SaveGameData.CreateDefault();
            Migrate(Current);
            return Current;
        }

        public void Save()
        {
            if (Current == null)
            {
                Current = SaveGameData.CreateDefault();
            }

            Migrate(Current);
            Current.version = CurrentVersion;
            string json = JsonUtility.ToJson(Current, true);

            try
            {
                if (File.Exists(SavePath))
                {
                    File.Copy(SavePath, BackupPath, true);
                }

                File.WriteAllText(SavePath, json);
            }
            catch (IOException exception)
            {
                Debug.LogError($"Unable to save game: {exception.Message}");
            }
        }

        public void Reset()
        {
            Current = SaveGameData.CreateDefault();
            Migrate(Current);
            Save();
        }

        private static SaveGameData TryLoad(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<SaveGameData>(json);
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning($"Ignoring invalid save at {path}: {exception.Message}");
                return null;
            }
        }

        private static void Migrate(SaveGameData data)
        {
            if (data == null)
            {
                return;
            }

            if (data.unlockedCharacterIds == null)
            {
                data.unlockedCharacterIds = new List<string>();
            }

            if (data.dreamTeamCharacterIds == null)
            {
                data.dreamTeamCharacterIds = new List<string>();
            }

            while (data.dreamTeamCharacterIds.Count < DefaultDreamTeam.Length)
            {
                data.dreamTeamCharacterIds.Add(DefaultDreamTeam[data.dreamTeamCharacterIds.Count]);
            }

            if (data.dreamTeamCharacterIds.Count > DefaultDreamTeam.Length)
            {
                data.dreamTeamCharacterIds.RemoveRange(DefaultDreamTeam.Length, data.dreamTeamCharacterIds.Count - DefaultDreamTeam.Length);
            }

            if (data.career == null)
            {
                data.career = new CareerSaveData();
            }

            if (data.settings == null)
            {
                data.settings = new GameSettingsSaveData();
            }

            if (data.version < 2)
            {
                data.settings.screenShake = true;
            }

            // v3 adds campaign/tournament/challenge completion counters. Their
            // default zero/false values are already backward-compatible.
            data.version = CurrentVersion;
        }
    }
}
