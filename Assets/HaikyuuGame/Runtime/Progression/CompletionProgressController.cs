using HaikyuuGame.Core;
using HaikyuuGame.Gameplay.Match;
using HaikyuuGame.Meta;
using HaikyuuGame.Story;
using UnityEngine;

namespace HaikyuuGame.Progression
{
    public sealed class CompletionProgressController : MonoBehaviour
    {
        private RallyController _rally;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Ensure()
        {
            if (FindFirstObjectByType<CompletionProgressController>() != null)
            {
                return;
            }

            new GameObject("CompletionProgressController").AddComponent<CompletionProgressController>();
        }

        private void Update()
        {
            if (_rally != null)
            {
                return;
            }

            _rally = FindFirstObjectByType<RallyController>();
            if (_rally != null)
            {
                _rally.MatchCompleted += OnMatchCompleted;
            }
        }

        private void OnDestroy()
        {
            if (_rally != null)
            {
                _rally.MatchCompleted -= OnMatchCompleted;
            }
        }

        private void OnMatchCompleted(TeamSide winner)
        {
            if (_rally.HumanPlayer == null || winner != _rally.HumanPlayer.Team)
            {
                return;
            }

            GameShellController shell = FindFirstObjectByType<GameShellController>();
            if (shell == null || shell.Save == null)
            {
                return;
            }

            if (GameSessionState.CurrentMode == GameMode.Story
                && shell.Save.storyChapter >= StoryCampaignCatalog.All.Count - 1
                && !shell.Save.storyCompleted)
            {
                shell.Save.storyCompleted = true;
                shell.AwardCoins(500, "Story complete!");
            }
            else if (GameSessionState.CurrentMode == GameMode.Tournament
                && GameSessionState.TournamentRound > 4)
            {
                GameSessionState.TournamentRound = 0;
                shell.Save.tournamentsWon++;
                shell.AwardCoins(300, "Tournament champion!");
                GameSessionState.SelectMode(GameMode.Tournament);
            }
        }
    }
}
