using HaikyuuGame.Core;
using HaikyuuGame.Gameplay.Match;
using HaikyuuGame.Localization;
using HaikyuuGame.Persistence;
using HaikyuuGame.Progression;
using HaikyuuGame.Story;
using UnityEngine;

namespace HaikyuuGame.Meta
{
    public sealed class GameShellController : MonoBehaviour
    {
        private RallyController _rally;
        private SaveGameService _saveService;
        private ProgressionService _progression;
        private readonly LocalizationService _localization = new LocalizationService();
        private string _toast;
        private float _toastUntil;

        public SaveGameData Save => _saveService != null ? _saveService.Current : null;

        public void Initialize(RallyController rally)
        {
            _rally = rally;
            _saveService = new SaveGameService();
            SaveGameData save = _saveService.Load();
            _localization.SetLanguage(save.language);
            _progression = new ProgressionService(save);
            _rally.MatchCompleted += OnMatchCompleted;
        }

        private void OnDestroy()
        {
            if (_rally != null)
            {
                _rally.MatchCompleted -= OnMatchCompleted;
            }

            _saveService?.Save();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.L))
            {
                ToggleLanguage();
            }
        }

        private void OnMatchCompleted(TeamSide winner)
        {
            bool won = _rally.HumanPlayer != null && winner == _rally.HumanPlayer.Team;
            string unlock = _progression.ApplyMatchResult(won);
            _saveService.Save();

            _toast = won ? _localization.T("match.win") : _localization.T("match.loss");
            if (!string.IsNullOrEmpty(unlock))
            {
                _toast += $" | Unlocked: {unlock}";
            }

            _toastUntil = Time.unscaledTime + 4f;
        }

        private void ToggleLanguage()
        {
            SaveGameData save = _saveService.Current;
            save.language = save.language == "vi" ? "en" : "vi";
            _localization.SetLanguage(save.language);
            _saveService.Save();
        }

        private void OnGUI()
        {
            if (_saveService == null || _saveService.Current == null)
            {
                return;
            }

            SaveGameData save = _saveService.Current;
            StoryChapter chapter = StoryCampaignCatalog.Get(save.storyChapter);
            float width = 310f;
            float x = Screen.width - width - 16f;
            GUI.Box(new Rect(x, 16f, width, 128f), _localization.T("shell.title"));
            GUI.Label(new Rect(x + 16f, 44f, width - 32f, 22f), $"{_localization.T("shell.level")}: {save.playerLevel} | {_localization.T("shell.coins")}: {save.coins}");
            GUI.Label(new Rect(x + 16f, 66f, width - 32f, 22f), $"{_localization.T("shell.wins")}: {save.matchesWon}/{save.matchesPlayed}");
            GUI.Label(new Rect(x + 16f, 88f, width - 32f, 22f), $"Story: {chapter.Index + 1}. {chapter.Title}");

            if (GUI.Button(new Rect(x + 16f, 109f, 120f, 25f), $"Lang: {save.language.ToUpperInvariant()}"))
            {
                ToggleLanguage();
            }

            if (Time.unscaledTime < _toastUntil)
            {
                GUI.Box(new Rect((Screen.width - 360f) * 0.5f, 205f, 360f, 42f), _toast);
            }
        }
    }
}
