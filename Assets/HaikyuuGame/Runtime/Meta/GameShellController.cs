using System.Collections.Generic;
using HaikyuuGame.Career;
using HaikyuuGame.Core;
using HaikyuuGame.Gameplay;
using HaikyuuGame.Gameplay.Match;
using HaikyuuGame.Localization;
using HaikyuuGame.Persistence;
using HaikyuuGame.Progression;
using HaikyuuGame.Story;
using HaikyuuGame.Tournament;
using UnityEngine;

namespace HaikyuuGame.Meta
{
    public sealed class GameShellController : MonoBehaviour
    {
        private static readonly string[] TournamentTeams =
        {
            "Karasuno", "Nekoma", "Aoba Johsai", "Date Tech",
            "Shiratorizawa", "Fukurodani", "Inarizaki", "Kamomedai"
        };

        private RallyController _rally;
        private VolleyballTuning _tuning;
        private SaveGameService _saveService;
        private ProgressionService _progression;
        private CareerService _career;
        private readonly LocalizationService _localization = new LocalizationService();
        private IReadOnlyList<TournamentPairing> _tournamentPairings;
        private bool _menuOpen = true;
        private string _toast;
        private float _toastUntil;

        public SaveGameData Save => _saveService != null ? _saveService.Current : null;

        public void Initialize(RallyController rally, VolleyballTuning tuning)
        {
            _rally = rally;
            _tuning = tuning;
            _saveService = new SaveGameService();
            SaveGameData save = _saveService.Load();
            _localization.SetLanguage(save.language);
            _progression = new ProgressionService(save);
            _career = new CareerService(save.career);
            _rally.MatchCompleted += OnMatchCompleted;
            OpenMenu();
        }

