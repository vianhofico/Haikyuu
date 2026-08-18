using System.Collections.Generic;
using HaikyuuGame.Core;
using HaikyuuGame.Gameplay;
using HaikyuuGame.Gameplay.Ball;
using HaikyuuGame.Gameplay.Character;
using HaikyuuGame.Gameplay.Input;
using HaikyuuGame.Gameplay.Match;
using HaikyuuGame.Gameplay.Player;
using HaikyuuGame.Gameplay.Presentation;
using HaikyuuGame.Gameplay.UI;
using HaikyuuGame.Meta;
using HaikyuuGame.Training;
using UnityEngine;

namespace HaikyuuGame
{
    public sealed class PlayableCoreBootstrap : MonoBehaviour
    {
        private readonly VolleyballTuning _tuning = new VolleyballTuning();
        private readonly List<PlayerActor> _players = new List<PlayerActor>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureBootstrap()
        {
            if (FindFirstObjectByType<PlayableCoreBootstrap>() != null)
            {
                return;
            }

            GameObject root = new GameObject("PlayableCoreBootstrap");
            root.AddComponent<PlayableCoreBootstrap>();
        }

        private void Start()
        {
            BuildLighting();
            Camera camera = BuildCamera();
            BuildCourt();
            VolleyballBall ball = BuildBall();

            if (FindFirstObjectByType<TouchInputRouter>() == null)
            {
                gameObject.AddComponent<TouchInputRouter>();
            }

            MatchCameraController cameraController = gameObject.AddComponent<MatchCameraController>();
            cameraController.Initialize(camera, ball);
            GameFeelController feel = gameObject.AddComponent<GameFeelController>();
            feel.Initialize(ball, cameraController);
            ProceduralAudioController audio = gameObject.AddComponent<ProceduralAudioController>();
            audio.Initialize(ball);

            BuildTeams(ball, out TeamRotation leftRotation, out TeamRotation rightRotation);

            PlayableCoreHud hud = gameObject.AddComponent<PlayableCoreHud>();
            RallyController rally = gameObject.AddComponent<RallyController>();
            rally.Initialize(ball, _players, _tuning, hud, leftRotation, rightRotation);

            TrainingTracker training = gameObject.AddComponent<TrainingTracker>();
            training.Initialize(rally);

            GameShellController shell = gameObject.AddComponent<GameShellController>();
            shell.Initialize(rally, _tuning);
        }

        private static void BuildLighting()
        {
            if (FindFirstObjectByType<Light>() != null)
            {
                return;
            }

            GameObject lightObject = new GameObject("Directional Light");
            Light lightComponent = lightObject.AddComponent<Light>();
            lightComponent.type = LightType.Directional;
            lightComponent.intensity = 1.15f;
            lightObject.transform.rotation = Quaternion.Euler(52f, -28f, 0f);
        }

        private static Camera BuildCamera()
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
            }

