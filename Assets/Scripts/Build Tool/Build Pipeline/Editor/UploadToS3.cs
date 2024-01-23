namespace BuildPipeline {
#pragma warning disable 4014
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Linq;
    using Amazon;
    using Amazon.Runtime;
    using UnityEditor;
    using UnityEngine;

    public class UploadToS3 : ServerHostedFileWindow {
        public class UploadToS3Settings : SerializedScriptableSingleton<UploadToS3Settings> {
            public string selectedBucketName = string.Empty;

            public bool bucketExpanded = true;
            public bool fileListExpanded = false;
            public Vector2 fileListPosition;

            public Platform fileListPlatformMask;
        }

        [Flags]
        public enum Platform {
            StandaloneWindows64 = 1 << 0,
            StandaloneOSX = 1 << 1,
            WebGL = 1 << 2,
            iOS = 1 << 3
        }



        [MenuItem("Tools/Addressable Content Uploader (S3)")]
        static new void Init() {
            var window = (UploadToS3) EditorWindow.GetWindow(typeof(UploadToS3));
            window.Show();
        }

        void OnEnable() {
            EditorApplication.delayCall += () => {
                ValidateSelectedBucket();
                BuildFileList();
                RecheckFileList();
            };
        }

        void ValidateSelectedBucket() {
            // profileAndBucketProperties = BuildPipelineEditorWindow.GetProfilePropertiesFromBucketName(UploadToS3Settings.Instance.selectedBucketName);
            // if(profileAndBucketProperties == null) {
            //     if(BuildPipelineEditorWindow.profileAndBucketProperties.Count > 0) {
            //         profileAndBucketProperties = BuildPipelineEditorWindow.profileAndBucketProperties.First();
            //         UploadToS3Settings.Instance.selectedBucketName = profileAndBucketProperties.bucketName;
            //     }
            // }
        }

        void BuildFileList() {
            allFileStatuses.Clear();
            string path = string.Empty;
            if (UploadToS3Settings.Instance.fileListPlatformMask.HasFlag(Platform.StandaloneWindows64)) {
                AddToFileList(Platform.StandaloneWindows64.ToString());
            }

            if (UploadToS3Settings.Instance.fileListPlatformMask.HasFlag(Platform.StandaloneOSX)) {
                AddToFileList(Platform.StandaloneOSX.ToString());
            }

            if (UploadToS3Settings.Instance.fileListPlatformMask.HasFlag(Platform.iOS)) {
                AddToFileList(Platform.iOS.ToString());
            }

            if (UploadToS3Settings.Instance.fileListPlatformMask.HasFlag(Platform.WebGL)) {
                AddToFileList(Platform.WebGL.ToString());
            }

            void AddToFileList(string platform) {
                var relativeFilePath = BuildPipelineEditorWindow.ProfileAndBucketProperties.GetBuildOutputPathForPlatform(UploadToS3Settings.Instance.selectedBucketName, platform);
                if (Directory.Exists(relativeFilePath)) {
                    var directory = new DirectoryInfo(relativeFilePath);

                    foreach (var fileInfo in directory.GetFiles("*", SearchOption.AllDirectories)) {
                        var relativePath = BuildPipelineEditorWindow.ProfileAndBucketProperties.GetRelativePath(fileInfo.FullName, relativeFilePath);
                        allFileStatuses.Add(new ServerHostedFileStatus(fileInfo, platform, relativePath));
                    }
                }
            }

            RefreshValidFileStatuses();
        }

        void OnInspectorUpdate() {
            Repaint();
        }

        void OnGUI() {
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            ValidateSelectedBucket();
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                UploadToS3Settings.Instance.bucketExpanded = EditorGUILayout.Foldout(UploadToS3Settings.Instance.bucketExpanded, "Buckets");
                EditorGUILayout.EndHorizontal();
                if (UploadToS3Settings.Instance.bucketExpanded) {
                    GUILayout.BeginHorizontal();
                    // for (int i = 0; i < BuildPipelineEditorWindow.profileAndBucketProperties.Count; i++) {
                    //     EditorGUI.BeginChangeCheck();
                    //     BuildPipelineEditorWindow.ProfileAndBucketProperties p = BuildPipelineEditorWindow.profileAndBucketProperties[i];
                    //     if (GUILayout.Toggle(profileAndBucketProperties == p, p.profileName, BuildPipelineEditorWindow.MiniButtonGUI(i, BuildPipelineEditorWindow.profileAndBucketProperties.Count))) {
                    //         UploadToS3Settings.Instance.selectedBucketName = p.bucketName;
                    //         profileAndBucketProperties = p;
                    //     }
                    //     if(EditorGUI.EndChangeCheck()) {
                    //         BuildFileList();
                    //         RecheckFileList();
                    //     }
                    // }
                    GUILayout.EndHorizontal();

                    if (profileAndBucketProperties != null) {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.TextField("Region", profileAndBucketProperties.regionString);
                        EditorGUILayout.TextField("Bucket name", profileAndBucketProperties.bucketName);
                        EditorGUI.EndDisabledGroup();

                        EditorGUILayout.Separator();


                        if (!Directory.Exists(profileAndBucketProperties.localBuildOutputPath)) {
                            EditorGUILayout.HelpBox("Folder at " + profileAndBucketProperties.localBuildOutputPath + " does not exist! Have addressables been built?", MessageType.Warning);
                        } else {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox(profileAndBucketProperties.localBuildOutputPath, MessageType.Info);
                            if (GUILayout.Button(">")) EditorUtility.RevealInFinder(profileAndBucketProperties.localBuildOutputPath);
                            GUILayout.EndHorizontal();
                        }
                    }
                }

                EditorGUILayout.Separator();
                EditorGUI.EndDisabledGroup();

                var newFileMask = (Platform) EnumFlagsButtonGroupDrawer.DrawLayout(UploadToS3Settings.Instance.fileListPlatformMask, new GUIContent("Platforms"));
                if (UploadToS3Settings.Instance.fileListPlatformMask != newFileMask) {
                    UploadToS3Settings.Instance.fileListPlatformMask = newFileMask;
                    BuildFileList();
                    RecheckFileList();
                }

                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                UploadToS3Settings.Instance.fileListExpanded = EditorGUILayout.Foldout(UploadToS3Settings.Instance.fileListExpanded, "Files");
                EditorGUILayout.EndHorizontal();


                if (UploadToS3Settings.Instance.fileListExpanded) {
                    DrawToolbar();
                    DrawFilesInDirectory(ref UploadToS3Settings.Instance.fileListPosition, 100);
                }
            }
            // EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            UploadToS3Settings.Save();


            // EditorGUILayout.Separator();
            // if (GUILayout.Button("Help me setting up the bucket!"))
            // {
            //     Application.OpenURL("https://thegamedev.guru/unity-addressables/hosting-with-amazon-s3/");
            // }
            // if (GUILayout.Button("Help me finding my region!"))
            // {
            //     Application.OpenURL("https://docs.aws.amazon.com/general/latest/gr/rande.html#s3_region");
            // }
            // if (GUILayout.Button("Help me setting up the keys!"))
            // {
            //     Application.OpenURL("https://aws.amazon.com/premiumsupport/knowledge-center/create-access-key/");
            // }

        }
    }
#pragma warning restore 4014
}