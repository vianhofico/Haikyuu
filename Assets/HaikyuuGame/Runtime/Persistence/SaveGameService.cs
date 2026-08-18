using System.IO;
using UnityEngine;

namespace HaikyuuGame.Persistence
{
    public sealed class SaveGameService
    {
        public const int CurrentVersion = 1;
        private const string FileName = "haikyuu_save.json";
        private const string BackupFileName = "haikyuu_save.backup.json";

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
                data.unlockedCharacterIds = new System.Collections.Generic.List<string>();
            }

            if (data.career == null)
            {
                data.career = new CareerSaveData();
            }

            if (data.settings == null)
            {
                data.settings = new GameSettingsSaveData();
            }

            data.version = CurrentVersion;
        }
    }
}