        private void OnDestroy()
        {
            if (_rally != null)
            {
                _rally.MatchCompleted -= OnMatchCompleted;
            }

            Time.timeScale = 1f;
            _saveService?.Save();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.L))
            {
                ToggleLanguage();
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.M) || UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                if (_menuOpen)
                {
                    CloseMenuWithoutRestart();
                }
                else
                {
                    OpenMenu();
                }
            }
        }

        private void OnMatchCompleted(TeamSide winner)
        {
            bool won = _rally.HumanPlayer != null && winner == _rally.HumanPlayer.Team;
            string unlock = _progression.ApplyMatchResult(won);

            if (GameSessionState.CurrentMode == GameMode.Story && won)
            {
                if (_saveService.Current.storyChapter < StoryCampaignCatalog.All.Count - 1)
                {
                    _saveService.Current.storyChapter++;
                }
            }
            else if (GameSessionState.CurrentMode == GameMode.Career)
            {
                _career.AdvanceWeek();
                if (won)
                {
                    _saveService.Current.career.trainingPoints += 2;
                }
            }
            else if (GameSessionState.CurrentMode == GameMode.Tournament && won)
            {
                GameSessionState.TournamentRound++;
            }

            _saveService.Save();

            _toast = won ? _localization.T("match.win") : _localization.T("match.loss");
            if (!string.IsNullOrEmpty(unlock))
            {
                _toast += $" | Unlocked: {unlock}";
            }

            _toastUntil = Time.unscaledTime + 4f;
        }

        private void SelectMode(GameMode mode)
        {
            GameSessionState.SelectMode(mode);
            _tuning.quickMatch = mode == GameMode.QuickMatch
                || mode == GameMode.Training
                || mode == GameMode.Challenge
                || mode == GameMode.DreamTeam;

            if (mode == GameMode.Tournament)
            {
                _tournamentPairings = TournamentBracket.Generate(TournamentTeams, System.Environment.TickCount);
            }

            _menuOpen = false;
            Time.timeScale = 1f;
            _rally.RestartMatchNow();
        }

        private void OpenMenu()
        {
            _menuOpen = true;
            Time.timeScale = 0f;
        }

        private void CloseMenuWithoutRestart()
        {
            _menuOpen = false;
            Time.timeScale = 1f;
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

            DrawProfilePanel();

            if (_menuOpen)
            {
                DrawModeMenu();
            }

            if (Time.unscaledTime < _toastUntil)
            {
                GUI.Box(new Rect((Screen.width - 400f) * 0.5f, 205f, 400f, 42f), _toast);
            }
        }

        private void DrawProfilePanel()
        {
            SaveGameData save = _saveService.Current;
            StoryChapter chapter = StoryCampaignCatalog.Get(save.storyChapter);
            float width = 330f;
            float x = Screen.width - width - 16f;
            GUI.Box(new Rect(x, 16f, width, 150f), _localization.T("shell.title"));
            GUI.Label(new Rect(x + 16f, 44f, width - 32f, 22f), $"{_localization.T("shell.level")}: {save.playerLevel} | {_localization.T("shell.coins")}: {save.coins}");
            GUI.Label(new Rect(x + 16f, 66f, width - 32f, 22f), $"{_localization.T("shell.wins")}: {save.matchesWon}/{save.matchesPlayed}");
            GUI.Label(new Rect(x + 16f, 88f, width - 32f, 22f), $"Mode: {GameSessionState.CurrentMode}");
            GUI.Label(new Rect(x + 16f, 110f, width - 32f, 22f), $"Story: {chapter.Index + 1}. {chapter.Title}");

            if (GUI.Button(new Rect(x + 16f, 132f, 118f, 25f), $"Lang: {save.language.ToUpperInvariant()}"))
            {
                ToggleLanguage();
            }
        }

        private void DrawModeMenu()
        {
            float width = 520f;
            float height = 430f;
            float x = (Screen.width - width) * 0.5f;
            float y = (Screen.height - height) * 0.5f;
            GUI.Box(new Rect(x, y, width, height), "SELECT MODE");

            float buttonWidth = 220f;
            float buttonHeight = 44f;
            float left = x + 28f;
            float right = x + width - 28f - buttonWidth;
            float top = y + 54f;
            float gap = 54f;

            if (GUI.Button(new Rect(left, top, buttonWidth, buttonHeight), "QUICK MATCH")) SelectMode(GameMode.QuickMatch);
            if (GUI.Button(new Rect(right, top, buttonWidth, buttonHeight), "STORY")) SelectMode(GameMode.Story);
            if (GUI.Button(new Rect(left, top + gap, buttonWidth, buttonHeight), "CAREER")) SelectMode(GameMode.Career);
            if (GUI.Button(new Rect(right, top + gap, buttonWidth, buttonHeight), "TOURNAMENT")) SelectMode(GameMode.Tournament);
            if (GUI.Button(new Rect(left, top + gap * 2f, buttonWidth, buttonHeight), "TRAINING")) SelectMode(GameMode.Training);
            if (GUI.Button(new Rect(right, top + gap * 2f, buttonWidth, buttonHeight), "CHALLENGE")) SelectMode(GameMode.Challenge);
            if (GUI.Button(new Rect(left, top + gap * 3f, buttonWidth * 2f + 24f, buttonHeight), "DREAM TEAM")) SelectMode(GameMode.DreamTeam);

            StoryChapter chapter = StoryCampaignCatalog.Get(_saveService.Current.storyChapter);
            GUI.Label(new Rect(x + 28f, y + 282f, width - 56f, 24f), $"Story next: {chapter.Title} vs {chapter.Opponent}");
            GUI.Label(new Rect(x + 28f, y + 306f, width - 56f, 42f), $"Objective: {chapter.Objective}");
            GUI.Label(new Rect(x + 28f, y + 348f, width - 56f, 24f), $"Career: Season {_saveService.Current.career.season}, Week {_saveService.Current.career.week}, TP {_saveService.Current.career.trainingPoints}");

            if (_tournamentPairings != null && _tournamentPairings.Count > 0)
            {
                TournamentPairing pair = _tournamentPairings[0];
                GUI.Label(new Rect(x + 28f, y + 372f, width - 56f, 24f), $"Tournament bracket seed: {pair.Left} vs {pair.Right} | Round {GameSessionState.TournamentRound + 1}");
            }

            GUI.Label(new Rect(x + 28f, y + 398f, width - 56f, 22f), "M / Esc: open or close this menu");
        }
    }
}
