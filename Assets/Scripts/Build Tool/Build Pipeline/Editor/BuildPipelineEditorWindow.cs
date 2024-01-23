namespace BuildPipeline {
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection.Emit;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
    using Debug = UnityEngine.Debug;
#if ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Diagnostics;
using UnityEditor.Build.Pipeline.Utilities;
#endif
    using UnityEditor.IMGUI.Controls;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
    using UnityEngine.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon;
    using Amazon.Runtime;
    using Amazon.S3;
    using Amazon.S3.Transfer;
    using Amazon.S3.Model;

// #define Addressables

// This is an editor window that we use for producing and uploading builds for testing and production.
// It builds and uploads addressables, produces builds, performs platform specific additional tasks, and uploads and commits builds to wherever they need to go.
// The upload path for addressables is defined by the Addressables Profiles window. Various input and output paths are displayed in the Info panel of this window. 
    [InitializeOnLoad]
    public class BuildPipelineEditorWindow : EditorWindow, IPreprocessBuildWithReport, IPostprocessBuildWithReport {
        [System.Serializable]
        public class BuildPipelineEditorWindowSettings : SerializedScriptableSingleton<BuildPipelineEditorWindowSettings> {
            public Vector2 scrollPos;

            public bool showAdvancedSettings = true;
            public bool settingsExpanded = true;
            public bool commandsExpanded = false;
            public bool infoExpanded = false;
            public bool buildExpanded = true;

            // public OptionsMode optionsMode = OptionsMode.ReleaseFullBuild;
            // public enum OptionsMode {
            //     ReleaseFullBuild,
            //     ReleaseContentUpdate,
            //     DebugFullBuild,
            //     DebugContentUpdate,
            //     Custom
            // }
            public S3IAMKeyParams awsKeys;

            [System.Serializable]
            public class BuildPipelineSettings {
                public SetPlatformStep setPlatformStep = new SetPlatformStep();

                public SetBuildVersionStep setBuildVersionStep = new SetBuildVersionStep();

                // public BuildAddressablesStep buildAddressablesStep = new BuildAddressablesStep();
                // public UploadAddressablesStep uploadAddressablesStep = new UploadAddressablesStep();
                public CreateBuildStep createBuildStep = new CreateBuildStep();
                public UploadToAwsBuildStep uploadToAwsBuildStep = new UploadToAwsBuildStep();
                public RunBuildStep runBuildStep = new RunBuildStep();

                public IEnumerable<BuildPipelineStep> buildSteps {
                    get {
                        yield return setPlatformStep;
                        yield return setBuildVersionStep;
                        // yield return buildAddressablesStep;
                        // yield return uploadAddressablesStep;
                        yield return createBuildStep;
                        yield return uploadToAwsBuildStep;
                        yield return runBuildStep;
                    }
                }

                public BuildPipelineSettings() {
                }
                // public BuildPipelineSettings (BuildPipelineSettings toClone) {
                // pipelineSteps = toClone.pipelineSteps;
                // forceNewAddressablesBuild = toClone.forceNewAddressablesBuild;
                // caching = toClone.caching;
                // development = toClone.development;
                // compression = toClone.compression;
                // autoConnectProfiler = toClone.autoConnectProfiler;
                // }
            }

            public BuildPipelineSettings settings {
                get {
                    // if(optionsMode == OptionsMode.ReleaseFullBuild) return releaseFullBuildSettings;
                    // else if(optionsMode == OptionsMode.ReleaseContentUpdate) return releaseContentUpdateSettings;
                    // else if(optionsMode == OptionsMode.DebugFullBuild) return debugFullBuildSettings;
                    // else if(optionsMode == OptionsMode.DebugContentUpdate) return debugContentUpdateSettings;
                    // else 
                    return customSettings;
                }
            }

            public BuildPipelineSettings _customSettings;

            public BuildPipelineSettings customSettings {
                get {
                    if (_customSettings == null) {
                        _customSettings = new BuildPipelineSettings();
                    }

                    return _customSettings;
                }
            }

            static BuildPipelineSettings _standardSettings;
            // public static BuildPipelineSettings releaseFullBuildSettings {
            //     get {
            //         if(_standardSettings == null) {
            //             _standardSettings = new BuildPipelineSettings();
            //             _standardSettings.pipelineSteps = (PipelineSteps)~0;
            //             _standardSettings.forceNewAddressablesBuild = true;
            //             _standardSettings.caching = true;
            //             _standardSettings.compression = false;
            //             _standardSettings.development = false;
            //             _standardSettings.autoConnectProfiler = false;
            //         }
            //         return _standardSettings;
            //     }
            // }
            // static BuildPipelineSettings _standardContentUpdateSettings;
            // public static BuildPipelineSettings releaseContentUpdateSettings {
            //     get {
            //         if(_standardContentUpdateSettings == null) {
            //             _standardContentUpdateSettings = new BuildPipelineSettings(releaseFullBuildSettings);
            //             _standardContentUpdateSettings.pipelineSteps = PipelineSteps.BuildAddressables | PipelineSteps.UploadAddressables;
            //         }
            //         return _standardContentUpdateSettings;
            //     }
            // }
            // static BuildPipelineSettings _debugSettings;
            // public static BuildPipelineSettings debugFullBuildSettings {
            //     get {
            //         if(_debugSettings == null) {
            //             _debugSettings = new BuildPipelineSettings();
            //             _debugSettings.pipelineSteps = (PipelineSteps)~0;
            //             _debugSettings.forceNewAddressablesBuild = false;
            //             _debugSettings.caching = true;
            //             _debugSettings.compression = false;
            //             _debugSettings.development = true;
            //             _debugSettings.autoConnectProfiler = true;
            //         }
            //         return _debugSettings;
            //     }
            // }
            // static BuildPipelineSettings _debugContentUpdateSettings;
            // public static BuildPipelineSettings debugContentUpdateSettings {
            //     get {
            //         if(_debugContentUpdateSettings == null) {
            //             _debugContentUpdateSettings = new BuildPipelineSettings(debugFullBuildSettings);
            //             _debugContentUpdateSettings.pipelineSteps = PipelineSteps.BuildAddressables | PipelineSteps.UploadAddressables;
            //         }
            //         return _debugContentUpdateSettings;
            //     }
            // }


        }

        public static ProfileAndBucketProperties activeProfileProperties => new ProfileAndBucketProperties(
            BuildPipelineEditorWindowSettings.Instance.awsKeys.accessKey,
            BuildPipelineEditorWindowSettings.Instance.awsKeys.secretKey,
            S3CannedACL.PublicRead,
            "tomsgames",
            "eu-central-1"
        );
        // public static ProfileAndBucketProperties activeProfileProperties => profileAndBucketProperties.FirstOrDefault(p => AddressableAssetSettingsDefaultObject.Settings.activeProfileId == p.profileID);


        public int callbackOrder {
            get { return 0; }
        }

        public void OnPreprocessBuild(BuildReport report) {
            // Debug.Log("PREPROCESSBUILD");
            BuildInfo.Instance.UpdateCurrentVersion();
            EditorUtility.SetDirty(BuildInfo.Instance);
            BuildInfo.Instance.ApplyVersionToPlayerSettings();
            AssetDatabase.SaveAssets();
        }

        public void OnPostprocessBuild(BuildReport report) {
            // Debug.Log("POSTPROCESSBUILD");
        }

        public static string defaultDir {
            get { return Path.GetFullPath(Path.Combine(Application.dataPath, "../Builds")); }
        }

        static bool runningPipeline;

        public static ServerHostedFileWindow addressablesUploaderWindow;


        /*
        #region Profile/Bucket Properties
        static List<ProfileAndBucketProperties> _profileAndBucketProperties;
        public static List<ProfileAndBucketProperties> profileAndBucketProperties {
            get {
                if(_profileAndBucketProperties == null) {
                    _profileAndBucketProperties = new List<ProfileAndBucketProperties>();
                    _profileAndBucketProperties.Add(new ProfileAndBucketProperties(BuildPipelineEditorWindowSettings.Instance.awsKeys.accessKey, BuildPipelineEditorWindowSettings.Instance.awsKeys.secretKey, "Development", "com.BetterUp.InkFramework", "Assets/Icons/Icon.psd", S3CannedACL.Private));
                    _profileAndBucketProperties.Add(new ProfileAndBucketProperties(BuildPipelineEditorWindowSettings.Instance.awsKeys.accessKey, BuildPipelineEditorWindowSettings.Instance.awsKeys.secretKey, "Staging", "com.BetterUp.InkFramework", "Assets/Icons/Icon.psd", S3CannedACL.Private));
                    _profileAndBucketProperties.Add(new ProfileAndBucketProperties(BuildPipelineEditorWindowSettings.Instance.awsKeys.accessKey, BuildPipelineEditorWindowSettings.Instance.awsKeys.secretKey, "Production", "com.BetterUp.InkFramework", "Assets/Icons/Icon.psd", S3CannedACL.Private));
                    // _profileAndBucketProperties.Add(new ProfileAndBucketProperties(BuildPipelineEditorWindowSettings.Instance.legacyAWSKeys.accessKey, BuildPipelineEditorWindowSettings.Instance.legacyAWSKeys.secretKey, "Live", "com.BetterUp.InkFramework", "Assets/Icons/Icon.psd", S3CannedACL.PublicRead));
                    // _profileAndBucketProperties.Add(new ProfileAndBucketProperties(BuildPipelineEditorWindowSettings.Instance.legacyAWSKeys.accessKey, BuildPipelineEditorWindowSettings.Instance.legacyAWSKeys.secretKey, "A", "com.BetterUp.InkFrameworkA", "Assets/Icons/Icon A.psd", S3CannedACL.PublicRead));
                    // _profileAndBucketProperties.Add(new ProfileAndBucketProperties(BuildPipelineEditorWindowSettings.Instance.legacyAWSKeys.accessKey, BuildPipelineEditorWindowSettings.Instance.legacyAWSKeys.secretKey, "B", "com.BetterUp.InkFrameworkB", "Assets/Icons/Icon B.psd", S3CannedACL.PublicRead));
                    // _profileAndBucketProperties.Add(new ProfileAndBucketProperties(BuildPipelineEditorWindowSettings.Instance.legacyAWSKeys.accessKey, BuildPipelineEditorWindowSettings.Instance.legacyAWSKeys.secretKey, "C", "com.BetterUp.InkFrameworkC", "Assets/Icons/Icon C.psd", S3CannedACL.PublicRead));
                    // _profileAndBucketProperties.Add(new ProfileAndBucketProperties(BuildPipelineEditorWindowSettings.Instance.legacyAWSKeys.accessKey, BuildPipelineEditorWindowSettings.Instance.legacyAWSKeys.secretKey, "D", "com.BetterUp.InkFrameworkD", "Assets/Icons/Icon D.psd", S3CannedACL.PublicRead));
                    // _profileAndBucketProperties.Add(new ProfileAndBucketProperties(BuildPipelineEditorWindowSettings.Instance.legacyAWSKeys.accessKey, BuildPipelineEditorWindowSettings.Instance.legacyAWSKeys.secretKey, "AddressablesTests", "com.BetterUp.BUInkTesting", "Assets/Icons/Icon Test.psd", S3CannedACL.PublicRead));
                }
                return _profileAndBucketProperties;
            }
        }
        */


        public static string GetAppNameForServer() {
            return new string(Application.productName.ToCharArray()
                .Select(c => {
                    c = Char.ToLower(c);
                    if (Char.IsWhiteSpace(c)) c = '_';
                    return c;
                })
                .ToArray());
        }

        public static string GetBuildTargetString(BuildTarget buildTarget) {
            return buildTarget.ToString().ToLower();
        }

        public class ProfileAndBucketProperties {
            // The name of the profile as specified in Unity.
            public string iamAccessKeyId;
            public string iamSecretKey;

            public string bucketName;
            public string regionString;


            public string localBuildOutputPath => GetBuildOutputPath(bucketName);
            // public string remoteBucketFolderRoot => AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetValueByName(profileID, "BucketRelativePath");

            // Permissions - this only works when ACLs are enabled
            public S3CannedACL permissions;

            public ProfileAndBucketProperties(string iamAccessKeyId, string iamSecretKey, S3CannedACL permissions, string bucketName, string regionString) {
                this.iamAccessKeyId = iamAccessKeyId;
                this.iamSecretKey = iamSecretKey;
                this.permissions = permissions;
                this.bucketName = bucketName;
                this.regionString = regionString;
            }

            public AmazonS3Client CreateClient() {
                var credentials = new BasicAWSCredentials(iamAccessKeyId, iamSecretKey);
                var bucketRegion = RegionEndpoint.GetBySystemName(regionString);
                var s3Config = new AmazonS3Config {
                    RegionEndpoint = bucketRegion,
                    // Timeout = TimeSpan.FromSeconds(timeoutSeconds),
                    // NOTE: The following property is obsolete for
                    //       versions of the AWS SDK for .NET that target .NET Core.
                    // ReadWriteTimeout = TimeSpan.FromSeconds(timeoutSeconds),
                    // RetryMode = RequestRetryMode.Standard,
                    // MaxErrorRetry = 3
                };
                // s3Config.DisableLogging = false;
                // s3Config.LogResponse = true;
                // s3Config.FastFailRequests = true;
                return new AmazonS3Client(credentials, s3Config);
            }


            // public string GetFormattedServerPath (string buildTarget) {
            //     return GetAppName().Replace("[BuildTarget]", buildTarget);
            // }

            public string GetAwsConsoleURL(string relativePath) {
                return "https://s3.console.aws.amazon.com/s3/buckets/" + bucketName + "?region=" + regionString + "&prefix=" + relativePath + "&showversions=false";
            }

            public string GetBuildURL(string relativePath) {
                return "https://" + bucketName + ".s3." + regionString + ".amazonaws.com/" + relativePath;
            }

            
            // public string GetFormattedServerAddressablesPath (string buildTarget, string filePath) {
            //     return AddressableAssetSettingsDefaultObject.Settings.profileSettings.EvaluateString(profileID, remoteAddressablesPath.Replace("[BuildTarget]", buildTarget).Replace("(FilePath)", filePath));
            // }

            // Gets the path that Addressables should build the addressable state .bin file to.
            // public string GetTargetAddressablesOutputBinPath () {
            //     return Path.Combine(Application.dataPath, "AddressableAssetsData", "Content States", bucketName, PlatformMappingService.GetPlatformPathSubFolder(), "addressables_content_state.bin").Replace("\\","/");
            // }



            // Gets the build output path
            // On Windows we build an extra folder to contain the build, since additional files are placed in the same folder.
            // On OSX the .app is basically a folder in itself and on other platforms builds output to a folder
            public string GetBuildDirectoryPath(string versionString, BuildTarget buildTarget) {
                string buildDirectoryPath = string.Empty;
                buildDirectoryPath = defaultDir;
                // buildDirectoryPath = Path.Combine(buildDirectoryPath, bucketName);
                buildDirectoryPath = Path.Combine(buildDirectoryPath, versionString);
                buildDirectoryPath = Path.GetFullPath(Path.Combine(buildDirectoryPath, buildTarget.ToString()));

                return buildDirectoryPath.Replace("\\", "/");
            }

            public string GetBuildPath(string versionString, BuildTarget buildTarget) {
                string buildDirectoryPath = GetBuildDirectoryPath(versionString, buildTarget);
                string buildPath = string.Empty;
                var appName = PlayerSettings.productName.Replace(" ", "_");
                if (buildTarget == BuildTarget.StandaloneWindows || buildTarget == BuildTarget.StandaloneWindows64) {
                    buildPath = Path.Combine(buildDirectoryPath, appName, Path.ChangeExtension(appName, ".exe"));
                } else if (buildTarget == BuildTarget.StandaloneOSX) {
                    buildPath = Path.Combine(buildDirectoryPath, Path.ChangeExtension(appName, ".app"));
                } else if (buildTarget == BuildTarget.WebGL) {
                    buildPath = buildDirectoryPath;
                } else {
                    buildPath = Path.Combine(buildDirectoryPath, appName);
                }

                return buildPath.Replace("\\", "/");
            }



            // Utility to get a path relative to another path
            public static string GetRelativePath(string path, string rootPath) {
                return path.Substring(rootPath.Length).Replace("\\", "/");
            }

            // Gets the path of the ServerData folder that a Addressable builds for each platform are copied into. 
            public static string GetBuildOutputPath(string bucketName) {
                return Path.Combine(Directory.GetCurrentDirectory(), "ServerData", bucketName).Replace("\\", "/") + "/";
            }

            // Gets the path of the ServerData folder that an Addressable build for a given platform is copied into. 
            public static string GetBuildOutputPathForPlatform(string bucketName, string platform) {
                return Path.Combine(Directory.GetCurrentDirectory(), "ServerData", bucketName, platform).Replace("\\", "/") + "/";
            }
        }




        [MenuItem("Build/Build Pipeline", false, 2400)]
        static void Init() {
            BuildPipelineEditorWindow window = EditorWindow.GetWindow(typeof(BuildPipelineEditorWindow), false, "Build Pipeline") as BuildPipelineEditorWindow;
            window.titleContent = new GUIContent("Build Pipeline");
        }

        void OnFocus() {
            BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.ResetSettingsForActivePlatform();
        }

        void OnGUI() {
            /*
            // This can be null for the first few frames of the editor loading up.
            if(AddressableAssetSettingsDefaultObject.Settings == null) return;
    
            // Ensure we have a valid profile selected.
            if(activeProfileProperties == null) AddressableAssetSettingsDefaultObject.Settings.activeProfileId = profileAndBucketProperties.First().profileID;
            */

            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 200;

            BuildPipelineEditorWindowSettings.Instance.scrollPos = EditorGUILayout.BeginScrollView(BuildPipelineEditorWindowSettings.Instance.scrollPos);
            if (BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform == null)
                BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.ResetSettingsForActivePlatform();

            // BuildPipelineEditorWindowSettings.Instance.showAdvancedSettings = EditorGUILayout.Toggle("Show Advanced Dev Settings", BuildPipelineEditorWindowSettings.Instance.showAdvancedSettings);

            EditorGUI.BeginDisabledGroup(runningPipeline);

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            BuildPipelineEditorWindowSettings.Instance.settingsExpanded = EditorGUILayout.Foldout(BuildPipelineEditorWindowSettings.Instance.settingsExpanded, "Settings", true);
            EditorGUILayout.EndHorizontal();
            if (BuildPipelineEditorWindowSettings.Instance.settingsExpanded) {
                EditorGUI.indentLevel++;



                {
                    // Bucket
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    // GUILayout.Label(new GUIContent("Bucket", "Which AWS bucket this build is placed into. A bucket can currently only hold one build, so building will replace the old one."), EditorStyles.miniBoldLabel);
                    // GUILayout.BeginHorizontal();
                    // for (int i = 0; i < profileAndBucketProperties.Count; i++) {
                    //     ProfileAndBucketProperties p = profileAndBucketProperties[i];
                    //     if (GUILayout.Toggle(AddressableAssetSettingsDefaultObject.Settings.activeProfileId == p.profileID, p.profileName, MiniButtonGUI(i, profileAndBucketProperties.Count))) {
                    //         AddressableAssetSettingsDefaultObject.Settings.activeProfileId = p.profileID;
                    //     }
                    // }
                    // GUILayout.EndHorizontal();

                    bool DrawAWSKeys(ref S3IAMKeyParams keyParams, string name, string errorMessage) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(name, GUILayout.Width(160));
                        keyParams.accessKey = EditorGUILayout.TextField(keyParams.accessKey, GUILayout.Width(190));
                        keyParams.secretKey = EditorGUILayout.TextField(keyParams.secretKey, GUILayout.Width(340));
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Copy to clipboard", GUILayout.Width(140))) {
                            GUIUtility.systemCopyBuffer = JsonUtility.ToJson(keyParams);
                        }

                        if (GUILayout.Button("Paste from clipboard", GUILayout.Width(140))) {
                            keyParams = JsonUtility.FromJson<S3IAMKeyParams>(GUIUtility.systemCopyBuffer);
                        }

                        EditorGUILayout.EndHorizontal();
                        EditorGUI.BeginChangeCheck();

                        if (keyParams.isUndefined) EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
                        return EditorGUI.EndChangeCheck();
                    }

                    if (DrawAWSKeys(ref BuildPipelineEditorWindowSettings.Instance.awsKeys, "AWS Keys", "Please enter the AWS API credentials in the fields above."))
                        BuildPipelineEditorWindowSettings.Save();
                    EditorGUILayout.EndVertical();

                }

                // { // Mode
                //     EditorGUILayout.BeginVertical(GUI.skin.box);
                //     GUILayout.Label("Mode", EditorStyles.miniBoldLabel);
                //
                //     GUILayout.BeginHorizontal();
                //     var length = Enum.GetNames(typeof(BuildPipelineEditorWindowSettings.OptionsMode)).Length;
                //     if(!BuildPipelineEditorWindowSettings.Instance.showAdvancedSettings) {
                //         length--;
                //         if((int)BuildPipelineEditorWindowSettings.Instance.optionsMode >= length) BuildPipelineEditorWindowSettings.Instance.optionsMode = BuildPipelineEditorWindowSettings.OptionsMode.ReleaseFullBuild;
                //     }
                //     for (int i = 0; i < length; i++) {
                //         if (GUILayout.Toggle((int)BuildPipelineEditorWindowSettings.Instance.optionsMode == i, ((BuildPipelineEditorWindowSettings.OptionsMode)i).ToString(), BuildPipelineUtils.MiniButtonGUI(i, length))) {
                //             BuildPipelineEditorWindowSettings.Instance.optionsMode = (BuildPipelineEditorWindowSettings.OptionsMode)i;
                //         }
                //     }
                //     GUILayout.EndHorizontal();
                //     if(BuildPipelineEditorWindowSettings.Instance.optionsMode == BuildPipelineEditorWindowSettings.OptionsMode.DebugFullBuild || BuildPipelineEditorWindowSettings.Instance.optionsMode == BuildPipelineEditorWindowSettings.OptionsMode.DebugContentUpdate) {
                //         EditorGUILayout.HelpBox("Debug builds are for testing and should never be pushed to production", MessageType.Info);
                //     }
                //     EditorGUILayout.EndVertical();
                // }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.Foldout(true, new GUIContent("Build Steps"), true, EditorStyles.foldout);
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            foreach (var buildStep in BuildPipelineEditorWindowSettings.Instance.settings.buildSteps) {
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                EditorGUI.BeginDisabledGroup(!buildStep.canBeDisabled);
                if (buildStep.canBeDisabled)
                    buildStep.enabled = EditorGUILayout.Foldout(buildStep.enabled, new GUIContent(buildStep.name), true, EditorStyles.toggle);
                else
                    EditorGUILayout.Foldout(true, new GUIContent(buildStep.name), true, EditorStyles.toggle);
                EditorGUI.EndDisabledGroup();
                // buildStep.enabled = EditorGUILayout.Toggle(buildStep.enabled);
                EditorGUILayout.EndHorizontal();
                if (buildStep.expandedInSettings) {
                    EditorGUI.BeginDisabledGroup(!buildStep.enabled);
                    EditorGUI.indentLevel++;
                    buildStep.DrawSettings();
                    EditorGUI.indentLevel--;
                    EditorGUI.EndDisabledGroup();
                }
            }

            EditorGUI.indentLevel--;

            if (BuildPipelineEditorWindowSettings.Instance.showAdvancedSettings) {

                {
                    // Commands
                    EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                    BuildPipelineEditorWindowSettings.Instance.commandsExpanded = EditorGUILayout.Foldout(BuildPipelineEditorWindowSettings.Instance.commandsExpanded, "Commands", true);
                    EditorGUILayout.EndHorizontal();
                    if (BuildPipelineEditorWindowSettings.Instance.commandsExpanded) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Open uploader tool")) {
                            if (addressablesUploaderWindow == null) addressablesUploaderWindow = ServerHostedFileWindow.Init();
                            // addressablesUploaderWindow.SetFiles(activeProfileProperties, fileStatuses);
                            addressablesUploaderWindow.Show();
                        }

                        // if(GUILayout.Button("Delete")) {
                        //     PerformDeleteTests();
                        // }
                        // if(GUILayout.Button("Upload Dir")) {
                        //     UploadDirectoryTest();
                        // }
                        // if(GUILayout.Button("Upload Info File")) {
                        //     UploadInfoFileTest();
                        // }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel--;
                    }
                }

                {
                    // Info
                    EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                    BuildPipelineEditorWindowSettings.Instance.infoExpanded = EditorGUILayout.Foldout(BuildPipelineEditorWindowSettings.Instance.infoExpanded, "Info", true);
                    EditorGUILayout.EndHorizontal();
                    if (BuildPipelineEditorWindowSettings.Instance.infoExpanded) {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        
                        var buildTargetString = GetBuildTargetString(BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget);
                        GetServerRelativeBuildZipPath();

                        if (BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget == BuildTarget.WebGL) {
                            DrawURLPath(new GUIContent("Latest Build URL", "Path to the latest playable build"), activeProfileProperties.GetBuildURL(GetRelativeServerPath("current", buildTargetString, "index.html")));
                            DrawURLPath(new GUIContent("Version Build URL", "Path to the latest playable build"), activeProfileProperties.GetBuildURL(GetRelativeServerPath(BuildInfo.Instance.version.ToString(), buildTargetString, "index.html")));
                        } else {
                            DrawURLPath(new GUIContent("Latest Build Zip", "Path to the latest playable build"), activeProfileProperties.GetBuildURL(GetRelativeServerPath("current", buildTargetString, GetZipFileName())));
                            DrawURLPath(new GUIContent("Version Build Zip", "Path to the latest playable build"), activeProfileProperties.GetBuildURL(GetRelativeServerPath(BuildInfo.Instance.version.ToString(), buildTargetString, GetZipFileName())));
                        }

                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.EnumPopup("Build Target", BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget);
                        EditorGUILayout.TextField("Bucket", activeProfileProperties.bucketName);
                        EditorGUILayout.TextField("Relative Remote Path", GetRelativeServerPath(BuildInfo.Instance.version.ToString(), buildTargetString, "example.txt"));
                        EditorGUI.EndDisabledGroup();

                        EditorGUILayout.Space(2);

                        var consoleURL = "https://s3.console.aws.amazon.com/s3/buckets/" + activeProfileProperties.bucketName + "?region=" + activeProfileProperties.regionString + "&prefix=" + GetRelativeServerPath(BuildInfo.Instance.version.ToString(), buildTargetString, string.Empty) + "&showversions=false";
                        DrawURLPath(new GUIContent("Remote Path Console URL", "Path to the AWS console dashboard for the latest build"), consoleURL);

                        EditorGUILayout.Space(2);

                        // DrawPath(new GUIContent(".bin path", "Path to the content state for this bucket/platform. This is required for updating existing addressables, and is stored in source control."), activeProfileProperties.GetTargetAddressablesOutputBinPath());
                        // DrawPath(new GUIContent("Built addressables path", "This is where addressables are uploaded from"), ProfileAndBucketProperties.GetBuildOutputPathForPlatform(activeProfileProperties.bucketName, buildTargetString));
                        DrawPath(new GUIContent("Build path", "Path to the build"), activeProfileProperties.GetBuildPath(BuildInfo.Instance.version.ToString(), BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget) + "/");

                        EditorGUILayout.EndVertical();
                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUI.EndDisabledGroup();
            }



            // EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            BuildPipelineEditorWindowSettings.Instance.buildExpanded = EditorGUILayout.Foldout(BuildPipelineEditorWindowSettings.Instance.buildExpanded, "Build", true);
            EditorGUILayout.EndHorizontal();
            if (BuildPipelineEditorWindowSettings.Instance.buildExpanded) {
                // EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(!runningPipeline);
                EditorGUI.EndDisabledGroup();

                if (!runningPipeline) {
                    EditorGUI.BeginDisabledGroup(runningPipeline);
                    var col = GUI.backgroundColor;
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Build", GUILayout.Height(40))) {
                        string warning = null;
                        // if(BuildPipelineEditorWindowSettings.Instance.settings.development && (activeProfileProperties.profileName == "Staging" || activeProfileProperties.profileName == "Production")) {
                        //     warning = "You are attempting to build a development build to "+activeProfileProperties.profileName+". Are you sure you want to do this?";
                        // } else if(activeProfileProperties.profileName == "Production") {
                        //     warning = "You are attempting to build to "+activeProfileProperties.profileName+". Are you sure you want to do this?";
                        // }
                        if (warning == null || EditorUtility.DisplayDialog("Are you sure you want to build?", warning, "Yes", "No")) {
                            PerformBuild();
                        }
                    }

                    GUI.backgroundColor = col;
                }

                EditorGUI.EndDisabledGroup();
                // if(GUILayout.Button("Test")) {
                //     UploadDirectory(activeProfileProperties, activeProfileProperties.GetBuildPath(buildTarget)+"/build", activeProfileProperties.GetFormattedServerPath(buildTarget.ToString())+"/build");
                //     // Debug.Log(activeProfileProperties.GetBuildPath(buildTarget)+"/Build");
                //     // Debug.Log(activeProfileProperties.GetFormattedServerPath(buildTarget.ToString())+"/build");
                // }
                // EditorGUI.indentLevel--;

                if (runningPipeline) {
                    // if(GUILayout.Button("Cancel")) {
                    //     Cancel(BuildPipelineEditorWindowSettings.Instance.settings.uploadToAwsBuildStep.state == BuildStageProgressTracker.State.Working);
                    // }

                    EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandHeight(true));
                    EditorGUILayout.LabelField("Build in progress!", EditorStyles.centeredGreyMiniLabel);
                    EditorGUI.BeginDisabledGroup(!runningPipeline);

                    foreach (var buildStep in BuildPipelineEditorWindowSettings.Instance.settings.buildSteps) {
                        if (buildStep.enabled) {
                            buildStep.DrawProgress();
                        }
                    }

                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space(10);
            }

            BuildPipelineEditorWindowSettings.Save();

            Repaint();

            EditorGUIUtility.labelWidth = labelWidth;
        }

        static void Cancel(bool recompile) {
            runningPipeline = false;
            if (recompile)
                UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }





        void DrawURLPath(GUIContent label, string path) {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(label, path);
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("Copy", GUILayout.Width(42))) GUIUtility.systemCopyBuffer = path;
            if (GUILayout.Button(">", GUILayout.Width(22))) Application.OpenURL(path);
            EditorGUILayout.EndHorizontal();
        }

        void DrawPath(GUIContent label, string path) {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(label, path);
            EditorGUI.EndDisabledGroup();

            path = Path.GetDirectoryName(path);
            EditorGUI.BeginDisabledGroup(!Directory.Exists(path));
            if (GUILayout.Button(">", GUILayout.Width(22))) {
                EditorUtility.RevealInFinder(path);
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
        }

        static async void PerformBuild() {
            foreach (var buildStep in BuildPipelineEditorWindowSettings.Instance.settings.buildSteps) {
                buildStep.BeginRunPipeline();
            }

            runningPipeline = true;

            BuildPipelineEditorWindowSettings.Instance.settings.setBuildVersionStep.SetVersion();
            BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.SwitchToTargetPlatform();


            // Apply some settings before we start the build
            // changingTargetLabel = "Applying build settings...";
            // changingTargetProgress = 0;
            BuildPipelineEditorWindowSettings.Instance.settings.createBuildStep.ApplyBuildParams(BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform);
            // changingTargetLabel = "Applied build settings!";
            // changingTargetProgress = 1;


            // Save
            AssetDatabase.SaveAssets();
            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();

#if ADDRESSABLES
        if(BuildPipelineEditorWindowSettings.Instance.settings.buildAddressablesStep.enabled) {
            // Build addressables
            if(!UpdateAddressables()) {
                Cancel(false);
                Debug.LogError("Something went wrong while updating addressables and build was cancelled!");
                return;
            }
        } else
#endif

#if ADDRESSABLES
        if(BuildPipelineEditorWindowSettings.Instance.settings.uploadAddressablesStep.enabled) {
            // Upload addressables
            await UploadAddressables();
        } else
#endif

            if (BuildPipelineEditorWindowSettings.Instance.settings.createBuildStep.enabled) {
                // Build the app
                if (!BuildPipelineEditorWindowSettings.Instance.settings.createBuildStep.Build(activeProfileProperties, BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget, BuildPipelineEditorWindowSettings.Instance.settings.runBuildStep.enabled, BuildPipelineEditorWindowSettings.Instance.settings.runBuildStep.autoConnectProfiler)) {
                    Cancel(false);
                    return;
                }
            }

            if (BuildPipelineEditorWindowSettings.Instance.settings.uploadToAwsBuildStep.enabled) {
                await PostBuild();
            }

            runningPipeline = false;
        }




        static List<ServerHostedFileStatus> GetFileStatuses(FileInfo[] files, string bucketPlatformAddressablesFolderPath, string buildTargetPath) {
            List<ServerHostedFileStatus> fileStatuses = new List<ServerHostedFileStatus>();
            foreach (var fileInfo in files) {
                var relativePath = ProfileAndBucketProperties.GetRelativePath(fileInfo.FullName, bucketPlatformAddressablesFolderPath);
                var currentlyUploadingFileStatus = new ServerHostedFileStatus(fileInfo, buildTargetPath, relativePath);

                fileStatuses.Add(currentlyUploadingFileStatus);
            }

            return fileStatuses;
        }



        static async Task PostBuild() {
            if (BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform == null)
                BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.ResetSettingsForActivePlatform();
            var outputPath = activeProfileProperties.GetBuildPath(BuildInfo.Instance.version.ToString(), BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget);

            // Create an info text file.
            var infoTextPath = CreateInfoTextFile(outputPath, BuildInfo.Instance);

            //  WebGL
            if (BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget == BuildTarget.WebGL) {
                // Upload entire build
                var filesToUpload = BuildPipelineEditorWindowSettings.Instance.settings.uploadToAwsBuildStep.GetFilesToUploadForWebGL(activeProfileProperties.CreateClient(), BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform, activeProfileProperties);
                await BuildPipelineEditorWindowSettings.Instance.settings.uploadToAwsBuildStep.UploadFiles(activeProfileProperties.CreateClient(), filesToUpload, BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform, activeProfileProperties);

                // Copy it into the "current" directory 
                if (BuildPipelineEditorWindowSettings.Instance.settings.uploadToAwsBuildStep.copyIntoCurrentDirectory)
                    await BuildPipelineEditorWindowSettings.Instance.settings.uploadToAwsBuildStep.CopyBuildAsLatest(activeProfileProperties.CreateClient(), activeProfileProperties, GetBuildTargetString(BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget));

                // Run the build that's been uploaded to the server.
                if (BuildPipelineEditorWindowSettings.Instance.settings.runBuildStep.enabled) {
                    var buildURL = activeProfileProperties.GetBuildURL(GetRelativeServerPath("current", GetBuildTargetString(BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget), "index.html"));
                    Application.OpenURL(buildURL);
                }
            } else {
                var zipDirectoryPath = activeProfileProperties.GetBuildDirectoryPath(BuildInfo.Instance.version.ToString(), BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget);
                var buildDirectoryPath = activeProfileProperties.GetBuildPath(BuildInfo.Instance.version.ToString(), BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget);
                var zipFileName = GetAppNameForServer() + "_" + BuildInfo.Instance.version.ToString() + ".zip";
                var outputZipFilePath = Path.Combine(zipDirectoryPath, zipFileName);
                if (File.Exists(outputZipFilePath)) File.Delete(outputZipFilePath);
                System.IO.Compression.ZipFile.CreateFromDirectory(buildDirectoryPath, outputZipFilePath);

                var buildTargetString = GetBuildTargetString(BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget);
                ServerHostedFileStatus buildFileStatus = new ServerHostedFileStatus(new FileInfo(outputZipFilePath), buildTargetString, GetServerRelativeBuildZipPath());
                // ServerHostedFileStatus infoFileStatus = new ServerHostedFileStatus(new FileInfo(outputZipFilePath), GetBuildTargetString(BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget), );
                // var filesToUpload = BuildPipelineEditorWindowSettings.Instance.settings.uploadToAwsBuildStep.GetFilesToUploadForWebGL(activeProfileProperties.CreateClient(), BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform, activeProfileProperties);
                await BuildPipelineEditorWindowSettings.Instance.settings.uploadToAwsBuildStep.UploadFiles(activeProfileProperties.CreateClient(), new List<ServerHostedFileStatus>() {buildFileStatus /*, infoFileStatus*/}, BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform, activeProfileProperties);

                // Copy it into the "current" directory 
                if (BuildPipelineEditorWindowSettings.Instance.settings.uploadToAwsBuildStep.copyIntoCurrentDirectory)
                    await BuildPipelineEditorWindowSettings.Instance.settings.uploadToAwsBuildStep.CopyBuildAsLatest(activeProfileProperties.CreateClient(), activeProfileProperties, GetBuildTargetString(BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget));
            }
        }
        
        public static string GetRelativeServerPath(string versionString, string buildTargetString, string filePath = null) {
            var path = GetAppNameForServer();
            if (versionString != null) path += "/" + versionString;
            if (buildTargetString != null) path += "/" + buildTargetString;
            if (filePath != null) path += "/" + filePath;
            return path;
            // return AddressableAssetSettingsDefaultObject.Settings.profileSettings.EvaluateString(profileID, remoteAddressablesPath.Replace("[BuildTarget]", buildTarget).Replace("(FilePath)", filePath));
        }

        static string GetZipFileName() {
            return GetAppNameForServer() + "_" + BuildInfo.Instance.version.ToString() + ".zip";
        }

        static string GetServerRelativeBuildZipPath() {
            var zipFileName = GetAppNameForServer() + "_" + BuildInfo.Instance.version.ToString() + ".zip";
            var buildTargetString = GetBuildTargetString(BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget);
            return GetRelativeServerPath(BuildInfo.Instance.version.ToString(), buildTargetString, GetZipFileName());
        }



        static string CreateInfoTextFile(string buildOutputPath, BuildInfo buildInfo) {
            if (!Directory.Exists(buildOutputPath)) {
                Debug.LogWarning("BuildPipelineEditorWindow.CreateInfoTextFile: No folder exists at buildOutputPath " + buildOutputPath);
                return null;
            }

            var infoTextPath = Path.GetFullPath(Path.Combine(buildOutputPath, "Info.txt"));
            using (StreamWriter sw = File.CreateText(infoTextPath)) {
                sw.WriteLine("Build Info\n\n" + buildInfo.ToString());
            }

            return infoTextPath;
        }




        /*
        static async void PerformDeleteTests() {
            var formattedServerPath = GetRelativeServerPath(BuildInfo.Instance.version.ToString(), GetBuildTargetString(BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget));
            var targetPath = formattedServerPath + "/build";
            await AWSUtils.DeleteDirectoryAsync(activeProfileProperties.CreateClient(), activeProfileProperties.bucketName, targetPath);
        }

        static async void UploadDirectoryTest() {
            await AWSUtils.UploadDirectoryAsync(activeProfileProperties.CreateClient(), activeProfileProperties, activeProfileProperties.GetBuildPath(BuildInfo.Instance.version.ToString(), BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget), GetRelativeServerPath(BuildInfo.Instance.version.ToString(), GetBuildTargetString(BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget)) + "/build");
        }

        static async void UploadInfoFileTest() {
            var profileAndBucketProperties = activeProfileProperties;

            // var buildTargetPath = GetBuildTargetString(BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget);
            var outputPath = activeProfileProperties.GetBuildPath(BuildInfo.Instance.version.ToString(), BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget);
            var infoTextPath = Path.GetFullPath(Path.Combine(outputPath, "..", "Info.txt"));
            // var fileInfo = new FileInfo(infoTextPath);
            // var bucketPlatformAddressablesFolderPath = ProfileAndBucketProperties.GetBuildOutputPathForPlatform(activeProfileProperties.bucketName, buildTargetPath);
            // var relativePath = ProfileAndBucketProperties.GetRelativePath(fileInfo.FullName, bucketPlatformAddressablesFolderPath);
            var formattedServerPath = GetRelativeServerPath(BuildInfo.Instance.version.ToString(), GetBuildTargetString(BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget)) + "/Info.txt";

            var file = new FileInfo(infoTextPath);
            var fileStatus = new ServerHostedFileStatus(file, GetBuildTargetString(BuildPipelineEditorWindowSettings.Instance.settings.setPlatformStep.targetBuildPlatform.buildTarget), formattedServerPath);
            var uploadPath = formattedServerPath;
            // if(uploadPath == null) 
            // uploadPath = profileAndBucketProperties.GetRelativeServerPath(BuildInfo.Instance.version.ToString(), fileStatus.buildTarget, fileStatus.relativePath);
            var transferUtility = new TransferUtility(activeProfileProperties.CreateClient());
            fileStatus.uploadTaskProgress = new AWSTaskProgress();
            try {
                fileStatus.status = ServerHostedFileStatus.Status.Uploading;
                fileStatus.uploadTaskProgress.progress = 0;
                // var uploadStartTime = System.DateTime.Now;

                Debug.Log("Starting file upload from:\n" + fileStatus.fileInfo.FullName + "\nTo:\n" + profileAndBucketProperties.bucketName + ": " + uploadPath);
                var transferUtilityRequest = new TransferUtilityUploadRequest {
                    BucketName = profileAndBucketProperties.bucketName,
                    FilePath = fileStatus.fileInfo.FullName,
                    CannedACL = profileAndBucketProperties.permissions,
                    Key = uploadPath,
                    Headers = {
                        ContentEncoding = "br",
                        ContentType = "binary/octet-stream"
                    }
                };

                transferUtilityRequest.UploadProgressEvent += (object sender, UploadProgressArgs args) => {
                    fileStatus.uploadTaskProgress.progress = args.PercentDone / 100f;
                    fileStatus.uploadTaskProgress.bytesTransferred = args.TransferredBytes;
                    fileStatus.uploadTaskProgress.bytesTotal = args.TotalBytes;
                    // if(progressCallback != null) progressCallback(args);
                };

                await transferUtility.UploadAsync(transferUtilityRequest, fileStatus.uploadTaskProgress.cancellationTokenSource.Token);
                // var duration = System.DateTime.Now - uploadStartTime;
                // Debug.Log("File upload completed in "+duration+"! "+uploadPath);
                fileStatus.status = ServerHostedFileStatus.Status.Uploaded;
            } catch (AmazonS3Exception e) {
                // This didn't fire when the error
                // Unknown encountered on server when writing an object: Cannot access a disposed object.
                // occured
                fileStatus.status = ServerHostedFileStatus.Status.NotUploaded;
                Debug.LogError("AmazonS3Exception encountered on server when writing an object: " + e.Message);
                Debug.Log("fileStatus.uploadTaskProgress.cancellationTokenSource.IsCancellationRequested: " + fileStatus.uploadTaskProgress.cancellationTokenSource.IsCancellationRequested);
                fileStatus.uploadTaskProgress.cancellationTokenSource.Cancel();
                Debug.Log("fileStatus.uploadTaskProgress.cancellationTokenSource.IsCancellationRequested after manual Cancel: " + fileStatus.uploadTaskProgress.cancellationTokenSource.IsCancellationRequested);
            } catch (Exception e) {
                // This did fire when the error
                // Unknown encountered on server when writing an object: Cannot access a disposed object.
                // occured
                fileStatus.status = ServerHostedFileStatus.Status.NotUploaded;
                Debug.LogError("Exception encountered on server when writing an object: " + e.Message);
                fileStatus.uploadTaskProgress.cancellationTokenSource.Cancel();
                // This causes source.IsCancellationRequested to become true.
            }
            // if(source.IsCancellationRequested) {
            //     // Debug.Log("Should cancel or retry!");
            //     // await UploadFileAsync(transferUtility, profileAndBucketProperties, fileStatus);
            // }

            Debug.Log(activeProfileProperties.bucketName + ": " + formattedServerPath + "/Info.txt");
        }
        */
    }
}