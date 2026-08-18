using HaikyuuGame.Core;
using HaikyuuGame.Gameplay.Ball;
using HaikyuuGame.Gameplay.Player;

namespace HaikyuuGame.Gameplay.Match
{
    public readonly struct PossessionUpdate
    {
        public PossessionUpdate(bool fault, TeamSide faultingTeam, int touches, string reason)
        {
            Fault = fault;
            FaultingTeam = faultingTeam;
            Touches = touches;
            Reason = reason;
        }

        public bool Fault { get; }
        public TeamSide FaultingTeam { get; }
        public int Touches { get; }
        public string Reason { get; }
    }

    public sealed class TeamPossession
    {
        private PlayerActor _lastCountedPlayer;

        public TeamSide Team { get; private set; } = TeamSide.None;
        public int CountedTouches { get; private set; }
        public BallContactType LastContactType { get; private set; } = BallContactType.None;

        public PossessionUpdate Register(BallContact contact)
        {
            BallContactType previousType = LastContactType;
            LastContactType = contact.Type;

            if (contact.Type == BallContactType.Serve)
            {
                Team = contact.Team;
                CountedTouches = 0;
                _lastCountedPlayer = null;
                return Ok();
            }

            if (contact.Team != Team)
            {
                Team = contact.Team;
                CountedTouches = 0;
                _lastCountedPlayer = null;
            }

            if (!contact.CountsAsTeamTouch)
            {
                // A block does not count as a team contact and the blocker may
                // legally make the next contact.
                if (contact.Type == BallContactType.Block)
                {
                    _lastCountedPlayer = null;
                }

                return Ok();
            }

            if (contact.Player != null
                && _lastCountedPlayer == contact.Player
                && previousType != BallContactType.Block)
            {
                return new PossessionUpdate(true, contact.Team, CountedTouches, "Double contact");
            }

            CountedTouches++;
            _lastCountedPlayer = contact.Player;

            if (CountedTouches > 3)
            {
                return new PossessionUpdate(true, contact.Team, CountedTouches, "Four contacts");
            }

            return Ok();
        }

        public void Reset()
        {
            Team = TeamSide.None;
            CountedTouches = 0;
            LastContactType = BallContactType.None;
            _lastCountedPlayer = null;
        }

        private PossessionUpdate Ok()
        {
            return new PossessionUpdate(false, TeamSide.None, CountedTouches, string.Empty);
        }
    }
}
