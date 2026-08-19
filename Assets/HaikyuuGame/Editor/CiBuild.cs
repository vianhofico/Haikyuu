#if UNITY_EDITOR
using System;
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
        private const string AndroidTestOutput = "build/Android/Haikyuu-Test.apk";

        public static void BuildLinuxSmoke()
        {
            PrepareProject("Haikyuu CI Smoke");
            BuildAndRequireSuccess(
                ResolveBuildOutput(LinuxOutput),
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
                ResolveBuildOutput(AndroidOutput),
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
                ResolveBuildOutput(AndroidTestOutput),
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

        private static string ResolveBuildOutput(string fallbackRelativePath)
        {
            string gameCiPath = GetCommandLineArgument("-customBuildPath");
            if (!string.IsNullOrWhiteSpace(gameCiPath))
            {
                string resolvedGameCiPath = Path.GetFullPath(gameCiPath);
                Debug.Log($"CI_OUTPUT_PATH source=gameci raw={gameCiPath} resolved={resolvedGameCiPath}");
                return resolvedGameCiPath;
            }

            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string fallbackPath = Path.GetFullPath(Path.Combine(projectRoot, fallbackRelativePath));
            Debug.Log($"CI_OUTPUT_PATH source=fallback raw={fallbackRelativePath} resolved={fallbackPath}");
            return fallbackPath;
        }

        private static string GetCommandLineArgument(string argumentName)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], argumentName, StringComparison.OrdinalIgnoreCase))
                {
                    return args[i + 1];
                }
            }

            return null;
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
