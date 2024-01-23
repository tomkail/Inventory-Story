namespace BuildPipeline {
#pragma warning disable 4014
    using System.Linq;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Amazon;
    using Amazon.Runtime;

    public class ServerHostedFileWindow : EditorWindow {
        protected BuildPipelineEditorWindow.ProfileAndBucketProperties profileAndBucketProperties;
        protected List<ServerHostedFileStatus> allFileStatuses = new List<ServerHostedFileStatus>();
        protected ServerHostedFileStatus[] fileStatuses = new ServerHostedFileStatus[0];

        Vector2 fileListPosition;
        string searchString;


        const float fileFieldHeight = 20;
        static float extraHeight = 9;
        static float spacing = 5;

        // [MenuItem("Tools/Addressable Content Uploader (S3)")]
        // static new void Init() {
        //     var window = (UploadToS3)EditorWindow.GetWindow(typeof(UploadToS3));
        //     window.Show();
        // }

        public static ServerHostedFileWindow Init() {
            var window = (ServerHostedFileWindow) EditorWindow.GetWindow(typeof(ServerHostedFileWindow), false, "Addressable Files Uploader", true);
            window.minSize = new Vector2(800, 600);
            return window;
        }

        public void SetFiles(BuildPipelineEditorWindow.ProfileAndBucketProperties profileAndBucketProperties, List<ServerHostedFileStatus> fileStatuses) {
            this.profileAndBucketProperties = profileAndBucketProperties;
            this.allFileStatuses = fileStatuses;
            RefreshValidFileStatuses();
        }

        void OnInspectorUpdate() {
            Repaint();
        }

        void OnGUI() {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            EditorGUILayout.LabelField(fileStatuses.Count(x => x.status == ServerHostedFileStatus.Status.Uploaded) + "/" + fileStatuses.Length + " uploaded files");
            // EditorGUILayout.LabelField(fileStatuses.Count(x => x.status == ServerHostedFileStatus.Status.Checking)+" checking");
            EditorGUILayout.LabelField(fileStatuses.Count(x => x.status == ServerHostedFileStatus.Status.QueuedForUpload) + " queued");
            EditorGUILayout.LabelField(fileStatuses.Count(x => x.status == ServerHostedFileStatus.Status.Uploading) + " uploading");

            bool changed = DrawSearchBar(ref searchString);
            if (changed) RefreshValidFileStatuses();

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            DrawFilesInDirectory(ref fileListPosition, 100);
            EditorGUILayout.EndVertical();
        }

        protected void RefreshValidFileStatuses() {
            if (allFileStatuses != null)
                fileStatuses = allFileStatuses.Where(x => SearchStringMatch(x.relativePath, searchString)).OrderByDescending(x => x.fileInfo.LastWriteTime).ToArray();
        }

        static bool StringContains(string str, string toCheck, StringComparison comp) {
            if (toCheck.Length == 0) return false;
            return str.IndexOf(toCheck, comp) >= 0;
        }

        static GUIStyle searchTextFieldStyle;
        static GUIStyle searchCancelButtonStyle;

        static bool DrawSearchBar(ref string searchString) {
            if (searchTextFieldStyle == null) searchTextFieldStyle = GUI.skin.FindStyle("ToolbarSearchTextField");
            if (searchCancelButtonStyle == null) searchCancelButtonStyle = GUI.skin.FindStyle("ToolbarSearchCancelButton");

            var lastString = searchString;
            searchString = GUILayout.TextField(searchString, searchTextFieldStyle);
            if (GUILayout.Button("", searchCancelButtonStyle)) {
                searchString = string.Empty;
            }

            return lastString != searchString;
        }

        static bool SearchStringMatch(string content, string searchString) {
            return string.IsNullOrWhiteSpace(searchString) || StringContains(content, searchString, StringComparison.OrdinalIgnoreCase);
        }

        static void DrawLoadableFile(BuildPipelineEditorWindow.ProfileAndBucketProperties profileAndBucketProperties, ServerHostedFileStatus fileStatus) {
            var lastWriteTime = fileStatus.fileInfo.LastWriteTime;

            EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.Height(fileFieldHeight));

            EditorGUILayout.LabelField(fileStatus.buildTarget, GUILayout.Width(80), GUILayout.Height(fileFieldHeight - 5));
            EditorGUI.BeginDisabledGroup(fileStatus.status != ServerHostedFileStatus.Status.Uploaded);
            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("d_UnityEditor.ConsoleWindow")), EditorStyles.miniButton, GUILayout.Width(24))) {
                var consoleURL = profileAndBucketProperties.GetAwsConsoleURL(fileStatus.relativePath);
                Application.OpenURL(consoleURL);
            }

            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("BuildSettings.Web.Small")), EditorStyles.miniButton, GUILayout.Width(24))) {
                var consoleURL = profileAndBucketProperties.GetBuildURL(fileStatus.relativePath);
                Application.OpenURL(consoleURL);
            }

            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("d_Folder Icon")), EditorStyles.miniButton, GUILayout.Width(24))) {
                EditorUtility.RevealInFinder(fileStatus.fileInfo.FullName);
            }

            EditorGUILayout.LabelField(fileStatus.relativePath, GUILayout.Width(780));

            EditorGUILayout.LabelField(new GUIContent(ByteFormatter.ToString(fileStatus.fileInfo.Length, ByteFormatter.SI.MB)), EditorStyles.centeredGreyMiniLabel, GUILayout.Width(52), GUILayout.Height(fileFieldHeight - 5));
            EditorGUILayout.LabelField(new GUIContent(lastWriteTime.ToShortDateString() + " " + lastWriteTime.ToShortTimeString()), EditorStyles.miniBoldLabel, GUILayout.Width(102), GUILayout.Height(fileFieldHeight - 5));



            EditorGUI.BeginDisabledGroup(!fileStatus.requiresUpload);
            if (GUILayout.Button("Upload", EditorStyles.miniButton, GUILayout.Width(90), GUILayout.Height(fileFieldHeight - 5))) {
                AWSUtils.UploadFileAsync(profileAndBucketProperties.CreateClient(), profileAndBucketProperties, fileStatus);
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!fileStatus.existsOnServer);
            if (GUILayout.Button("Delete", EditorStyles.miniButton, GUILayout.Width(90), GUILayout.Height(fileFieldHeight - 5))) {
                AWSUtils.DeleteFileAsync(profileAndBucketProperties.CreateClient(), profileAndBucketProperties, fileStatus);
            }

            EditorGUI.EndDisabledGroup();

            if (fileStatus.status == ServerHostedFileStatus.Status.NotUploaded) {
                GUILayout.Label("Not uploaded", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(100));
            } else if (fileStatus.status == ServerHostedFileStatus.Status.Changed) {
                GUILayout.Label("Changed", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(100));
            } else if (fileStatus.status == ServerHostedFileStatus.Status.QueuedForCheck) {
                GUILayout.Label("Queued for check", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(100));
            } else if (fileStatus.status == ServerHostedFileStatus.Status.Checking) {
                GUILayout.Label("Checking...", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(100));
            } else if (fileStatus.status == ServerHostedFileStatus.Status.Deleting) {
                GUILayout.Label("Deleting...", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(100));
            } else if (fileStatus.status == ServerHostedFileStatus.Status.Uploaded) {
                GUILayout.Label("Uploaded", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(100));
            } else if (fileStatus.status == ServerHostedFileStatus.Status.QueuedForUpload) {
                GUILayout.Label("Queued for upload", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(100));
            } else if (fileStatus.status == ServerHostedFileStatus.Status.Uploading) {
                var rect = EditorGUILayout.GetControlRect(GUILayout.Width(100));
                EditorGUI.ProgressBar(rect, fileStatus.uploadTaskProgress.progress, "");
                GUI.Label(rect, "Uploading...", EditorStyles.centeredGreyMiniLabel);
            } else if (fileStatus.status == ServerHostedFileStatus.Status.Unknown) {
                GUILayout.Label("???", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(100));
            } else {
                GUILayout.Label(fileStatus.status.ToString(), EditorStyles.centeredGreyMiniLabel, GUILayout.Width(100));
            }

            EditorGUILayout.EndHorizontal();
        }




        public void DrawToolbar() {
            EditorGUILayout.BeginHorizontal();

            var numRefreshing = fileStatuses.Count(x => x.status == ServerHostedFileStatus.Status.Checking || x.status == ServerHostedFileStatus.Status.QueuedForCheck);
            // EditorGUI.BeginDisabledGroup(numRefreshing > 0 && !Event.current.control);
            if (GUILayout.Button(numRefreshing == 0 ? "Refresh local file list" : "Refreshing " + numRefreshing + " files...")) {
                RecheckFileList();
            }
            // EditorGUI.EndDisabledGroup();

            var filesToUpload = fileStatuses.Where(x => x.requiresUpload);
            var filesToUploadCount = filesToUpload.Count();
            EditorGUI.BeginDisabledGroup((numRefreshing > 0 || filesToUploadCount == 0) && !Event.current.control);
            if (GUILayout.Button("Upload " + filesToUploadCount + " files missing from server (" + ByteFormatter.ToString(filesToUpload.Select(x => x.fileInfo.Length).Sum(), ByteFormatter.SI.MB) + ")")) {
                var startTime = Time.realtimeSinceStartup;
                // Debug.Log("Uploading "+files.Count()+" files...");
                // Debug.Log("Upload "+files.Count()+" files");

                AWSUtils.UploadFilesAsync(profileAndBucketProperties, filesToUpload.ToArray(), null, () => {
                    var endTime = Time.realtimeSinceStartup;
                    EditorUtility.DisplayDialog("Upload complete!", "Took " + (endTime - startTime) + "s", "Ok!");
                });
            }

            EditorGUI.EndDisabledGroup();

            var filesToDelete = fileStatuses.Where(x => x.existsOnServer);
            var filesToDeleteCount = filesToDelete.Count();
            EditorGUI.BeginDisabledGroup((numRefreshing > 0 || filesToDeleteCount == 0) && !Event.current.control);
            if (GUILayout.Button("Delete " + filesToDeleteCount + " files on server")) {
                var startTime = Time.realtimeSinceStartup;
                AWSUtils.DeleteFiles(profileAndBucketProperties, filesToDelete.ToArray(), () => {
                    var endTime = Time.realtimeSinceStartup;
                    EditorUtility.DisplayDialog("Delete complete!", "Took " + (endTime - startTime) + "s", "Ok!");
                });
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
        }

        public void DrawFilesInDirectory(ref Vector2 scrollPosition, int maxNumToShow = 1) {
            if (fileStatuses == null || fileStatuses.Length == 0) return;

            maxNumToShow = Mathf.Min(maxNumToShow, fileStatuses.Length);
            float m_ItemHeight = fileFieldHeight;
            float scrollRectHeight = (maxNumToShow * m_ItemHeight) + ((maxNumToShow - 1) * spacing) + extraHeight;
            int numToShow = Mathf.CeilToInt(scrollRectHeight / m_ItemHeight);
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(false));
            int firstIndex = (int) (scrollPosition.y / m_ItemHeight);
            firstIndex = Mathf.Clamp(firstIndex, 0, Mathf.Max(0, fileStatuses.Length - numToShow));
            if (firstIndex * m_ItemHeight > 0) GUILayout.Space(firstIndex * m_ItemHeight);
            var lastIndex = Mathf.Min(fileStatuses.Length, firstIndex + numToShow);
            for (int i = firstIndex; i < lastIndex; i++) {
                var item = fileStatuses[i];
                DrawLoadableFile(profileAndBucketProperties, item);
            }

            var numItemsOffScreenAtEnd = Mathf.Max(0, fileStatuses.Length - firstIndex - numToShow);
            if (numItemsOffScreenAtEnd * m_ItemHeight > 0) GUILayout.Space(numItemsOffScreenAtEnd * m_ItemHeight);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }






        public async Task RecheckFileList() {
            foreach (var fileStatus in fileStatuses)
                if (fileStatus.status == ServerHostedFileStatus.Status.Unknown)
                    fileStatus.status = ServerHostedFileStatus.Status.QueuedForCheck;
            var s3Client = profileAndBucketProperties.CreateClient();
            float time = Time.realtimeSinceStartup;
            // foreach(var fileStatus in fileList) await CheckFileUploadedStatus(s3Client, profileAndBucketProperties, fileStatus);
            // This works inconsistently, but is much faster
            await AWSUtils.RefreshStatusOfFiles(s3Client, profileAndBucketProperties, fileStatuses);
            Debug.Log("Took " + (Time.realtimeSinceStartup - time) + " to refresh state of " + fileStatuses.Length + " files");
            RefreshValidFileStatuses();
        }
    }
#pragma warning restore 4014
}