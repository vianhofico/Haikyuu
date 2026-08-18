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

        public static void BuildLinuxSmoke()
        {
            Debug.Log("CI_BUILD_START");
            ProjectValidator.ValidateProjectData();
            PlayableCoreSceneBuilder.Generate();

            Directory.CreateDirectory(Path.GetDirectoryName(LinuxOutput) ?? "build/StandaloneLinux64");
            PlayerSettings.productName = "Haikyuu CI Smoke";
            PlayerSettings.companyName = "vianhofico";

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = LinuxOutput,
                target = BuildTarget.StandaloneLinux64,
                options = BuildOptions.Development
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException(
                    $"CI Linux build failed: {report.summary.result}; errors={report.summary.totalErrors}");
            }

            Debug.Log($"CI_BUILD_PASS path={LinuxOutput} bytes={report.summary.totalSize}");
        }
    }
}
#endif
