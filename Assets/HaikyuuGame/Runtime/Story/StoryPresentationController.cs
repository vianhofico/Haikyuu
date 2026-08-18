using HaikyuuGame.Meta;
using HaikyuuGame.Persistence;
using UnityEngine;

namespace HaikyuuGame.Story
{
    public sealed class StoryPresentationController : MonoBehaviour
    {
        private int _lastRevision = -1;
        private StoryChapter _chapter;
        private float _showUntil;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Ensure()
        {
            if (FindFirstObjectByType<StoryPresentationController>() != null)
            {
                return;
            }

            new GameObject("StoryPresentationController").AddComponent<StoryPresentationController>();
        }

        private void Update()
        {
            if (GameSessionState.CurrentMode != GameMode.Story)
            {
                return;
            }

            if (_lastRevision != GameSessionState.SessionRevision)
            {
                _lastRevision = GameSessionState.SessionRevision;
                SaveGameData save = new SaveGameService().Load();
                _chapter = StoryCampaignCatalog.Get(save.storyChapter);
                _showUntil = Time.unscaledTime + 5f;
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Return)
                || UnityEngine.Input.GetKeyDown(KeyCode.F)
                || UnityEngine.Input.GetKeyDown(KeyCode.J))
            {
                _showUntil = 0f;
            }
        }

        private void OnGUI()
        {
            if (GameSessionState.CurrentMode != GameMode.Story || Time.unscaledTime >= _showUntil)
            {
                return;
            }

            float width = Mathf.Min(620f, Screen.width - 32f);
            float height = 132f;
            float x = (Screen.width - width) * 0.5f;
            float y = Mathf.Max(70f, Screen.height * 0.12f);
            GUI.Box(new Rect(x, y, width, height), $"CHAPTER {_chapter.Index + 1} — {_chapter.Title}");
            GUI.Label(new Rect(x + 22f, y + 38f, width - 44f, 24f), $"Opponent: {_chapter.Opponent}");
            GUI.Label(new Rect(x + 22f, y + 64f, width - 44f, 42f), $"Objective: {_chapter.Objective}");
            GUI.Label(new Rect(x + 22f, y + 108f, width - 44f, 20f), "Enter / Action: close");
        }
    }
}
