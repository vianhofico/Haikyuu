using UnityEngine;

namespace HaikyuuGame.Gameplay.Presentation
{
    public sealed class ArenaPresentationBuilder : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Build()
        {
            if (GameObject.Find("ArenaPresentation") != null) return;
            GameObject root = new GameObject("ArenaPresentation");
            CreateCube("BackWall", new Vector3(0f, 5f, 9f), new Vector3(25f, 10f, 0.4f), new Color(0.06f, 0.08f, 0.12f), root.transform);
            CreateCube("LeftWall", new Vector3(-12f, 5f, 0f), new Vector3(0.4f, 10f, 18f), new Color(0.05f, 0.06f, 0.09f), root.transform);
            CreateCube("RightWall", new Vector3(12f, 5f, 0f), new Vector3(0.4f, 10f, 18f), new Color(0.05f, 0.06f, 0.09f), root.transform);
            CreateCube("CeilingBeam", new Vector3(0f, 9.5f, 0f), new Vector3(24f, 0.25f, 0.4f), new Color(0.1f, 0.11f, 0.14f), root.transform);

            for (int row = 0; row < 4; row++)
            {
                float y = 0.8f + row * 0.65f;
                float z = 5.5f + row * 0.7f;
                CreateCube($"StandBack{row}", new Vector3(0f, y, z), new Vector3(20f, 0.6f, 1.1f), new Color(0.12f + row * 0.018f, 0.14f, 0.18f), root.transform);
                for (int i = 0; i < 18; i++)
                {
                    int seed = i * 13 + row * 7;
                    Color color = new Color(0.22f + (seed % 5) * 0.06f, 0.23f + (seed % 3) * 0.05f, 0.27f + (seed % 4) * 0.05f);
                    CreateSphere($"Crowd_{row}_{i}", new Vector3(-8.5f + i, y + 0.55f, z - 0.25f), new Vector3(0.28f, 0.36f, 0.28f), color, root.transform);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                GameObject lightObject = new GameObject($"ArenaLight{i}");
                lightObject.transform.SetParent(root.transform, false);
                lightObject.transform.position = new Vector3(-7.5f + i * 5f, 8f, -1f);
                Light light = lightObject.AddComponent<Light>();
                light.type = LightType.Point;
                light.range = 14f;
                light.intensity = 1.3f;
                light.color = new Color(1f, 0.94f, 0.84f);
            }
        }

        private static void CreateCube(string name, Vector3 position, Vector3 scale, Color color, Transform parent)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.position = position;
            obj.transform.localScale = scale;
            obj.GetComponent<Renderer>().material.color = color;
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null) collider.enabled = false;
        }

        private static void CreateSphere(string name, Vector3 position, Vector3 scale, Color color, Transform parent)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.position = position;
            obj.transform.localScale = scale;
            obj.GetComponent<Renderer>().material.color = color;
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null) collider.enabled = false;
        }
    }
}
