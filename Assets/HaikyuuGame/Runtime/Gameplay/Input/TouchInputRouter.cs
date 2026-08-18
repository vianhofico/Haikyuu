using HaikyuuGame.Gameplay.Ball;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Input
{
    public sealed class TouchInputRouter : MonoBehaviour
    {
        private const float MoveRadiusPixels = 105f;
        private int _moveFingerId = -1;
        private Vector2 _moveOrigin;
        private bool _jumpPressed;
        private bool _contextPressed;

        public static TouchInputRouter Instance { get; private set; }
        public Vector2 Move { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            for (int i = 0; i < UnityEngine.Input.touchCount; i++)
            {
                Touch touch = UnityEngine.Input.GetTouch(i);

                if (touch.phase == TouchPhase.Began)
                {
                    if (touch.position.x < Screen.width * 0.46f && _moveFingerId < 0)
                    {
                        _moveFingerId = touch.fingerId;
                        _moveOrigin = touch.position;
                        Move = Vector2.zero;
                    }
                    else if (InsideButton(touch.position, 0.84f, 0.18f, 0.095f))
                    {
                        _contextPressed = true;
                    }
                    else if (InsideButton(touch.position, 0.69f, 0.17f, 0.078f))
                    {
                        _jumpPressed = true;
                    }
                }

                if (touch.fingerId == _moveFingerId)
                {
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        Vector2 delta = touch.position - _moveOrigin;
                        Move = Vector2.ClampMagnitude(delta / MoveRadiusPixels, 1f);
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        _moveFingerId = -1;
                        Move = Vector2.zero;
                    }
                }
            }
        }

        public bool ConsumeJump()
        {
            bool value = _jumpPressed;
            _jumpPressed = false;
            return value;
        }

        public bool ConsumeContextAction()
        {
            bool value = _contextPressed;
            _contextPressed = false;
            return value;
        }

        private static bool InsideButton(Vector2 point, float normalizedX, float normalizedY, float normalizedRadius)
        {
            Vector2 center = new Vector2(Screen.width * normalizedX, Screen.height * normalizedY);
            float radius = Mathf.Min(Screen.width, Screen.height) * normalizedRadius;
            return Vector2.Distance(point, center) <= radius;
        }

        private void OnGUI()
        {
            if (!Application.isMobilePlatform)
            {
                return;
            }

            float size = Mathf.Min(Screen.width, Screen.height) * 0.16f;
            float guiYAction = Screen.height - (Screen.height * 0.18f) - (size * 0.5f);
            float guiYJump = Screen.height - (Screen.height * 0.17f) - (size * 0.5f);

            GUI.Box(new Rect(Screen.width * 0.84f - size * 0.5f, guiYAction, size, size), "ACTION");
            GUI.Box(new Rect(Screen.width * 0.69f - size * 0.5f, guiYJump, size * 0.82f, size * 0.82f), "JUMP");
            GUI.Box(new Rect(24f, Screen.height - 190f, 170f, 150f), "MOVE");
        }
    }
}
