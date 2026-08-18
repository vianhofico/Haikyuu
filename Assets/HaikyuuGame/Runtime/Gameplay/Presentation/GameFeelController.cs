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
        private float _ownedScale = 1f;
        private float _restoreScale = 1f;
        private bool _ownsTimeScale;

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

            RestoreTimeScaleIfOwned();
        }

        private void OnBallContact(BallContact contact)
        {
            float timingBoost = contact.Timing == ContactTimingGrade.Perfect ? 1.6f : 1f;
            switch (contact.Type)
            {
                case BallContactType.Attack:
                    _camera?.Punch(0.12f * timingBoost);
                    TriggerSlowMotion(
                        contact.Timing == ContactTimingGrade.Perfect ? 0.055f : 0.035f,
                        contact.Timing == ContactTimingGrade.Perfect ? 0.56f : 0.72f);
                    break;
                case BallContactType.Block:
                    _camera?.Punch(0.16f * timingBoost);
                    TriggerSlowMotion(
                        contact.Timing == ContactTimingGrade.Perfect ? 0.065f : 0.045f,
                        contact.Timing == ContactTimingGrade.Perfect ? 0.52f : 0.62f);
                    break;
                case BallContactType.Serve:
                    _camera?.Punch(0.035f * timingBoost);
                    break;
            }
        }

        private void TriggerSlowMotion(float duration, float scale)
        {
            if (_slowMotionRoutine != null)
            {
                StopCoroutine(_slowMotionRoutine);
                _slowMotionRoutine = null;
                RestoreTimeScaleIfOwned();
            }

            // Do not start hit-stop while another system (menu/pause) already
            // owns a zero timeScale.
            if (Time.timeScale <= 0.0001f)
            {
                return;
            }

            _slowMotionRoutine = StartCoroutine(BriefSlowMotion(duration, scale));
        }

        private IEnumerator BriefSlowMotion(float duration, float scale)
        {
            _restoreScale = Time.timeScale;
            _ownedScale = scale;
            _ownsTimeScale = true;
            Time.timeScale = scale;

            yield return new WaitForSecondsRealtime(duration);

            RestoreTimeScaleIfOwned();
            _slowMotionRoutine = null;
        }

        private void RestoreTimeScaleIfOwned()
        {
            if (!_ownsTimeScale)
            {
                return;
            }

            // If another system changed timeScale while hit-stop was active
            // (notably the pause menu setting it to 0), leave that value alone.
            if (Mathf.Abs(Time.timeScale - _ownedScale) <= 0.0001f)
            {
                Time.timeScale = _restoreScale;
            }

            _ownsTimeScale = false;
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
