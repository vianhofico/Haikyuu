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
            string outputPath = ResolveProjectPath(LinuxOutput);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ResolveProjectPath("build/StandaloneLinux64"));

            BuildAndRequireSuccess(
                outputPath,
                BuildTarget.StandaloneLinux64,
                BuildOptions.Development,
                "CI_BUILD_PASS");
        }

        public static void BuildAndroidApk()
        {
            Debug.Log("CI_ANDROID_BUILD_START");
            PrepareProject("Haikyuu Volleyball Prototype");
            string outputPath = ResolveProjectPath(AndroidOutput);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? ResolveProjectPath("build/Android"));

            EditorUserBuildSettings.buildAppBundle = false;
            BuildAndRequireSuccess(
                outputPath,
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

        private static string ResolveProjectPath(string relativePath)
        {
            // Unity Builder executes the editor in a container. Resolve build paths
            // from Application.dataPath so output lands in the mounted repository
            // workspace instead of depending on the process working directory.
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
