using System.Collections;
using HaikyuuGame.Gameplay.Ball;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Presentation
{
    public sealed class ImpactStreakVfx : MonoBehaviour
    {
        private VolleyballBall _ball;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Ensure()
        {
            if (FindFirstObjectByType<ImpactStreakVfx>() != null) return;
            new GameObject("ImpactStreakVfx").AddComponent<ImpactStreakVfx>();
        }

        private IEnumerator Start()
        {
            while (_ball == null)
            {
                _ball = FindFirstObjectByType<VolleyballBall>();
                if (_ball == null) yield return null;
            }
            _ball.Contacted += OnContact;
        }

        private void OnDestroy()
        {
            if (_ball != null) _ball.Contacted -= OnContact;
        }

        private void OnContact(BallContact contact)
        {
            if (contact.Type != BallContactType.Attack && contact.Type != BallContactType.Block && contact.Type != BallContactType.Serve) return;
            int count = contact.Timing == ContactTimingGrade.Perfect ? 9 : 4;
            StartCoroutine(SpawnStreaks(contact.Velocity, count, contact.Timing == ContactTimingGrade.Perfect));
        }

        private IEnumerator SpawnStreaks(Vector3 velocity, int count, bool perfect)
        {
            Vector3 direction = velocity.sqrMagnitude > 0.01f ? velocity.normalized : Vector3.right;
            for (int i = 0; i < count; i++)
            {
                GameObject streak = GameObject.CreatePrimitive(PrimitiveType.Cube);
                streak.name = "ImpactStreak";
                Collider collider = streak.GetComponent<Collider>();
                if (collider != null) collider.enabled = false;
                streak.transform.position = _ball.transform.position + Random.insideUnitSphere * (perfect ? 0.45f : 0.25f);
                streak.transform.localScale = new Vector3(perfect ? 0.7f : 0.4f, 0.025f, 0.025f);
                streak.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction + Random.insideUnitSphere * 0.14f);
                streak.GetComponent<Renderer>().material.color = perfect ? new Color(1f, 0.76f, 0.15f) : new Color(0.9f, 0.92f, 1f);
                Destroy(streak, perfect ? 0.16f : 0.09f);
            }
            yield return null;
        }
    }
}
