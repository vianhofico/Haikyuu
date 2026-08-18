using HaikyuuGame.Core;
using HaikyuuGame.Gameplay.Player;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Ball
{
    public enum BallContactType
    {
        None = 0,
        Serve = 1,
        Receive = 2,
        Set = 3,
        Attack = 4,
        Block = 5,
        Dig = 6,
        FreeBall = 7
    }

    public readonly struct BallContact
    {
        public BallContact(
            TeamSide team,
            PlayerActor player,
            BallContactType type,
            ContactTimingGrade timing,
            Vector3 velocity,
            float timestamp)
        {
            Team = team;
            Player = player;
            Type = type;
            Timing = timing;
            Velocity = velocity;
            Timestamp = timestamp;
        }

        public TeamSide Team { get; }
        public PlayerActor Player { get; }
        public BallContactType Type { get; }
        public ContactTimingGrade Timing { get; }
        public Vector3 Velocity { get; }
        public float Timestamp { get; }
        public bool CountsAsTeamTouch => Type != BallContactType.Serve && Type != BallContactType.Block;
    }
}
