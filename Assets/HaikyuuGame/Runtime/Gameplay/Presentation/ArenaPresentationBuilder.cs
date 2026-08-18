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
            BuildBackdrop(root.transform);
            BuildStands(root.transform);
            BuildCrowd(root.transform);
            BuildLights(root.transform);
        }

        private static void BuildBackdrop(Transform root)
        {
            CreateCube("BackWall", new Vector3(0f, 5f, 9f), new Vector3(25f, 10f, 0.4f), new Color(0.06f, 0.08f, 0.12f), root);
            CreateCube("LeftWall", new Vector3(-12f, 5f, 0f), new Vector3(0.4f, 10f, 18f), new Color(0.05f, 0.06f, 0.09f), root);
            CreateCube("RightWall", new Vector3(12f, 5f, 0f), new Vector3(0.4f, 10f, 18f), new Color(0.05f, 0.06f, 0.09f), root);
            CreateCube("CeilingBeam", new Vector3(0f, 9.5f, 0f), new Vector3(24f, 0.25f, 0.4f), new Color(0.1f, 0.11f, 0.14f), root);
        }

        private static void BuildStands(Transform root)
        {
            for (int row = 0; row < 3; row++)
            {
                float y = 0.8f + row * 0.65f;
                float zBack = 5.5f + row * 0.75f;
                float zFront = -5.5f - row * 0.75f;
                CreateCube($"StandBack{row}", new Vector3(0f, y, zBack), new Vector3(20f, 0.6f, 1.2f), new Color(0.12f + row * 0.02f, 0.14f, 0.18f), root);
                CreateCube($"StandCameraSide{row}", new Vector3(0f, y, zFront), new Vector3(20f, 0.6f, 1.2f), new Color(0.12f + row * 0.02f, 0.14f, 0.18f), root);
            }
        }

        private static void BuildCrowd(Transform root)
        {
            for (int side = -1; side <= 1; side += 2)
            {
                for (int row = 0; row < 3; row++)
                {
                    for (int i = 0; i < 18; i++)
                    {
                        float x = -8.5f + i;
                        float z = side * (5.1f + row * 0.75f);
                        float y = 1.35f + row * 0.65f;
                        int seed = (i * 13) + (row * 7) + (side > 0 ? 5 : 0);
                        Color color = new Color(0.22f + (seed % 5) * 0.06f, 0.23f + (seed % 3) * 0.05f, 0.27f + (seed % 4) * 0.05f);
                        CreateSphere($"Crowd_{side}_{row}_{i}", new Vector3(x, y, z), new Vector3(0.28f, 0.36f, 0.28f), color, root);
                    }
                }
            }
        }

        private static void BuildLights(Transform root)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject lightObject = new GameObject($"ArenaLight{i}");
                lightObject.transform.SetParent(root, false);
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
