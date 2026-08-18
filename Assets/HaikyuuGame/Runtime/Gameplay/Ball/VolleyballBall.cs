using System;
using HaikyuuGame.Core;
using HaikyuuGame.Gameplay.Player;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Ball
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class VolleyballBall : MonoBehaviour
    {
        private Rigidbody _body;

        public event Action<BallContact> Contacted;

        public TeamSide LastTouchTeam { get; private set; } = TeamSide.None;
        public BallContactType LastContactType { get; private set; } = BallContactType.None;
        public BallContact LastContact { get; private set; }
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

        public void Contact(
            TeamSide team,
            PlayerActor player,
            BallContactType contactType,
            Vector3 velocity,
            Vector3 angularVelocity,
            ContactTimingGrade timing = ContactTimingGrade.Good)
        {
            LastTouchTeam = team;
            LastContactType = contactType;
            _body.WakeUp();
            _body.linearVelocity = velocity;
            _body.angularVelocity = angularVelocity;
            BallContact contact = new BallContact(team, player, contactType, timing, velocity, Time.time);
            LastContact = contact;
            Contacted?.Invoke(contact);
        }

        public void ResetBall(Vector3 position)
        {
            transform.position = position;
            transform.rotation = Quaternion.identity;
            LastTouchTeam = TeamSide.None;
            LastContactType = BallContactType.None;
            LastContact = default;
            _body.linearVelocity = Vector3.zero;
            _body.angularVelocity = Vector3.zero;
            _body.Sleep();
        }

        public void WakeAndServe(
            TeamSide team,
            PlayerActor server,
            Vector3 velocity,
            Vector3 angularVelocity,
            ContactTimingGrade timing)
        {
            Contact(team, server, BallContactType.Serve, velocity, angularVelocity, timing);
        }
    }
}
