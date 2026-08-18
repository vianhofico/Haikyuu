using System.Collections;
using HaikyuuGame.Gameplay.Player;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Presentation
{
    public sealed class AnimeVisualInstaller : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Ensure()
        {
            if (FindFirstObjectByType<AnimeVisualInstaller>() != null) return;
            new GameObject("AnimeVisualInstaller").AddComponent<AnimeVisualInstaller>();
        }

        private IEnumerator Start()
        {
            yield return null;
            while (true)
            {
                PlayerActor[] players = FindObjectsByType<PlayerActor>(FindObjectsSortMode.None);
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].GetComponent<AnimePlayerVisual>() != null) continue;
                    AnimePlayerVisual visual = players[i].gameObject.AddComponent<AnimePlayerVisual>();
                    visual.Initialize(players[i]);
                }
                yield return new WaitForSecondsRealtime(1f);
            }
        }
    }
}
