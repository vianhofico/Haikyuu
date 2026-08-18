using HaikyuuGame.Gameplay.Character;
using HaikyuuGame.Persistence;

namespace HaikyuuGame.Progression
{
    public sealed class ProgressionService
    {
        private readonly SaveGameData _save;

        public ProgressionService(SaveGameData save)
        {
            _save = save;
        }

        public string ApplyMatchResult(bool won)
        {
            _save.matchesPlayed++;
            _save.coins += won ? 120 : 45;
            _save.playerExperience += won ? 100 : 45;

            if (won)
            {
                _save.matchesWon++;
            }

            while (_save.playerExperience >= ExperienceForNextLevel(_save.playerLevel))
            {
                _save.playerExperience -= ExperienceForNextLevel(_save.playerLevel);
                _save.playerLevel++;
            }

            if (won && _save.matchesWon % 2 == 0)
            {
                return UnlockNextCharacter();
            }

            return null;
        }

        private string UnlockNextCharacter()
        {
            for (int i = 0; i < HaikyuuRosterCatalog.All.Count; i++)
            {
                string id = HaikyuuRosterCatalog.All[i].Id;
                if (!_save.unlockedCharacterIds.Contains(id))
                {
                    _save.unlockedCharacterIds.Add(id);
                    return HaikyuuRosterCatalog.All[i].DisplayName;
                }
            }

            return null;
        }

        private static int ExperienceForNextLevel(int level)
        {
            return 150 + ((level - 1) * 35);
        }
    }
}
