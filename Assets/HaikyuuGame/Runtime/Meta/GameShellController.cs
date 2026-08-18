using System.Collections.Generic;
using HaikyuuGame.Career;
using HaikyuuGame.Core;
using HaikyuuGame.Gameplay;
using HaikyuuGame.Gameplay.Character;
using HaikyuuGame.Gameplay.Match;
using HaikyuuGame.Gameplay.Teams;
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

        private static readonly string[] CareerRoles =
        {
            "OutsideHitter", "MiddleBlocker", "Setter", "Opposite"
        };

        private RallyController _rally;
        private VolleyballTuning _tuning;
        private SaveGameService _saveService;
        private ProgressionService _progression;
        private CareerService _career;
        private DreamTeamService _dreamTeam;
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
            _dreamTeam = new DreamTeamService(save);
            _rally.MatchCompleted += OnMatchCompleted;
            OpenMenu();
        }

        private void OnDestroy()
        {
            if (_rally != null) _rally.MatchCompleted -= OnMatchCompleted;
            Time.timeScale = 1f;
            _saveService?.Save();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.L)) ToggleLanguage();
            if (UnityEngine.Input.GetKeyDown(KeyCode.M) || UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                if (_menuOpen) CloseMenuWithoutRestart(); else OpenMenu();
            }

            if (_menuOpen && GameSessionState.CurrentMode == GameMode.Career)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) Train(CareerStat.Attack);
                if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) Train(CareerStat.Serve);
                if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3)) Train(CareerStat.Set);
                if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4)) Train(CareerStat.Receive);
                if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5)) Train(CareerStat.Block);
                if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha6)) Train(CareerStat.Jump);
                if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha7)) Train(CareerStat.Speed);
                if (UnityEngine.Input.GetKeyDown(KeyCode.Tab)) CycleCareerRole();
            }
        }

        private void OnMatchCompleted(TeamSide winner)
        {
            bool won = _rally.HumanPlayer != null && winner == _rally.HumanPlayer.Team;
            string unlock = _progression.ApplyMatchResult(won);
            bool refreshMatchup = false;

            if (GameSessionState.CurrentMode == GameMode.Story && won)
            {
                if (_saveService.Current.storyChapter < StoryCampaignCatalog.All.Count - 1)
                {
                    _saveService.Current.storyChapter++;
                    refreshMatchup = true;
                }
            }
            else if (GameSessionState.CurrentMode == GameMode.Career)
            {
                _career.AdvanceWeek();
                if (won) _saveService.Current.career.trainingPoints += 2;
                refreshMatchup = true;
            }
            else if (GameSessionState.CurrentMode == GameMode.Tournament && won)
            {
                GameSessionState.TournamentRound++;
            }

            _saveService.Save();
            _toast = won ? _localization.T("match.win") : _localization.T("match.loss");
            if (!string.IsNullOrEmpty(unlock)) _toast += $" | Unlocked: {unlock}";
            _toastUntil = Time.unscaledTime + 4f;

            if (refreshMatchup) GameSessionState.SelectMode(GameSessionState.CurrentMode);
        }

        private void SelectMode(GameMode mode)
        {
            GameSessionState.SelectMode(mode);
            bool arcadeThree = mode == GameMode.Arcade3v3;
            _tuning.quickMatch = mode == GameMode.QuickMatch
                || mode == GameMode.Training
                || mode == GameMode.Challenge
                || mode == GameMode.DreamTeam
                || arcadeThree;

            if (mode == GameMode.Tournament)
            {
                _tournamentPairings = TournamentBracket.Generate(TournamentTeams, System.Environment.TickCount);
            }

            _rally.SetPlayersPerSide(arcadeThree ? 3 : 6);
            _menuOpen = false;
            Time.timeScale = 1f;
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

        private void Train(CareerStat stat)
        {
            if (!_career.Train(stat))
            {
                ShowToast("Not enough training points");
                return;
            }

            _saveService.Save();
            GameSessionState.SelectMode(GameMode.Career);
            ShowToast($"{stat} +1");
        }

        private void CycleCareerRole()
        {
            CareerSaveData career = _saveService.Current.career;
            int index = 0;
            for (int i = 0; i < CareerRoles.Length; i++)
            {
                if (CareerRoles[i] == career.role)
                {
                    index = i;
                    break;
                }
            }

            career.role = CareerRoles[(index + 1) % CareerRoles.Length];
            _saveService.Save();
            GameSessionState.SelectMode(GameMode.Career);
            ShowToast($"Career role: {career.role}");
        }

        private void CycleDreamSlot(int slot, int direction)
        {
            RuntimeCharacterProfile selected = _dreamTeam.Cycle(slot, direction);
            _saveService.Save();
            GameSessionState.SelectMode(GameMode.DreamTeam);
            if (selected != null) ShowToast($"Slot {slot + 1}: {selected.DisplayName}");
        }

        private void ShowToast(string message)
        {
            _toast = message;
            _toastUntil = Time.unscaledTime + 3f;
        }

        private void OnGUI()
        {
            if (_saveService == null || _saveService.Current == null) return;
            DrawProfilePanel();
            if (_menuOpen) DrawModeMenu();
            if (Time.unscaledTime < _toastUntil)
            {
                GUI.Box(new Rect((Screen.width - 420f) * 0.5f, 18f, 420f, 42f), _toast);
            }
        }

        private void DrawProfilePanel()
        {
            SaveGameData save = _saveService.Current;
            StoryChapter chapter = StoryCampaignCatalog.Get(save.storyChapter);
            float width = 350f;
            float x = Screen.width - width - 16f;
            GUI.Box(new Rect(x, 16f, width, 174f), _localization.T("shell.title"));
            GUI.Label(new Rect(x + 16f, 44f, width - 32f, 22f), $"{_localization.T("shell.level")}: {save.playerLevel} | {_localization.T("shell.coins")}: {save.coins}");
            GUI.Label(new Rect(x + 16f, 66f, width - 32f, 22f), $"{_localization.T("shell.wins")}: {save.matchesWon}/{save.matchesPlayed}");
            GUI.Label(new Rect(x + 16f, 88f, width - 32f, 22f), $"Mode: {GameSessionState.CurrentMode}");
            if (MatchRosterController.Current != null)
            {
                GUI.Label(new Rect(x + 16f, 110f, width - 32f, 22f), $"{MatchRosterController.Current.LeftTeamName} vs {MatchRosterController.Current.RightTeamName}");
            }
            GUI.Label(new Rect(x + 16f, 132f, width - 32f, 22f), $"Story: {chapter.Index + 1}. {chapter.Title}");
            if (GUI.Button(new Rect(x + 16f, 154f, 118f, 25f), $"Lang: {save.language.ToUpperInvariant()}")) ToggleLanguage();
        }

        private void DrawModeMenu()
        {
            bool extended = GameSessionState.CurrentMode == GameMode.Career || GameSessionState.CurrentMode == GameMode.DreamTeam;
            float width = 600f;
            float height = extended ? 660f : 500f;
            float x = (Screen.width - width) * 0.5f;
            float y = Mathf.Max(8f, (Screen.height - height) * 0.5f);
            GUI.Box(new Rect(x, y, width, height), "SELECT MODE");

            float buttonWidth = 250f;
            float buttonHeight = 40f;
            float left = x + 35f;
            float right = x + width - 35f - buttonWidth;
            float top = y + 48f;
            float gap = 48f;

            if (GUI.Button(new Rect(left, top, buttonWidth, buttonHeight), "QUICK MATCH 6v6")) SelectMode(GameMode.QuickMatch);
            if (GUI.Button(new Rect(right, top, buttonWidth, buttonHeight), "ARCADE 3v3")) SelectMode(GameMode.Arcade3v3);
            if (GUI.Button(new Rect(left, top + gap, buttonWidth, buttonHeight), "STORY")) SelectMode(GameMode.Story);
            if (GUI.Button(new Rect(right, top + gap, buttonWidth, buttonHeight), "CAREER")) SelectMode(GameMode.Career);
            if (GUI.Button(new Rect(left, top + gap * 2f, buttonWidth, buttonHeight), "TOURNAMENT")) SelectMode(GameMode.Tournament);
            if (GUI.Button(new Rect(right, top + gap * 2f, buttonWidth, buttonHeight), "TRAINING")) SelectMode(GameMode.Training);
            if (GUI.Button(new Rect(left, top + gap * 3f, buttonWidth, buttonHeight), "CHALLENGE")) SelectMode(GameMode.Challenge);
            if (GUI.Button(new Rect(right, top + gap * 3f, buttonWidth, buttonHeight), "DREAM TEAM")) SelectMode(GameMode.DreamTeam);

            StoryChapter chapter = StoryCampaignCatalog.Get(_saveService.Current.storyChapter);
            float infoY = top + gap * 4f + 8f;
            GUI.Label(new Rect(x + 35f, infoY, width - 70f, 24f), $"Story next: {chapter.Title} vs {chapter.Opponent}");
            GUI.Label(new Rect(x + 35f, infoY + 23f, width - 70f, 38f), $"Objective: {chapter.Objective}");
            GUI.Label(new Rect(x + 35f, infoY + 60f, width - 70f, 24f), $"Career: Season {_saveService.Current.career.season}, Week {_saveService.Current.career.week}, TP {_saveService.Current.career.trainingPoints}");

            if (_tournamentPairings != null && _tournamentPairings.Count > 0)
            {
                TournamentPairing pair = _tournamentPairings[0];
                GUI.Label(new Rect(x + 35f, infoY + 84f, width - 70f, 24f), $"Tournament seed: {pair.Left} vs {pair.Right} | Round {GameSessionState.TournamentRound + 1}");
            }

            if (GameSessionState.CurrentMode == GameMode.Career)
            {
                DrawCareerEditor(x + 35f, infoY + 116f, width - 70f);
            }
            else if (GameSessionState.CurrentMode == GameMode.DreamTeam)
            {
                DrawDreamTeamEditor(x + 35f, infoY + 110f, width - 70f);
            }

            GUI.Label(new Rect(x + 35f, y + height - 28f, width - 70f, 22f), "M / Esc: resume | L: VI/EN");
        }

        private void DrawCareerEditor(float x, float y, float width)
        {
            CareerSaveData c = _saveService.Current.career;
            GUI.Box(new Rect(x, y, width, 160f), "CAREER PLAYER");
            GUI.Label(new Rect(x + 12f, y + 25f, width - 24f, 22f), $"{c.playerName} | {c.role} | Training Points: {c.trainingPoints}");
            if (GUI.Button(new Rect(x + width - 126f, y + 22f, 112f, 25f), "Change Role")) CycleCareerRole();

            CareerStat[] stats =
            {
                CareerStat.Attack, CareerStat.Serve, CareerStat.Set, CareerStat.Receive,
                CareerStat.Block, CareerStat.Jump, CareerStat.Speed
            };

            int[] values = { c.attack, c.serve, c.set, c.receive, c.block, c.jump, c.speed };
            for (int i = 0; i < stats.Length; i++)
            {
                int column = i % 4;
                int row = i / 4;
                float bx = x + 12f + column * 128f;
                float by = y + 56f + row * 45f;
                if (GUI.Button(new Rect(bx, by, 118f, 36f), $"{stats[i]} {values[i]}  +")) Train(stats[i]);
            }
        }

        private void DrawDreamTeamEditor(float x, float y, float width)
        {
            GUI.Box(new Rect(x, y, width, 210f), "DREAM TEAM BUILDER");
            for (int slot = 0; slot < 7; slot++)
            {
                RuntimeCharacterProfile profile = _dreamTeam.GetProfile(slot);
                float rowY = y + 24f + slot * 25f;
                string role = DreamTeamService.RoleForSlot(slot).ToString();
                string name = profile != null ? profile.DisplayName : "-";
                GUI.Label(new Rect(x + 12f, rowY, 330f, 22f), $"{slot + 1}. {role}: {name}");
                if (GUI.Button(new Rect(x + width - 88f, rowY, 32f, 22f), "<")) CycleDreamSlot(slot, -1);
                if (GUI.Button(new Rect(x + width - 48f, rowY, 32f, 22f), ">")) CycleDreamSlot(slot, 1);
            }
        }
    }
}
