using System;
using System.Collections.Generic;

namespace HaikyuuGame.Persistence
{
    [Serializable]
    public sealed class SaveGameData
    {
        public int version = 4;
        public string language = "vi";
        public int playerLevel = 1;
        public int playerExperience;
        public int coins;
        public int matchesPlayed;
        public int matchesWon;
        public int storyChapter;
        public bool storyCompleted;
        public int tournamentsWon;
        public int challengesCompleted;
        public List<string> unlockedCharacterIds = new List<string>();
        public List<string> dreamTeamCharacterIds = new List<string>();
        public CareerSaveData career = new CareerSaveData();
        public GameSettingsSaveData settings = new GameSettingsSaveData();

        public static SaveGameData CreateDefault()
        {
            SaveGameData data = new SaveGameData();
            data.unlockedCharacterIds.Add("hinata_shoyo");
            data.unlockedCharacterIds.Add("kageyama_tobio");
            data.unlockedCharacterIds.Add("nishinoya_yu");
            data.unlockedCharacterIds.Add("daichi_sawamura");
            data.unlockedCharacterIds.Add("tanaka_ryunosuke");
            data.unlockedCharacterIds.Add("tsukishima_kei");
            return data;
        }
    }

    [Serializable]
    public sealed class CareerSaveData
    {
        public string playerName = "Rookie";
        public string role = "OutsideHitter";
        public int season = 1;
        public int week = 1;
        public int trainingPoints;
        public int attack = 50;
        public int serve = 50;
        public int set = 50;
        public int receive = 50;
        public int block = 50;
        public int jump = 50;
        public int speed = 50;
    }

    [Serializable]
    public sealed class GameSettingsSaveData
    {
        public int aiDifficulty = 1;
        public float masterVolume = 1f;
        public float musicVolume = 0.8f;
        public float sfxVolume = 1f;
        public bool vibration = true;
        public bool reducedCinematics;
        public bool advancedControls;
        public bool screenShake = true;
        public bool highContrastUi;
    }
}
