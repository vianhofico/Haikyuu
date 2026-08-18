using HaikyuuGame.Core;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Match
{
    public sealed class TeamMomentum
    {
        public float Left { get; private set; }
        public float Right { get; private set; }

        public float Get(TeamSide team)
        {
            if (team == TeamSide.Left) return Left;
            if (team == TeamSide.Right) return Right;
            return 0f;
        }

        public void Add(TeamSide team, float amount)
        {
            if (team == TeamSide.Left)
            {
                Left = Mathf.Clamp(Left + amount, -100f, 100f);
            }
            else if (team == TeamSide.Right)
            {
                Right = Mathf.Clamp(Right + amount, -100f, 100f);
            }
        }

        public void PointTo(TeamSide scorer)
        {
            TeamSide loser = RallyController.Opposite(scorer);
            Add(scorer, 10f);
            Add(loser, -7f);
        }

        public void ResetSet()
        {
            Left *= 0.35f;
            Right *= 0.35f;
        }

        public void ResetMatch()
        {
            Left = 0f;
            Right = 0f;
        }
    }
}
