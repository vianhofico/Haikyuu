#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HaikyuuGame.Editor
{
    public static class ReleaseBuildMenu
    {
        private const string ScenePath = "Assets/Scenes/PlayableCore.unity";

        [MenuItem("Haikyuu/Build/Windows Development")]
        public static void BuildWindowsDevelopment()
        {
            EnsureScene();
            Directory.CreateDirectory("Builds/Windows");
            Build("Builds/Windows/HaikyuuPrototype.exe", BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Haikyuu/Build/Android Development APK")]
        public static void BuildAndroidDevelopment()
        {
            EnsureScene();
            Directory.CreateDirectory("Builds/Android");
            EditorUserBuildSettings.buildAppBundle = false;
            Build("Builds/Android/HaikyuuPrototype.apk", BuildTarget.Android);
        }

        private static void EnsureScene()
        {
            if (!File.Exists(ScenePath)) PlayableCoreSceneBuilder.Generate();
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        }

        private static void Build(string outputPath, BuildTarget target)
        {
            PlayerSettings.productName = "Haikyuu Volleyball Fan Prototype";
            PlayerSettings.companyName = "vianhofico";
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = outputPath,
                target = target,
                options = BuildOptions.Development
            };
            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException($"Build failed: {report.summary.result}. Errors: {report.summary.totalErrors}");
            }
            Debug.Log($"Build succeeded: {outputPath} ({report.summary.totalSize} bytes)");
        }
    }
}
#endif
