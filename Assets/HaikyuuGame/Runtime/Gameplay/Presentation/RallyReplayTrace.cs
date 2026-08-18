using System.Collections.Generic;
using HaikyuuGame.Core;
using HaikyuuGame.Gameplay.Ball;
using HaikyuuGame.Gameplay.Match;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Presentation
{
    public sealed class RallyReplayTrace : MonoBehaviour
    {
        private readonly List<Vector3> _current = new List<Vector3>(256);
        private readonly List<Vector3> _last = new List<Vector3>(256);
        private VolleyballBall _ball;
        private RallyController _rally;
        private LineRenderer _line;
        private float _nextSampleAt;
        private float _visibleUntil;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Ensure()
        {
            if (FindFirstObjectByType<RallyReplayTrace>() != null)
            {
                return;
            }

            new GameObject("RallyReplayTrace").AddComponent<RallyReplayTrace>();
        }

        private void Update()
        {
            EnsureBound();
            if (_rally == null || _ball == null)
            {
                return;
            }

            if (_rally.RallyActive && Time.unscaledTime >= _nextSampleAt)
            {
                _nextSampleAt = Time.unscaledTime + 0.045f;
                if (_current.Count < 320)
                {
                    _current.Add(_ball.transform.position);
                }
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.P) && _last.Count > 1)
            {
                ShowLastTrace();
            }

            if (_line != null && _line.enabled && Time.unscaledTime >= _visibleUntil)
            {
                _line.enabled = false;
            }
        }

        private void OnDestroy()
        {
            if (_rally != null)
            {
                _rally.PointScored -= OnPointScored;
            }
        }

        private void EnsureBound()
        {
            if (_ball == null)
            {
                _ball = FindFirstObjectByType<VolleyballBall>();
            }

            if (_rally != null)
            {
                return;
            }

            _rally = FindFirstObjectByType<RallyController>();
            if (_rally != null)
            {
                _rally.PointScored += OnPointScored;
            }
        }

        private void OnPointScored(TeamSide scorer)
        {
            _last.Clear();
            _last.AddRange(_current);
            _current.Clear();
        }

        private void ShowLastTrace()
        {
            EnsureLineRenderer();
            _line.positionCount = _last.Count;
            for (int i = 0; i < _last.Count; i++)
            {
                _line.SetPosition(i, _last[i]);
            }

            _line.enabled = true;
            _visibleUntil = Time.unscaledTime + 4f;
        }

        private void EnsureLineRenderer()
        {
            if (_line != null)
            {
                return;
            }

            _line = gameObject.AddComponent<LineRenderer>();
            _line.useWorldSpace = true;
            _line.startWidth = 0.055f;
            _line.endWidth = 0.018f;
            _line.startColor = new Color(1f, 0.92f, 0.3f, 0.92f);
            _line.endColor = new Color(1f, 0.35f, 0.08f, 0.28f);
            _line.numCapVertices = 2;
            Shader shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                _line.material = new Material(shader);
            }
            _line.enabled = false;
        }

        private void OnGUI()
        {
            if (_last.Count > 1)
            {
                GUI.Label(new Rect(16f, Screen.height - 28f, 360f, 22f), "P: show last rally trajectory replay");
            }
        }
    }
}
