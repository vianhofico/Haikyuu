using System.Collections.Generic;
using HaikyuuGame.Career;
using HaikyuuGame.Core;
using HaikyuuGame.Gameplay.Character;
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
            bool tournamentAdvanced = GameSessionState.CurrentMode == GameMode.Tournament
                && _lastTournamentRound != GameSessionState.TournamentRound;
            if (!revisionChanged && !tournamentAdvanced) return;

            _lastRevision = GameSessionState.SessionRevision;
            _lastTournamentRound = GameSessionState.TournamentRound;
            ApplyCurrentMode();
        }

        private void ApplyCurrentMode()
        {
            SaveGameData save = new SaveGameService().Load();
            string left = "karasuno";
            string right;

            switch (GameSessionState.CurrentMode)
            {
                case GameMode.Story:
                    right = TeamPresetCatalog.StoryOpponentForChapter(save.storyChapter);
                    break;
                case GameMode.Career:
                    int careerStep = (save.career.season + save.career.week) % 4;
                    right = careerStep == 0
                        ? "nekoma"
                        : careerStep == 1
                            ? "aoba_johsai"
                            : careerStep == 2 ? "fukurodani" : "inarizaki";
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
                case GameMode.Arcade3v3:
                    right = "inarizaki";
                    break;
                default:
                    right = "inarizaki";
                    break;
            }

            MatchRosterController.Current.ApplyMatchup(left, right);

            if (GameSessionState.CurrentMode == GameMode.Career)
            {
                MatchRosterController.Current.AssignProfileToSlot(
                    TeamSide.Left,
                    2,
                    CareerProfileFactory.Create(save.career));
            }
            else if (GameSessionState.CurrentMode == GameMode.DreamTeam)
            {
                ApplyDreamTeam(save);
            }

            _rally.RestartMatchNow();
        }

        private static void ApplyDreamTeam(SaveGameData save)
        {
            DreamTeamService service = new DreamTeamService(save);
            List<RuntimeCharacterProfile> six = new List<RuntimeCharacterProfile>(6);
            for (int slot = 0; slot < 6; slot++)
            {
                six.Add(service.GetProfile(slot));
            }

            MatchRosterController.Current.ApplyCustomLineup(
                TeamSide.Left,
                "Dream Team",
                six,
                service.GetProfile(6));
        }
    }
}
