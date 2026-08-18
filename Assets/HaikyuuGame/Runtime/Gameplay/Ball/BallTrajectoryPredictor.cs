using UnityEngine;

namespace HaikyuuGame.Gameplay.Ball
{
    public static class BallTrajectoryPredictor
    {
        public static Vector3 EstimateLandingPoint(VolleyballBall ball, float targetY = 0.2f)
        {
            Vector3 position = ball.transform.position;
            Vector3 velocity = ball.Body.linearVelocity;
            float gravity = Physics.gravity.y;

            float a = 0.5f * gravity;
            float b = velocity.y;
            float c = position.y - targetY;
            float discriminant = (b * b) - (4f * a * c);

            if (discriminant < 0f || Mathf.Abs(a) < 0.0001f)
            {
                return position;
            }

            float root = Mathf.Sqrt(discriminant);
            float t1 = (-b + root) / (2f * a);
            float t2 = (-b - root) / (2f * a);
            float time = Mathf.Max(t1, t2);

            if (time < 0f)
            {
                return position;
            }

            return new Vector3(
                position.x + (velocity.x * time),
                targetY,
                position.z + (velocity.z * time));
        }

        public static Vector3 SolveBallisticVelocity(Vector3 origin, Vector3 target, float flightTime)
        {
            float safeTime = Mathf.Max(0.15f, flightTime);
            Vector3 displacement = target - origin;
            Vector3 horizontal = new Vector3(displacement.x, 0f, displacement.z) / safeTime;
            float vertical = (displacement.y - (0.5f * Physics.gravity.y * safeTime * safeTime)) / safeTime;
            return new Vector3(horizontal.x, vertical, horizontal.z);
        }
    }
}
