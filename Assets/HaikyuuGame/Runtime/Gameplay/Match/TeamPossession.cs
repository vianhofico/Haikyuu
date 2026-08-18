using HaikyuuGame.Core;
using HaikyuuGame.Gameplay.Ball;

namespace HaikyuuGame.Gameplay.Match
{
    public readonly struct PossessionUpdate
    {
        public PossessionUpdate(bool fault, TeamSide faultingTeam, int touches)
        {
            Fault = fault;
            FaultingTeam = faultingTeam;
            Touches = touches;
        }

        public bool Fault { get; }
        public TeamSide FaultingTeam { get; }
        public int Touches { get; }
    }

    public sealed class TeamPossession
    {
        public TeamSide Team { get; private set; } = TeamSide.None;
        public int CountedTouches { get; private set; }
        public BallContactType LastContactType { get; private set; } = BallContactType.None;

        public PossessionUpdate Register(BallContact contact)
        {
            LastContactType = contact.Type;

            if (contact.Type == BallContactType.Serve)
            {
                Team = contact.Team;
                CountedTouches = 0;
                return new PossessionUpdate(false, TeamSide.None, CountedTouches);
            }

            if (contact.Team != Team)
            {
                Team = contact.Team;
                CountedTouches = 0;
            }

            if (contact.CountsAsTeamTouch)
            {
                CountedTouches++;
            }

            if (CountedTouches > 3)
            {
                return new PossessionUpdate(true, contact.Team, CountedTouches);
            }

            return new PossessionUpdate(false, TeamSide.None, CountedTouches);
        }

        public void Reset()
        {
            Team = TeamSide.None;
            CountedTouches = 0;
            LastContactType = BallContactType.None;
        }
    }
}
