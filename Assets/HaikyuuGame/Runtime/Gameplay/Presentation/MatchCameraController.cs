using HaikyuuGame.Gameplay.Ball;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Presentation
{
    public sealed class MatchCameraController : MonoBehaviour
    {
        private Camera _camera;
        private VolleyballBall _ball;
        private Vector3 _basePosition;
        private Quaternion _baseRotation;
        private float _shake;

        public void Initialize(Camera camera, VolleyballBall ball)
        {
            _camera = camera;
            _ball = ball;
            _basePosition = camera.transform.position;
            _baseRotation = camera.transform.rotation;
        }

        public void Punch(float strength)
        {
            if (!RuntimePresentationSettings.ScreenShake)
            {
                return;
            }

            _shake = Mathf.Max(_shake, strength);
        }

        private void LateUpdate()
        {
            if (_camera == null || _ball == null)
            {
                return;
            }

            Vector3 followOffset = new Vector3(
                Mathf.Clamp(_ball.transform.position.x * 0.075f, -0.65f, 0.65f),
                Mathf.Clamp((_ball.transform.position.y - 2f) * 0.09f, -0.2f, 0.5f),
                0f);

            if (!RuntimePresentationSettings.ScreenShake)
            {
                _shake = 0f;
            }

            Vector3 shakeOffset = RuntimePresentationSettings.ScreenShake
                ? UnityEngine.Random.insideUnitSphere * _shake
                : Vector3.zero;
            Vector3 target = _basePosition + followOffset + shakeOffset;
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, target, Time.unscaledDeltaTime * 10f);
            _camera.transform.rotation = _baseRotation;
            _shake = Mathf.MoveTowards(_shake, 0f, Time.unscaledDeltaTime * 2.8f);
        }
    }
}
