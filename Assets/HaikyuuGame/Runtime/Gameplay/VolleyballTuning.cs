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
        public float moveSpeed = 5.6f;
        public float jumpVelocity = 7.4f;
        public float actionReach = 2.05f;
        public float actionCooldown = 0.28f;

        [Header("Contacts")]
        public float receiveForwardSpeed = 3.8f;
        public float receiveUpSpeed = 6.3f;
        public float spikeForwardSpeed = 12.8f;
        public float spikeDownSpeed = 4.5f;

        [Header("Prototype match")]
        public int quickSetTargetScore = 11;
        public float rallyResetDelay = 1.15f;
        public float autoServeDelay = 0.85f;
    }
}
