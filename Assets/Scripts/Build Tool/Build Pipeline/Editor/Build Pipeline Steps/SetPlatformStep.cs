namespace BuildPipeline {
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [System.Serializable]
    public class SetPlatformStep : BuildPipelineStep {
        public BuildPlatform targetBuildPlatform;

        #region Regions

        public List<BuildPlatform> buildPlatforms = new List<BuildPlatform>() {
            new BuildPlatform(BuildPlatformType.WebGL, BuildTargetGroup.WebGL, BuildTarget.WebGL, new GUIContent("WebGL", "WebGL")),
            new BuildPlatform(BuildPlatformType.iOS, BuildTargetGroup.iOS, BuildTarget.iOS, new GUIContent("iOS", "iOS")),
            new BuildPlatform(BuildPlatformType.Android, BuildTargetGroup.Android, BuildTarget.Android, new GUIContent("Android", "Android")),
            new BuildPlatform(BuildPlatformType.WindowsStandalone, BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, new GUIContent("Windows", "64bit Windows")),
            new BuildPlatform(BuildPlatformType.OSXStandalone, BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX, new GUIContent("OSX", "OSX")),
        };

        [Flags]
        public enum BuildPlatformType {
            WebGL,
            WebGLOffPlatform,
            iOS,
            Android,
            WindowsStandalone,
            OSXStandalone,
        }

        public class BuildPlatform {
            public BuildPlatformType type;
            public BuildTargetGroup buildTargetGroup;
            public BuildTarget buildTarget;
            public GUIContent label;
            public bool isInstalled => BuildPipeline.IsBuildTargetSupported(buildTargetGroup, buildTarget);

            public BuildPlatform(BuildPlatformType type, BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, GUIContent label) {
                this.type = type;
                this.buildTargetGroup = buildTargetGroup;
                this.buildTarget = buildTarget;
                this.label = label;
            }
        }

        #endregion

        public SetPlatformStep() {
            name = "Set Platform";
            canBeDisabled = false;
        }

        public void SwitchToTargetPlatform() {
            progressTracker.Update("Switching build target...", 0);
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(targetBuildPlatform.buildTarget), targetBuildPlatform.buildTarget);
            progressTracker.Complete("Switched build target!");
        }

        public void ResetSettingsForActivePlatform() {
            if (targetBuildPlatform == null) {
                targetBuildPlatform = buildPlatforms.FirstOrDefault(x => x.buildTarget == EditorUserBuildSettings.activeBuildTarget);
                if (targetBuildPlatform == null) targetBuildPlatform = buildPlatforms.FirstOrDefault(x => x.isInstalled);
            }
        }

        public override void DrawSettings() {
            if (!targetBuildPlatform.isInstalled) {
                EditorGUILayout.HelpBox("Target build platform doesn't seem to be installed! Please switch to one of the ones shown below.", MessageType.Error);
            }

            GUILayout.BeginHorizontal();

            for (int i = 0; i < buildPlatforms.Count; i++) {
                BuildPlatform buildPlatform = buildPlatforms[i];
                bool current = buildPlatform == targetBuildPlatform;
                EditorGUI.BeginDisabledGroup(!buildPlatform.isInstalled);
                // +(buildPlatform.isInstalled?"":" (not installed)")
                if (GUILayout.Toggle(current, buildPlatform.label, BuildPipelineUtils.MiniButtonGUI(i, buildPlatforms.Count))) {
                    targetBuildPlatform = buildPlatform;
                }

                EditorGUI.EndDisabledGroup();
            }
            // void DrawBuildTargetToggle(BuildTargetGroup targetBuildTargetGroup, BuildTarget targetBuildTarget, string name, int index, int length) {
            //     bool installed = BuildPipeline.IsBuildTargetSupported(targetBuildTargetGroup, targetBuildTarget);
            //     bool current = buildTarget == targetBuildTarget;
            //     EditorGUI.BeginDisabledGroup(!installed);
            //     if(!installed) name = name+" (not installed)";
            //     if(GUILayout.Toggle(current, name, MiniButtonGUI(index, length))) {
            //         buildTarget = targetBuildTarget;
            //     }
            //     EditorGUI.EndDisabledGroup();
            // }

            // DrawBuildTargetToggle(BuildTargetGroup.iOS, BuildTarget.iOS, "iOS", 0, 4);
            // DrawBuildTargetToggle(BuildTargetGroup.WebGL, BuildTarget.WebGL, "WebGL", 1, 4);
            // DrawBuildTargetToggle(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, "Standalone Windows", 2, 4);
            // DrawBuildTargetToggle(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX, "Standalone OSX", 3, 4);
            GUILayout.EndHorizontal();
        }
    }
}