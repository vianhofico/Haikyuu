using HaikyuuGame.Core;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Ball
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class VolleyballBall : MonoBehaviour
    {
        private Rigidbody _body;

        public TeamSide LastTouchTeam { get; private set; } = TeamSide.None;
        public Rigidbody Body => _body;

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _body.mass = 0.27f;
            _body.useGravity = true;
            _body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _body.interpolation = RigidbodyInterpolation.Interpolate;
            _body.maxAngularVelocity = 35f;
        }

        public void Contact(TeamSide team, Vector3 velocity, Vector3 angularVelocity)
        {
            LastTouchTeam = team;
            _body.linearVelocity = velocity;
            _body.angularVelocity = angularVelocity;
        }

        public void ResetBall(Vector3 position)
        {
            transform.position = position;
            transform.rotation = Quaternion.identity;
            LastTouchTeam = TeamSide.None;
            _body.linearVelocity = Vector3.zero;
            _body.angularVelocity = Vector3.zero;
            _body.Sleep();
        }

        public void WakeAndServe(TeamSide team, Vector3 velocity)
        {
            _body.WakeUp();
            Contact(team, velocity, new Vector3(0f, 0f, -8f * Mathf.Sign(velocity.x)));
        }
    }
}
