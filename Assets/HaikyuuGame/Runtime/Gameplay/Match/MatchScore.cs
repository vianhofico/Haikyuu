using HaikyuuGame.Core;

namespace HaikyuuGame.Gameplay.Match
{
    public sealed class MatchScore
    {
        public int Left { get; private set; }
        public int Right { get; private set; }

        public void AddPoint(TeamSide team)
        {
            if (team == TeamSide.Left)
            {
                Left++;
            }
            else if (team == TeamSide.Right)
            {
                Right++;
            }
        }

        public bool HasWinner(int targetScore, out TeamSide winner)
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

        public void Reset()
        {
            Left = 0;
            Right = 0;
        }
    }
}
