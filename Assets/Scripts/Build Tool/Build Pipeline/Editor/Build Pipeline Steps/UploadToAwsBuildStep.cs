namespace BuildPipeline {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Amazon.S3;
    using UnityEditor;
    using UnityEngine;

    [System.Serializable]
    public class UploadToAwsBuildStep : BuildPipelineStep {
        public bool copyIntoCurrentDirectory = true;

        public UploadToAwsBuildStep() {
            name = "Upload Build To AWS";
        }

        public override void DrawSettings() {
            copyIntoCurrentDirectory = EditorGUILayout.Toggle(new GUIContent("Copy into 'Current' directory"), copyIntoCurrentDirectory);
        }

        // public override void DrawProgress() {
        // GUILayout.BeginHorizontal();
        // EditorGUILayout.PrefixLabel("Step 3: Upload Addressables");
        // if(GUILayout.Button("Show Uploader", GUILayout.Width(100))) {
        //     if(addressablesUploaderWindow == null) addressablesUploaderWindow = ServerHostedFileWindow.Init();
        //     // addressablesUploaderWindow.SetFiles(activeProfileProperties, fileStatuses);
        //     addressablesUploaderWindow.Show();
        // }
        // BuildPipelineUtils.ProgressBar(uploadAddressablesProgress.label, uploadAddressablesProgress.progress);
        // GUILayout.EndHorizontal();
        // }


        public List<ServerHostedFileStatus> GetFilesToUploadForWebGL(AmazonS3Client s3Client, SetPlatformStep.BuildPlatform targetBuildPlatform, BuildPipelineEditorWindow.ProfileAndBucketProperties profileProperties) {
            List<ServerHostedFileStatus> serverHostedFileStatusList = new List<ServerHostedFileStatus>();

            // The path to the server that we'll use as the root to upload things to
            var formattedServerPath = BuildPipelineEditorWindow.GetRelativeServerPath(BuildInfo.Instance.version.ToString(), BuildPipelineEditorWindow.GetBuildTargetString(targetBuildPlatform.buildTarget));
            //
            // if(File.Exists(infoTextPath)) {
            //     var fileStatus = new ServerHostedFileStatus(new FileInfo(infoTextPath), BuildPipelineEditorWindow.GetBuildTargetString(targetBuildPlatform.buildTarget), formattedServerPath+"/Info.txt");
            //     serverHostedFileStatusList.Add(fileStatus);
            // }

            // Upload the WebGL build to the remote server so we can play it!
            var localPath = profileProperties.GetBuildPath(BuildInfo.Instance.version.ToString(), targetBuildPlatform.buildTarget);
            string[] files = Directory.GetFiles(localPath, "*", SearchOption.AllDirectories);

            foreach (var filePath in files) {
                var localFilePath = filePath.Substring(localPath.Length);
                var remoteRelativeFilePath = formattedServerPath + localFilePath;
                var fileStatus = new ServerHostedFileStatus(new FileInfo(filePath), BuildPipelineEditorWindow.GetBuildTargetString(targetBuildPlatform.buildTarget), remoteRelativeFilePath);
                var extension = Path.GetExtension(filePath);

                // FrisbeeGame.data.br
                // System defined	Content-Encoding	br
                // System defined	Content-Type	binary/octet-stream

                // FrisbeeGame.framework.js.br
                // System defined	Content-Encoding	br
                // System defined	Content-Type	application/javascript

                // FrisbeeGame.loader.js
                // System defined	Content-Type	application/javascript

                // FrisbeeGame.wasm.br
                // System defined	Content-Encoding	br
                // System defined	Content-Type	application/wasm
                {
                    if (extension == ".br") {
                        fileStatus.fileContentEncoding = "br";
                        extension = Path.GetExtension(filePath.Substring(0, filePath.Length - (".br".Length)));
                    } else if (extension == ".gz") {
                        fileStatus.fileContentEncoding = "gz";
                        extension = Path.GetExtension(filePath.Substring(0, filePath.Length - (".gz".Length)));
                    } else if (extension == ".unityweb") {
                        // I'm not sure how this differs from "gz", but unity recommends doing it like this for .unityweb files.
                        if (PlayerSettings.WebGL.compressionFormat == WebGLCompressionFormat.Gzip) fileStatus.fileContentEncoding = "gzip";
                        else if (PlayerSettings.WebGL.compressionFormat == WebGLCompressionFormat.Brotli) fileStatus.fileContentEncoding = "br";
                        extension = Path.GetExtension(filePath.Substring(0, filePath.Length - (".unityweb".Length)));
                    }

                    if (extension.EndsWith(".data")) fileStatus.fileContentType = "binary/octet-stream";
                    else if (extension.EndsWith(".js")) fileStatus.fileContentType = "application/javascript";
                    else if (extension.EndsWith(".wasm")) fileStatus.fileContentType = "application/wasm";
                }
                serverHostedFileStatusList.Add(fileStatus);
            }

            return serverHostedFileStatusList;
        }

        public async Task UploadFiles(AmazonS3Client s3Client, List<ServerHostedFileStatus> serverHostedFileStatusList, SetPlatformStep.BuildPlatform targetBuildPlatform, BuildPipelineEditorWindow.ProfileAndBucketProperties profileProperties) {
            progressTracker.Start("Uploading Build");

            var formattedServerPath = BuildPipelineEditorWindow.GetRelativeServerPath(BuildInfo.Instance.version.ToString(), BuildPipelineEditorWindow.GetBuildTargetString(targetBuildPlatform.buildTarget));
            // We don't need to do this, since we check if the files have changed; but it's cleaner
            await AWSUtils.DeleteDirectoryAsync(s3Client, profileProperties.bucketName, formattedServerPath);

            if (BuildPipelineEditorWindow.addressablesUploaderWindow == null) BuildPipelineEditorWindow.addressablesUploaderWindow = ServerHostedFileWindow.Init();
            BuildPipelineEditorWindow.addressablesUploaderWindow.SetFiles(profileProperties, serverHostedFileStatusList);

            foreach (var fileStatus in serverHostedFileStatusList)
                if (fileStatus.requiresCheck)
                    fileStatus.status = ServerHostedFileStatus.Status.QueuedForCheck;
            await AWSUtils.RefreshStatusOfFiles(s3Client, profileProperties, serverHostedFileStatusList, false, (progress) => { progressTracker.Update("Checking status " + (progress * 100) + "%", progress * 0.01f); });

            var filesToUpload = serverHostedFileStatusList.Where(x => x.requiresUpload);
            await AWSUtils.UploadFilesAsync(profileProperties, filesToUpload.ToArray(), (progress) => {
                progressTracker.Update("Uploading " + (progress * 100) + "%", progress);
                // progressTracker.Update("Uploading build files from "+localPath+" to "+profileProperties.GetRelativeServerPath()+"...", (progress/100f));
            });
            progressTracker.Complete("Uploaded Files!");


            // foreach (var filesToUpload in serverHostedFileStatusList) {
            //     await AWSUtils.UploadFileAsync(s3Client, profileProperties, filesToUpload, filesToUpload.relativePath, (progressState) => {
            //         // var progress = (float)progressState.TransferredBytes/progressState.TotalBytes;
            //         uploadBuildProgress.Update("Uploading build files from "+localPath+" to "+targetPath+"...", (progressState.PercentDone/100f));
            //     }, null);
            // }
        }



        public async Task CopyBuildAsLatest(AmazonS3Client s3Client, BuildPipelineEditorWindow.ProfileAndBucketProperties profileProperties, string buildTargetString) {
            var sourceDirectory = BuildPipelineEditorWindow.GetRelativeServerPath(BuildInfo.Instance.version.ToString(), buildTargetString);
            var targetDirectory = BuildPipelineEditorWindow.GetRelativeServerPath("current", buildTargetString);
            await AWSUtils.CopyDirectory(s3Client, profileProperties.bucketName, sourceDirectory, profileProperties.bucketName, targetDirectory);
        }
    }
}