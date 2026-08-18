using System.Collections;
using HaikyuuGame.Gameplay.Ball;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Presentation
{
    public sealed class GameFeelController : MonoBehaviour
    {
        private VolleyballBall _ball;
        private MatchCameraController _camera;
        private Coroutine _slowMotionRoutine;

        public void Initialize(VolleyballBall ball, MatchCameraController cameraController)
        {
            _ball = ball;
            _camera = cameraController;
            ConfigureBallTrail(ball);
            _ball.Contacted += OnBallContact;
        }

        private void OnDestroy()
        {
            if (_ball != null)
            {
                _ball.Contacted -= OnBallContact;
            }

            Time.timeScale = 1f;
        }

        private void OnBallContact(BallContact contact)
        {
            switch (contact.Type)
            {
                case BallContactType.Attack:
                    _camera?.Punch(0.12f);
                    TriggerSlowMotion(0.035f, 0.72f);
                    break;
                case BallContactType.Block:
                    _camera?.Punch(0.16f);
                    TriggerSlowMotion(0.045f, 0.62f);
                    break;
                case BallContactType.Serve:
                    _camera?.Punch(0.035f);
                    break;
            }
        }

        private void TriggerSlowMotion(float duration, float scale)
        {
            if (_slowMotionRoutine != null)
            {
                StopCoroutine(_slowMotionRoutine);
            }

            _slowMotionRoutine = StartCoroutine(BriefSlowMotion(duration, scale));
        }

        private IEnumerator BriefSlowMotion(float duration, float scale)
        {
            Time.timeScale = scale;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1f;
            _slowMotionRoutine = null;
        }

        private static void ConfigureBallTrail(VolleyballBall ball)
        {
            TrailRenderer trail = ball.GetComponent<TrailRenderer>();
            if (trail == null)
            {
                trail = ball.gameObject.AddComponent<TrailRenderer>();
            }

            trail.time = 0.11f;
            trail.startWidth = 0.095f;
            trail.endWidth = 0f;
            trail.minVertexDistance = 0.04f;
            trail.startColor = new Color(1f, 0.92f, 0.45f, 0.95f);
            trail.endColor = new Color(1f, 0.6f, 0.1f, 0f);

            Shader shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                trail.material = new Material(shader);
            }
        }
    }
}
