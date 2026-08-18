using HaikyuuGame.Core;

namespace HaikyuuGame.Gameplay.Match
{
    public readonly struct ScoreUpdate
    {
        public ScoreUpdate(bool setWon, bool matchWon, TeamSide winner)
        {
            SetWon = setWon;
            MatchWon = matchWon;
            Winner = winner;
        }

        public bool SetWon { get; }
        public bool MatchWon { get; }
        public TeamSide Winner { get; }
    }

    public sealed class MatchScore
    {
        public int Left { get; private set; }
        public int Right { get; private set; }
        public int LeftSets { get; private set; }
        public int RightSets { get; private set; }
        public int CurrentSet => LeftSets + RightSets + 1;

        public ScoreUpdate AddPoint(
            TeamSide team,
            bool quickMatch,
            int quickTarget,
            int standardTarget,
            int decidingTarget,
            int setsToWin)
        {
            if (team == TeamSide.Left)
            {
                Left++;
            }
            else if (team == TeamSide.Right)
            {
                Right++;
            }

            int target = quickMatch
                ? quickTarget
                : (LeftSets == setsToWin - 1 && RightSets == setsToWin - 1 ? decidingTarget : standardTarget);

            if (!HasPointWinner(target, out TeamSide setWinner))
            {
                return new ScoreUpdate(false, false, TeamSide.None);
            }

            if (quickMatch)
            {
                return new ScoreUpdate(true, true, setWinner);
            }

            if (setWinner == TeamSide.Left)
            {
                LeftSets++;
            }
            else
            {
                RightSets++;
            }

            bool matchWon = LeftSets >= setsToWin || RightSets >= setsToWin;
            TeamSide matchWinner = matchWon
                ? (LeftSets > RightSets ? TeamSide.Left : TeamSide.Right)
                : TeamSide.None;

            Left = 0;
            Right = 0;
            return new ScoreUpdate(true, matchWon, matchWinner);
        }

        public void ResetMatch()
        {
            Left = 0;
            Right = 0;
            LeftSets = 0;
            RightSets = 0;
        }

        private bool HasPointWinner(int targetScore, out TeamSide winner)
        {
            if (Left >= targetScore && Left - Right >= 2)
            {
                winner = TeamSide.Left;
                return true;
            }

            if (Right >= targetScore && Right - Left >= 2)
            {
                winner = TeamSide.Right;
                return true;
            }

            winner = TeamSide.None;
            return false;
        }
    }
}
