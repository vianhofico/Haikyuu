using UnityEngine;

namespace HaikyuuGame.Gameplay
{
    [System.Serializable]
    public sealed class VolleyballTuning
    {
        [Header("Court")]
        public float halfCourtLength = 9f;
        public float halfCourtWidth = 4.5f;
        public float netHeight = 2.43f;

        [Header("Player")]
        public float moveSpeed = 5.8f;
        public float jumpVelocity = 7.7f;
        public float actionReach = 2.15f;
        public float actionCooldown = 0.24f;
        public float aiDecisionMin = 0.16f;
        public float aiDecisionMax = 0.34f;

        [Header("Receive / Dig")]
        public float receiveFlightTime = 0.9f;
        public float digFlightTime = 1.0f;
        public float receiveTargetHeight = 1.8f;

        [Header("Set")]
        public float setFlightTime = 0.68f;
        public float setTargetHeight = 3.15f;
        public float quickSetFlightTime = 0.44f;

        [Header("Attack / Block")]
        public float spikeForwardSpeed = 13.8f;
        public float spikeDownSpeed = 5.1f;
        public float blockForwardSpeed = 8.5f;
        public float blockUpSpeed = 2.3f;

        [Header("Match")]
        public bool quickMatch = true;
        public int quickSetTargetScore = 11;
        public int standardSetTargetScore = 25;
        public int decidingSetTargetScore = 15;
        public int setsToWin = 2;
        public float rallyResetDelay = 1.0f;
        public float autoServeDelay = 0.75f;
    }
}
