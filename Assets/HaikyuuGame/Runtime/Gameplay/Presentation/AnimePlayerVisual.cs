using HaikyuuGame.Gameplay.Player;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Presentation
{
    public sealed class AnimePlayerVisual : MonoBehaviour
    {
        private PlayerActor _actor;
        private Transform _visualRoot;
        private Transform _leftArm;
        private Transform _rightArm;
        private Transform _head;
        private string _profileId;
        private Vector3 _lastPosition;
        private float _motion;

        public void Initialize(PlayerActor actor)
        {
            _actor = actor;
            Renderer rootRenderer = actor.GetComponent<Renderer>();
            if (rootRenderer != null) rootRenderer.enabled = false;
            Rebuild();
            _lastPosition = actor.transform.position;
        }

        private void Update()
        {
            if (_actor == null) return;
            string currentId = _actor.Profile != null ? _actor.Profile.Id : string.Empty;
            if (_profileId != currentId) Rebuild();

            float speed = Vector3.Distance(_actor.transform.position, _lastPosition) / Mathf.Max(Time.deltaTime, 0.001f);
            _motion += Time.deltaTime * Mathf.Lerp(2f, 10f, Mathf.Clamp01(speed / 6f));
            float swing = Mathf.Sin(_motion) * Mathf.Clamp01(speed / 3f) * 25f;
            if (_leftArm != null) _leftArm.localRotation = Quaternion.Euler(swing, 0f, 8f);
            if (_rightArm != null) _rightArm.localRotation = Quaternion.Euler(-swing, 0f, -8f);
            if (_head != null) _head.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(_motion * 0.5f) * 2f);
            _lastPosition = _actor.transform.position;
        }

        private void Rebuild()
        {
            if (_actor == null) return;
            if (_visualRoot != null) Destroy(_visualRoot.gameObject);
            _profileId = _actor.Profile != null ? _actor.Profile.Id : string.Empty;
            GameObject root = new GameObject("AnimeVisual");
            root.transform.SetParent(transform, false);
            _visualRoot = root.transform;

            string school = _actor.Profile != null ? _actor.Profile.School : string.Empty;
            TeamColors colors = TeamPalette.Get(school);
            int hash = StableHash(_profileId);
            Color skin = new Color(0.94f, 0.72f + ((hash & 7) * 0.008f), 0.58f + ((hash & 3) * 0.015f));
            Color hair = HairColor(hash);

            Transform torso = Primitive("Torso", PrimitiveType.Capsule, new Vector3(0f, 0.18f, 0f), new Vector3(0.72f, 0.58f, 0.52f), colors.Primary, root.transform);
            Transform shorts = Primitive("Shorts", PrimitiveType.Cube, new Vector3(0f, -0.42f, 0f), new Vector3(0.68f, 0.28f, 0.52f), colors.Secondary, root.transform);
            _head = Primitive("Head", PrimitiveType.Sphere, new Vector3(0f, 0.82f, 0f), new Vector3(0.52f, 0.56f, 0.5f), skin, root.transform);

            Primitive("HairBack", PrimitiveType.Sphere, new Vector3(0f, 1.03f, 0.03f), new Vector3(0.55f, 0.28f, 0.52f), hair, root.transform);
            for (int i = 0; i < 4; i++)
            {
                float x = -0.22f + i * 0.145f;
                Transform spike = Primitive($"HairSpike{i}", PrimitiveType.Cube, new Vector3(x, 1.17f + ((hash >> i) & 1) * 0.05f, -0.02f), new Vector3(0.12f, 0.28f, 0.14f), hair, root.transform);
                spike.localRotation = Quaternion.Euler(0f, 0f, -28f + i * 18f);
            }

            Primitive("EyeL", PrimitiveType.Sphere, new Vector3(-0.105f, 0.88f, -0.235f), new Vector3(0.045f, 0.065f, 0.035f), new Color(0.03f, 0.03f, 0.04f), root.transform);
            Primitive("EyeR", PrimitiveType.Sphere, new Vector3(0.105f, 0.88f, -0.235f), new Vector3(0.045f, 0.065f, 0.035f), new Color(0.03f, 0.03f, 0.04f), root.transform);

            _leftArm = Limb("ArmL", new Vector3(-0.48f, 0.18f, 0f), skin, root.transform);
            _rightArm = Limb("ArmR", new Vector3(0.48f, 0.18f, 0f), skin, root.transform);
            Limb("LegL", new Vector3(-0.2f, -0.86f, 0f), skin, root.transform).localScale = new Vector3(0.2f, 0.55f, 0.2f);
            Limb("LegR", new Vector3(0.2f, -0.86f, 0f), skin, root.transform).localScale = new Vector3(0.2f, 0.55f, 0.2f);

            GameObject labelObject = new GameObject("NameLabel");
            labelObject.transform.SetParent(root.transform, false);
            labelObject.transform.localPosition = new Vector3(0f, 1.55f, 0f);
            labelObject.transform.localRotation = Quaternion.Euler(35f, 0f, 0f);
            TextMesh label = labelObject.AddComponent<TextMesh>();
            label.text = ShortName(_actor.DisplayName);
            label.fontSize = 34;
            label.characterSize = 0.035f;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.color = colors.Accent;
        }

        private static Transform Limb(string name, Vector3 position, Color color, Transform parent)
        {
            Transform limb = Primitive(name, PrimitiveType.Capsule, position, new Vector3(0.18f, 0.48f, 0.18f), color, parent);
            return limb;
        }

        private static Transform Primitive(string name, PrimitiveType type, Vector3 localPosition, Vector3 localScale, Color color, Transform parent)
        {
            GameObject obj = GameObject.CreatePrimitive(type);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = localPosition;
            obj.transform.localScale = localScale;
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null) collider.enabled = false;
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null) renderer.material.color = color;
            return obj.transform;
        }

        private static int StableHash(string value)
        {
            unchecked
            {
                int hash = 17;
                if (value != null)
                {
                    for (int i = 0; i < value.Length; i++) hash = hash * 31 + value[i];
                }
                return hash & int.MaxValue;
            }
        }

        private static Color HairColor(int hash)
        {
            switch (hash % 7)
            {
                case 0: return new Color(0.08f, 0.06f, 0.05f);
                case 1: return new Color(0.95f, 0.31f, 0.05f);
                case 2: return new Color(0.12f, 0.1f, 0.08f);
                case 3: return new Color(0.8f, 0.68f, 0.42f);
                case 4: return new Color(0.15f, 0.18f, 0.22f);
                case 5: return new Color(0.72f, 0.72f, 0.7f);
                default: return new Color(0.3f, 0.2f, 0.12f);
            }
        }

        private static string ShortName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return "PLAYER";
            string[] parts = fullName.Split(' ');
            return parts[parts.Length - 1].ToUpperInvariant();
        }
    }
}
