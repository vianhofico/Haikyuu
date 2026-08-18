using HaikyuuGame.Gameplay.Ball;
using UnityEngine;

namespace HaikyuuGame.Gameplay.Presentation
{
    public sealed class ProceduralAudioController : MonoBehaviour
    {
        private const int SampleRate = 22050;
        private VolleyballBall _ball;
        private AudioSource _source;
        private AudioClip _attack;
        private AudioClip _block;
        private AudioClip _receive;
        private AudioClip _set;
        private AudioClip _serve;

        public void Initialize(VolleyballBall ball)
        {
            _ball = ball;
            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.spatialBlend = 0f;
            _attack = CreateImpact("Attack", 0.12f, 95f, 0.65f, 11);
            _block = CreateImpact("Block", 0.11f, 155f, 0.75f, 23);
            _receive = CreateImpact("Receive", 0.09f, 125f, 0.40f, 37);
            _set = CreateImpact("Set", 0.065f, 210f, 0.28f, 41);
            _serve = CreateImpact("Serve", 0.10f, 115f, 0.5f, 53);
            _ball.Contacted += OnContact;
        }

        private void OnDestroy()
        {
            if (_ball != null)
            {
                _ball.Contacted -= OnContact;
            }
        }

        private void OnContact(BallContact contact)
        {
            AudioClip clip;
            float volume;

            switch (contact.Type)
            {
                case BallContactType.Attack: clip = _attack; volume = 0.9f; break;
                case BallContactType.Block: clip = _block; volume = 1f; break;
                case BallContactType.Set: clip = _set; volume = 0.45f; break;
                case BallContactType.Serve: clip = _serve; volume = 0.75f; break;
                default: clip = _receive; volume = 0.55f; break;
            }

            if (clip != null)
            {
                _source.PlayOneShot(clip, volume);
            }
        }

        private static AudioClip CreateImpact(string name, float duration, float frequency, float noiseAmount, int seed)
        {
            int samples = Mathf.Max(64, Mathf.RoundToInt(SampleRate * duration));
            float[] data = new float[samples];
            System.Random random = new System.Random(seed);

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float progress = i / (float)samples;
                float envelope = (1f - progress) * (1f - progress);
                float sine = Mathf.Sin(2f * Mathf.PI * frequency * t);
                float noise = ((float)random.NextDouble() * 2f) - 1f;
                data[i] = ((sine * (1f - noiseAmount)) + (noise * noiseAmount)) * envelope * 0.55f;
            }

            AudioClip clip = AudioClip.Create(name, samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
