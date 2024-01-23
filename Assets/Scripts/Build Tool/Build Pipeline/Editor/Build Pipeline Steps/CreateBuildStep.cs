namespace BuildPipeline {
    using System;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEngine;

    [System.Serializable]
    public class CreateBuildStep : BuildPipelineStep {
        public bool caching = true;
        public bool development = true;
        public bool compression = true;
        public bool append = true;

        public CreateBuildStep() {
            name = "Create Build";
        }

        public void ApplyBuildParams(SetPlatformStep.BuildPlatform targetBuildPlatform) {
            // // These settings are saved in PlayerSettings, but we set them here because they should never change.
            // PlayerSettings.enableMetalAPIValidation = true;
            // PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;
            // PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;

#if Unity_2021_1_OR_NEWER
        // This creates builds that are about 0.7mb smaller.
        EditorUserBuildSettings.il2CppCodeGeneration = UnityEditor.Build.Il2CppCodeGeneration.OptimizeSize;
#endif
#if ADDRESSABLES
        AddressablesManager.Instance.clearCacheOnEnableInPlayer = !PlayerSettings.WebGL.dataCaching;
#endif

            PlayerSettings.WebGL.dataCaching = caching;

            if (development) {
                // Changing this to speed improves build times, but increases size by 1mb.
                if (targetBuildPlatform.buildTarget == BuildTarget.WebGL) EditorUserBuildSettings.SetPlatformSettings(BuildPipeline.GetBuildTargetName(BuildTarget.WebGL), "CodeOptimization", "speed"); // "speed" or "size"
                // FullWithStacktrace increases build size by 0.2mb compared to None.
                PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.FullWithStacktrace;
                // When on, increases build size by 0.1mb!
                PlayerSettings.WebGL.decompressionFallback = !compression;
                // GZip Increases build size by 3mb vs Brotli. Only works properly over HTTPS.
                PlayerSettings.WebGL.compressionFormat = compression ? WebGLCompressionFormat.Brotli : WebGLCompressionFormat.Gzip;
                // High saved us 0.55mb vs low. Medium saved 0.4mb. Low apparently builds faster though so lets use it here!
                PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.WebGL, ManagedStrippingLevel.Low);
            } else {
                if (targetBuildPlatform.buildTarget == BuildTarget.WebGL) EditorUserBuildSettings.SetPlatformSettings(BuildPipeline.GetBuildTargetName(BuildTarget.WebGL), "CodeOptimization", "size"); // "speed" or "size"
                PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
                // Brotli compressed files need to have file headers set when uploading when decompression fallback is off, which we do when uploading the build.
                PlayerSettings.WebGL.decompressionFallback = !compression;
                PlayerSettings.WebGL.compressionFormat = compression ? WebGLCompressionFormat.Brotli : WebGLCompressionFormat.Gzip;

                // High saved us 0.55mb vs low, but causes the GetParsers function to fail because parseMethod of ParserScriptInfo can't be found. We've fixed this by adding the [Preserve] attribute to them, but we could also have made a link.xml file.
                PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.WebGL, ManagedStrippingLevel.High);
            }
        }

        static string[] GetScenes() {
            return EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        }

        void DeleteOldBuild(BuildPipelineEditorWindow.ProfileAndBucketProperties activeProfileProperties, BuildTarget buildTarget) {
            var buildDirectory = new DirectoryInfo(activeProfileProperties.GetBuildPath(BuildInfo.Instance.version.ToString(), buildTarget));
            if (buildDirectory.Exists)
                buildDirectory.Delete(true);
        }

        BuildOptions GetBuildOptions(BuildTarget buildTarget, bool autoRun, bool autoConnectProfiler) {
            var buildOptions = BuildOptions.None;
            if (development)
                buildOptions |= BuildOptions.Development;
            if (autoConnectProfiler) {
                buildOptions |= BuildOptions.ConnectWithProfiler;
                // This isn't allowed on WebGL
                if (buildTarget != BuildTarget.WebGL)
                    buildOptions |= BuildOptions.AllowDebugging;
            }

            // Don't autorun on WebGL because we can't reach the server because of CORS issues anyway. Instead, we launch the hosted URL when complete
            // Don't autorun Android because it stops build when valid device isn't connected
            if (autoRun) {
                if (buildTarget == BuildTarget.WebGL) {
                    // If we're going to load it anyway we may want to do this
                    // buildOptions |= BuildOptions.AutoRunPlayer;
                } else if (buildTarget != BuildTarget.Android) {
                    buildOptions |= BuildOptions.AutoRunPlayer;
                }
            }

            if (append && buildTarget == BuildTarget.iOS && buildTarget == BuildTarget.Android)
                buildOptions |= BuildOptions.AcceptExternalModificationsToPlayer;


            return buildOptions;
        }

        public bool Build(BuildPipelineEditorWindow.ProfileAndBucketProperties activeProfileProperties, BuildTarget buildTarget, bool autoRun, bool autoConnectProfiler) {
            progressTracker.Start("Building Application...");

            // Clear out the old build entirely
            DeleteOldBuild(activeProfileProperties, buildTarget);

            // We've tested this option on WebGL - it leads to the same results as not having it. I'd guess it's the default. CompressWithLz4 was slightly larger.
            // buildOptions = buildOptions | BuildOptions.CompressWithLz4HC;
            var buildPlayerOptions = new BuildPlayerOptions {
                scenes = GetScenes(),
                locationPathName = activeProfileProperties.GetBuildPath(BuildInfo.Instance.version.ToString(), buildTarget),
                target = buildTarget,
                targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget),
                options = GetBuildOptions(buildTarget, autoRun, autoConnectProfiler)
            };

            // if(buildPlayerOptions.targetGroup == BuildTargetGroup.iOS)
            //     PlayerSettings.SetApplicationIdentifier(buildPlayerOptions.targetGroup, activeProfileProperties.standaloneIosAppID);


            // PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new Texture2D[] { icon }, IconKind.Application);
            // This isn't allowed on WebGL, and I'm fairly sure the above does all we need anyway. Strip this if that turns out to be true!
            // PlayerSettings.SetIconsForTargetGroup(buildPlayerOptions.targetGroup, new Texture2D[] {activeProfileProperties.appIcon}, IconKind.Application);

            // Do build
            var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

            // Post build steps
            if (buildReport.summary.result == BuildResult.Succeeded) {
                progressTracker.Complete("Built Application!");
                Debug.Log("Build succeeded at " + DateTime.Now.ToLongTimeString() + "\nPath: " + buildReport.summary.outputPath);
                Debug.Log(activeProfileProperties.GetBuildPath(BuildInfo.Instance.version.ToString(), buildTarget));
                Debug.Log(buildReport.summary.outputPath);
                return true;
            } else {
                progressTracker.Fail("Build failed: " + buildReport.summary.result);
                Debug.LogError("Build Failed\n" + buildReport.summary.result);
                return false;
            }
        }

        public override void DrawSettings() {
            development = EditorGUILayout.Toggle(new GUIContent("Development", "Leave unticked for public releases. If ticked, this build will include debugging code and have a label identifying the build as for debugging purposes only at all times."), development);
            if (BuildPipelineEditorWindow.BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget == BuildTarget.WebGL) {
                compression = EditorGUILayout.Toggle(new GUIContent("Compression", "Compresses files to Brotli and removes the decompression fallback. This is not currently set up for releases on the BU server."), compression);
                caching = EditorGUILayout.Toggle(new GUIContent("Caching", "If caching is enabled on the build."), caching);
            }
        }
    }
}