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
        private const string AndroidTestOutput = "build/AndroidTest/Haikyuu-Test.apk";

        public static void BuildLinuxSmoke()
        {
            PrepareProject("Haikyuu CI Smoke");
            BuildAndRequireSuccess(
                ResolveProjectPath(LinuxOutput),
                BuildTarget.StandaloneLinux64,
                BuildOptions.Development,
                "CI_BUILD_PASS");
        }

        public static void BuildAndroidApk()
        {
            Debug.Log("CI_ANDROID_BUILD_START");
            PrepareProject("Haikyuu Volleyball Prototype");
            EditorUserBuildSettings.buildAppBundle = false;
            BuildAndRequireSuccess(
                ResolveProjectPath(AndroidOutput),
                BuildTarget.Android,
                BuildOptions.Development,
                "CI_ANDROID_BUILD_PASS");
        }

        public static void BuildAndroidTestApk()
        {
            Debug.Log("CI_ANDROID_TEST_BUILD_START");
            PrepareAndroidTestProject("Haikyuu Test");
            EditorUserBuildSettings.buildAppBundle = false;
            BuildAndRequireSuccess(
                ResolveProjectPath(AndroidTestOutput),
                BuildTarget.Android,
                BuildOptions.Development | BuildOptions.AllowDebugging,
                "CI_ANDROID_TEST_BUILD_PASS");
        }

        private static void PrepareProject(string productName)
        {
            Debug.Log("CI_BUILD_START");
            ProjectValidator.ValidateProjectData();
            PlayableCoreSceneBuilder.Generate();
            ApplyPlayerSettings(productName);
        }

        private static void PrepareAndroidTestProject(string productName)
        {
            Debug.Log("CI_TEST_BUILD_START");
            Debug.Log("CI_TEST_BUILD_NOTE ProjectValidator.ValidateProjectData is intentionally skipped for sideload testing.");
            PlayableCoreSceneBuilder.Generate();
            ApplyPlayerSettings(productName);
        }

        private static void ApplyPlayerSettings(string productName)
        {
            PlayerSettings.productName = productName;
            PlayerSettings.companyName = "vianhofico";
        }

        private static string ResolveProjectPath(string relativePath)
        {
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string resolved = Path.GetFullPath(Path.Combine(projectRoot, relativePath));
            Debug.Log($"CI_OUTPUT_PATH relative={relativePath} resolved={resolved}");
            return resolved;
        }

        private static void BuildAndRequireSuccess(
            string outputPath,
            BuildTarget target,
            BuildOptions buildOptions,
            string successMarker)
        {
            string outputDirectory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

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

            if (!File.Exists(outputPath))
            {
                throw new BuildFailedException(
                    $"CI {target} reported success but output file is missing: {outputPath}");
            }

            FileInfo output = new FileInfo(outputPath);
            if (output.Length <= 0)
            {
                throw new BuildFailedException(
                    $"CI {target} produced an empty output file: {outputPath}");
            }

            Debug.Log($"{successMarker} path={outputPath} bytes={output.Length} reportBytes={report.summary.totalSize}");
        }
    }
}
#endif