            camera.transform.position = new Vector3(0f, 8.2f, -17.8f);
            camera.transform.LookAt(new Vector3(0f, 1.6f, 0f));
            camera.fieldOfView = 47f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 120f;
            return camera;
        }

        private void BuildCourt()
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "CourtFloor";
            floor.transform.position = new Vector3(0f, -0.05f, 0f);
            floor.transform.localScale = new Vector3(_tuning.halfCourtLength * 2f, 0.1f, _tuning.halfCourtWidth * 2f);
            floor.GetComponent<Renderer>().material.color = new Color(0.86f, 0.46f, 0.20f);

            GameObject net = GameObject.CreatePrimitive(PrimitiveType.Cube);
            net.name = "Net";
            net.transform.position = new Vector3(0f, _tuning.netHeight * 0.5f, 0f);
            net.transform.localScale = new Vector3(0.08f, _tuning.netHeight, (_tuning.halfCourtWidth * 2f) + 0.2f);
            net.GetComponent<Renderer>().material.color = new Color(0.92f, 0.92f, 0.92f);

            CreateLine(new Vector3(0f, 0.015f, -_tuning.halfCourtWidth), new Vector3(_tuning.halfCourtLength * 2f, 0.025f, 0.055f));
            CreateLine(new Vector3(0f, 0.015f, _tuning.halfCourtWidth), new Vector3(_tuning.halfCourtLength * 2f, 0.025f, 0.055f));
            CreateLine(new Vector3(-_tuning.halfCourtLength, 0.015f, 0f), new Vector3(0.055f, 0.025f, _tuning.halfCourtWidth * 2f));
            CreateLine(new Vector3(_tuning.halfCourtLength, 0.015f, 0f), new Vector3(0.055f, 0.025f, _tuning.halfCourtWidth * 2f));
            CreateAttackLine(-3f);
            CreateAttackLine(3f);
        }

        private void CreateAttackLine(float x)
        {
            CreateLine(new Vector3(x, 0.018f, 0f), new Vector3(0.045f, 0.026f, _tuning.halfCourtWidth * 2f));
        }

        private static void CreateLine(Vector3 position, Vector3 scale)
        {
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.name = "CourtLine";
            line.transform.position = position;
            line.transform.localScale = scale;
            line.GetComponent<Renderer>().material.color = Color.white;
            Collider collider = line.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        private static VolleyballBall BuildBall()
        {
            GameObject ballObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ballObject.name = "Volleyball";
            ballObject.transform.localScale = Vector3.one * 0.28f;
            ballObject.GetComponent<Renderer>().material.color = new Color(0.95f, 0.88f, 0.25f);
            Rigidbody body = ballObject.AddComponent<Rigidbody>();
            body.mass = 0.27f;
            return ballObject.AddComponent<VolleyballBall>();
        }

        private void BuildTeams(
            VolleyballBall ball,
            out TeamRotation leftRotation,
            out TeamRotation rightRotation)
        {
            Vector3[] leftFormation = CreateFormation(TeamSide.Left);
            Vector3[] rightFormation = CreateFormation(TeamSide.Right);

            string[] leftIds =
            {
                "kageyama_tobio",
                "asahi_azumane",
                "hinata_shoyo",
                "daichi_sawamura",
                "tanaka_ryunosuke",
                "tsukishima_kei"
            };

            string[] rightIds =
            {
                "miya_atsumu",
                "aran_ojiro",
                "suna_rintaro",
                "miya_osamu",
                "kita_shinsuke",
                "omimi_ren"
            };

            List<PlayerActor> leftRoster = new List<PlayerActor>();
            List<PlayerActor> rightRoster = new List<PlayerActor>();

            for (int i = 0; i < 6; i++)
            {
                RuntimeCharacterProfile leftProfile = HaikyuuRosterCatalog.Get(leftIds[i]);
                RuntimeCharacterProfile rightProfile = HaikyuuRosterCatalog.Get(rightIds[i]);
                leftRoster.Add(CreatePlayer(TeamSide.Left, i == 2, leftProfile, leftFormation[i], ball));
                rightRoster.Add(CreatePlayer(TeamSide.Right, false, rightProfile, rightFormation[i], ball));
            }

            PlayerActor leftLibero = CreatePlayer(
                TeamSide.Left,
                false,
                HaikyuuRosterCatalog.Get("nishinoya_yu"),
                new Vector3(-12f, 1f, 0f),
                ball);
            PlayerActor rightLibero = CreatePlayer(
                TeamSide.Right,
                false,
                HaikyuuRosterCatalog.Get("akagi_michinari"),
                new Vector3(12f, 1f, 0f),
                ball);

            leftRotation = new TeamRotation(TeamSide.Left, leftRoster, leftLibero, leftFormation);
            rightRotation = new TeamRotation(TeamSide.Right, rightRoster, rightLibero, rightFormation);
        }

        private static Vector3[] CreateFormation(TeamSide team)
        {
            float sign = team == TeamSide.Left ? -1f : 1f;
            return new[]
            {
                new Vector3(sign * 2.1f, 1f, -2.5f),
                new Vector3(sign * 2.1f, 1f, 0f),
                new Vector3(sign * 2.1f, 1f, 2.5f),
                new Vector3(sign * 6.0f, 1f, 2.5f),
                new Vector3(sign * 6.0f, 1f, 0f),
                new Vector3(sign * 6.0f, 1f, -2.5f)
            };
        }

        private PlayerActor CreatePlayer(
            TeamSide team,
            bool human,
            RuntimeCharacterProfile profile,
            Vector3 position,
            VolleyballBall ball)
        {
            GameObject playerObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            string playerName = profile != null ? profile.DisplayName.Replace(" ", "_") : "Unknown";
            playerObject.name = human ? $"Player_Human_{playerName}" : $"Player_{team}_{playerName}";
            playerObject.transform.localScale = new Vector3(0.65f, 1f, 0.65f);

            Collider collider = playerObject.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            Color teamColor = team == TeamSide.Left
                ? new Color(0.12f, 0.12f, 0.16f)
                : new Color(0.12f, 0.58f, 0.72f);

            if (profile != null && profile.Role == VolleyballRole.Libero)
            {
                teamColor = team == TeamSide.Left
                    ? new Color(0.12f, 0.55f, 0.25f)
                    : new Color(0.78f, 0.24f, 0.22f);
            }

            if (human)
            {
                teamColor = new Color(1f, 0.52f, 0.05f);
            }

            playerObject.GetComponent<Renderer>().material.color = teamColor;
            PlayerActor actor = playerObject.AddComponent<PlayerActor>();
            actor.Initialize(team, human, profile, position, ball, _tuning);
            _players.Add(actor);
            return actor;
        }
    }
}
