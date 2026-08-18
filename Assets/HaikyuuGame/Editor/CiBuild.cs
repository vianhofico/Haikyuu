#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace HaikyuuGame.Editor
{
    public static class CiBuild
    {
        private const string ScenePath = "Assets/Scenes/PlayableCore.unity";
        private const string LinuxOutput = "build/StandaloneLinux64/HaikyuuSmoke.x86_64";
        private const string AndroidOutput = "build/Android/Haikyuu.apk";

        public static void BuildLinuxSmoke()
        {
            PrepareProject("Haikyuu CI Smoke");
            Directory.CreateDirectory(Path.GetDirectoryName(LinuxOutput) ?? "build/StandaloneLinux64");

            BuildAndRequireSuccess(
                LinuxOutput,
                BuildTarget.StandaloneLinux64,
                BuildOptions.Development,
                "CI_BUILD_PASS");
        }

        public static void BuildAndroidApk()
        {
            Debug.Log("CI_ANDROID_BUILD_START");
            PrepareProject("Haikyuu Volleyball Prototype");
            Directory.CreateDirectory(Path.GetDirectoryName(AndroidOutput) ?? "build/Android");

            EditorUserBuildSettings.buildAppBundle = false;
            BuildAndRequireSuccess(
                AndroidOutput,
                BuildTarget.Android,
                BuildOptions.Development,
                "CI_ANDROID_BUILD_PASS");
        }

        private static void PrepareProject(string productName)
        {
            Debug.Log("CI_BUILD_START");
            ProjectValidator.ValidateProjectData();
            PlayableCoreSceneBuilder.Generate();
            PlayerSettings.productName = productName;
            PlayerSettings.companyName = "vianhofico";
        }

        private static void BuildAndRequireSuccess(
            string outputPath,
            BuildTarget target,
            BuildOptions buildOptions,
            string successMarker)
        {
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = outputPath,
                target = target,
                options = buildOptions
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException(
                    $"CI {target} build failed: {report.summary.result}; errors={report.summary.totalErrors}");
            }

            Debug.Log($"{successMarker} path={outputPath} bytes={report.summary.totalSize}");
        }
    }
}
#endif
