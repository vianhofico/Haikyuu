using HaikyuuGame.Gameplay.Match;
using HaikyuuGame.Gameplay.Teams;
using HaikyuuGame.Persistence;
using UnityEngine;

namespace HaikyuuGame.Meta
{
    public sealed class ModeMatchupDirector : MonoBehaviour
    {
        private static readonly string[] TournamentOpponents =
        {
            "date_tech", "aoba_johsai", "shiratorizawa", "inarizaki", "all_star"
        };

        private int _lastRevision = -1;
        private int _lastTournamentRound = -1;
        private RallyController _rally;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Ensure()
        {
            if (FindFirstObjectByType<ModeMatchupDirector>() != null) return;
            new GameObject("ModeMatchupDirector").AddComponent<ModeMatchupDirector>();
        }

        private void Update()
        {
            if (MatchRosterController.Current == null) return;
            if (_rally == null) _rally = FindFirstObjectByType<RallyController>();
            if (_rally == null) return;

            bool revisionChanged = _lastRevision != GameSessionState.SessionRevision;
            bool tournamentAdvanced = GameSessionState.CurrentMode == GameMode.Tournament && _lastTournamentRound != GameSessionState.TournamentRound;
            if (!revisionChanged && !tournamentAdvanced) return;

            _lastRevision = GameSessionState.SessionRevision;
            _lastTournamentRound = GameSessionState.TournamentRound;
            ApplyCurrentMode();
        }

        private void ApplyCurrentMode()
        {
            string left = "karasuno";
            string right;

            switch (GameSessionState.CurrentMode)
            {
                case GameMode.Story:
                    SaveGameData storySave = new SaveGameService().Load();
                    right = TeamPresetCatalog.StoryOpponentForChapter(storySave.storyChapter);
                    break;
                case GameMode.Career:
                    SaveGameData careerSave = new SaveGameService().Load();
                    int careerStep = (careerSave.career.season + careerSave.career.week) % 4;
                    right = careerStep == 0 ? "nekoma" : careerStep == 1 ? "aoba_johsai" : careerStep == 2 ? "fukurodani" : "inarizaki";
                    break;
                case GameMode.Tournament:
                    right = TournamentOpponents[Mathf.Min(GameSessionState.TournamentRound, TournamentOpponents.Length - 1)];
                    break;
                case GameMode.Training:
                    right = "training";
                    break;
                case GameMode.Challenge:
                    right = "shiratorizawa";
                    break;
                case GameMode.DreamTeam:
                    left = "dream_team";
                    right = "all_star";
                    break;
                default:
                    right = "inarizaki";
                    break;
            }

            MatchRosterController.Current.ApplyMatchup(left, right);
            _rally.RestartMatchNow();
        }
    }
}
