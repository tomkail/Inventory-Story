namespace BuildPipeline {
// Copyright 2019 The Gamedev Guru (http://thegamedev.guru)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#pragma warning disable 4014
    using Amazon.S3;
    using Amazon.S3.Transfer;
    using Amazon.S3.Model;
    using Amazon.Runtime;
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public static class AWSUtils {
        public static async void UploadDirectory(AmazonS3Client s3Client, BuildPipelineEditorWindow.ProfileAndBucketProperties profileAndBucketProperties, string directoryPath, string keyPrefix) {
            await UploadDirectoryAsync(s3Client, profileAndBucketProperties, directoryPath, keyPrefix);
        }

        public static async Task UploadFileAsync(AmazonS3Client s3Client, BuildPipelineEditorWindow.ProfileAndBucketProperties profileAndBucketProperties, string buildTarget, string filePath, string keyPrefix, Action<UploadProgressArgs> progressCallback = null) {
            var file = new FileInfo(filePath);
            var currentlyUploadingFileStatus = new ServerHostedFileStatus(file, buildTarget, keyPrefix);
            // var s3Client = activeProfileProperties.CreateClient(UploadToS3.GetMaxTimeForUpload(file));
            await UploadFileAsync(s3Client, profileAndBucketProperties, currentlyUploadingFileStatus, progressCallback);
        }

        // Deletes all objects at a directory recursively
        public static async Task DeleteDirectoryAsync(AmazonS3Client s3Client, string bucketName, string keyPrefix) {
            // Debug.Log("Deleting "+bucketName+": "+keyPrefix);
            ListObjectsRequest listRequest = new ListObjectsRequest {
                BucketName = bucketName,
                Prefix = keyPrefix
            };
            ListObjectsResponse response = await s3Client.ListObjectsAsync(listRequest);

            if (response.S3Objects.Count == 0) {
                // Debug.LogWarning("DeleteDirectoryAsync: No objects to delete at "+bucketName+"::"+keyPrefix);
                return;
            }

            DeleteObjectsRequest deleteRequest = new DeleteObjectsRequest();
            deleteRequest.BucketName = bucketName;
            foreach (S3Object entry in response.S3Objects) {
                deleteRequest.AddKey(entry.Key);
            }

            // DeleteObjectsResponse deletedObjectsResponse = 
            await s3Client.DeleteObjectsAsync(deleteRequest);
            // Debug.Log("Deleted "+bucketName+": "+keyPrefix);

        }

        public static async Task UploadDirectoryAsync(AmazonS3Client s3Client, BuildPipelineEditorWindow.ProfileAndBucketProperties profileAndBucketProperties, string directoryPath, string keyPrefix, Action<UploadDirectoryProgressArgs> progressCallback = null) {
            var directory = new DirectoryInfo(directoryPath);
            // Debug.Log("Directory upload contents of "+directoryPath+" to "+profileAndBucketProperties.bucketName+": "+keyPrefix+"\nMax Time: "+UploadToS3.GetMaxTimeForUpload(directory)+"\nTotal Size: "+(float)ByteFormatter.ToSize(UploadToS3.GetDirectorySize(directory.FullName), ByteFormatter.SI.MB)+"mb");
            var transferUtility = new TransferUtility(s3Client);
            // UploadToS3.GetMaxTimeForUpload(directory)
            var transferUtilityRequest = new TransferUtilityUploadDirectoryRequest {
                BucketName = profileAndBucketProperties.bucketName,
                KeyPrefix = keyPrefix,
                Directory = directoryPath,
                CannedACL = profileAndBucketProperties.permissions,
                SearchOption = SearchOption.AllDirectories,
            };

            if (progressCallback != null) {
                transferUtilityRequest.UploadDirectoryProgressEvent += (object sender, UploadDirectoryProgressArgs args) => { progressCallback(args); };
            }

            await transferUtility.UploadDirectoryAsync(transferUtilityRequest);
            // activeProfileProperties
            // UploadToS3.UploadFileAsync(new TransferUtility(s3Client), activeProfileProperties, new UploadToS3.FileStatus(new FileInfo(releasePath), buildTarget, relativePath))
        }

        public static async Task CopyDirectory(AmazonS3Client s3Client, string sourceBucket, string sourceKey, string destinationBucket, string destinationKey) {
            try {
                var request = new ListObjectsV2Request {
                    BucketName = sourceBucket,
                    Prefix = sourceKey
                };
                var listObjectsResponse = await s3Client.ListObjectsV2Async(request, CancellationToken.None);
                foreach (var obj in listObjectsResponse.S3Objects) {
                    var afterBuildPath = obj.Key.Substring(sourceKey.Length);
                    var destinationPath = destinationKey + afterBuildPath;
                    // Debug.Log("Copy from "+obj.Key+" to "+destinationPath);
                    CopyObjectRequest copyObjRequest = new CopyObjectRequest() {
                        SourceBucket = sourceBucket,
                        SourceKey = obj.Key,
                        DestinationBucket = destinationBucket,
                        DestinationKey = destinationPath
                    };
                    await s3Client.CopyObjectAsync(copyObjRequest);
                }
            } catch (AmazonServiceException e) {
                Debug.LogError(e);
            }
        }


        // public static long GetDirectorySize (string directoryPath) {
        //     long size = 0;
        //     foreach(var file in Directory.GetFiles(directoryPath)) {
        //         size += new FileInfo(file).Length;
        //     }
        //     foreach(var directory in Directory.GetDirectories(directoryPath)) {
        //         size += GetDirectorySize(directory);
        //     }
        //     return size;
        // } 

        // // Min upload speed before cancelling. Higher means faster timeouts.
        // public static float GetMaxTimeForUpload (DirectoryInfo directoryInfo, float minMBps = 0.05f) {
        //     return Mathf.Max(10, (float)ByteFormatter.ToSize(GetDirectorySize(directoryInfo.FullName), ByteFormatter.SI.MB) / minMBps);
        // }
        // public static float GetMaxTimeForUpload (FileInfo fileInfo, float minMBps = 0.05f) {
        //     return Mathf.Max(10, (float)ByteFormatter.ToSize(fileInfo.Length, ByteFormatter.SI.MB) / minMBps);
        // }





        public static ServerHostedFileStatus.Status GetStatusFromLocalAndRemoteFiles(ServerHostedFileStatus fileStatus, GetObjectMetadataResponse obj) {
            if (obj == null) {
                return ServerHostedFileStatus.Status.NotUploaded;
            } else {
                Debug.Log(fileStatus.fileContentEncoding + " " + obj.Headers.ContentEncoding);
                Debug.Log(fileStatus.fileContentType + " " + obj.Headers.ContentType);
                Debug.Log(fileStatus.fileInfo.Length + " " + obj.Headers.ContentLength);
                Debug.Log(fileStatus.fileInfo.LastWriteTime.ToUniversalTime() + " " + obj.LastModified.ToUniversalTime());
                if (
                    fileStatus.fileContentEncoding != obj.Headers.ContentEncoding ||
                    fileStatus.fileContentType != obj.Headers.ContentType ||
                    fileStatus.fileInfo.Length != obj.Headers.ContentLength ||
                    fileStatus.fileInfo.LastWriteTime.ToUniversalTime() > obj.LastModified.ToUniversalTime()
                ) {
                    return ServerHostedFileStatus.Status.Changed;
                } else {
                    return ServerHostedFileStatus.Status.Uploaded;
                }
            }
        }




        public static async Task RefreshStatusOfFiles(AmazonS3Client s3Client, BuildPipelineEditorWindow.ProfileAndBucketProperties profileAndBucketProperties, IList<ServerHostedFileStatus> fileStatuses, bool forceCheck = false, Action<float> onUpdateProgress = null) {
            await Task.WhenAll(fileStatuses.Select(fileStatus => CheckFileUploadedStatus(s3Client, profileAndBucketProperties, fileStatus, forceCheck, () => {
                if (onUpdateProgress != null) onUpdateProgress(fileStatuses.Sum(x => x.findTaskProgress == null ? 0 : x.findTaskProgress.progress) / fileStatuses.Count);
            })));
        }

        public static async Task CheckFileUploadedStatus(AmazonS3Client s3Client, BuildPipelineEditorWindow.ProfileAndBucketProperties profileAndBucketProperties, ServerHostedFileStatus fileStatus, bool forceCheck = false, Action onComplete = null) {
            if (fileStatus.status == ServerHostedFileStatus.Status.Unknown || fileStatus.status == ServerHostedFileStatus.Status.QueuedForCheck || forceCheck) {
                fileStatus.status = ServerHostedFileStatus.Status.Checking;
                // var uploadPath = profileAndBucketProperties.GetFormattedServerAddressablesPath(fileStatus.buildTarget, BuildInfo.Instance.version.ToString(), fileStatus.relativePath);
                var remoteFileMetadata = await FindRemoteFile(s3Client, profileAndBucketProperties.bucketName, fileStatus);
                fileStatus.status = GetStatusFromLocalAndRemoteFiles(fileStatus, remoteFileMetadata);
            }

            if (onComplete != null) onComplete();
        }

        public static async Task<GetObjectMetadataResponse> FindRemoteFile(AmazonS3Client s3Client, string bucketName, ServerHostedFileStatus fileStatus) {
            fileStatus.findTaskProgress = new AWSTaskProgress();
            GetObjectMetadataRequest request = new GetObjectMetadataRequest {
                BucketName = bucketName,
                Key = fileStatus.relativePath
            };
            GetObjectMetadataResponse response = null;
            try {
                response = await s3Client.GetObjectMetadataAsync(request);
            } catch {
            }

            fileStatus.findTaskProgress.progress = 1;
            return response;
        }




        public static async Task DeleteFiles(BuildPipelineEditorWindow.ProfileAndBucketProperties profileAndBucketProperties, IEnumerable<ServerHostedFileStatus> files, Action OnComplete = null) {
            Debug.Log("Deleting " + files.Count() + " files...");
            foreach (var fileStatus in files) {
                fileStatus.status = ServerHostedFileStatus.Status.Deleting;
            }

            var s3Client = profileAndBucketProperties.CreateClient();
            await Task.WhenAll(files.Select(fileStatus => DeleteFileAsync(s3Client, profileAndBucketProperties, fileStatus)));
            foreach (var fileStatus in files) {
                fileStatus.status = ServerHostedFileStatus.Status.NotUploaded;
            }

            Debug.Log("Upload " + files.Count() + " files");
            if (OnComplete != null) OnComplete();
        }

        public static async Task DeleteFileAsync(AmazonS3Client s3Client, BuildPipelineEditorWindow.ProfileAndBucketProperties profileAndBucketProperties, ServerHostedFileStatus fileStatus) {
            // var key = profileAndBucketProperties.GetFormattedServerAddressablesPath(fileStatus.buildTarget, BuildInfo.Instance.version.ToString(), fileStatus.relativePath);
            try {
                var deleteObjectRequest = new DeleteObjectRequest {
                    BucketName = profileAndBucketProperties.bucketName,
                    Key = fileStatus.relativePath
                };

                fileStatus.status = ServerHostedFileStatus.Status.Deleting;
                Console.WriteLine("Deleting an object");
                await s3Client.DeleteObjectAsync(deleteObjectRequest);
                fileStatus.status = ServerHostedFileStatus.Status.NotUploaded;
            } catch (AmazonS3Exception e) {
                Console.WriteLine("Error encountered on server. Message:'{0}' when deleting an object", e.Message);
            } catch (Exception e) {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when deleting an object", e.Message);
            }
        }



        public static async Task UploadFilesAsync(BuildPipelineEditorWindow.ProfileAndBucketProperties profileAndBucketProperties, IEnumerable<ServerHostedFileStatus> files, Action<float> OnUpdateProgress, Action OnComplete = null) {
            foreach (var fileStatus in files) {
                fileStatus.status = ServerHostedFileStatus.Status.QueuedForUpload;
            }

            var s3Client = profileAndBucketProperties.CreateClient();
            var totalBytes = files.Select(x => x.fileInfo.Length).Sum();

            void ProgressCallback(UploadProgressArgs uploadProgressArgs) {
                if (OnUpdateProgress != null) OnUpdateProgress(files.Select(x => x.uploadTaskProgress == null ? 0 : x.uploadTaskProgress.bytesTransferred).Sum() / totalBytes);
            }

            List<ServerHostedFileStatus> filesRemaining = new List<ServerHostedFileStatus>(files);
            List<ServerHostedFileStatus> filesToUploadThisBatch = new List<ServerHostedFileStatus>();
            List<Task> tasks = new List<Task>();

            // When uploading too much at once (I had around 1100 when I first noticed the error), AWS seems to throw an error. I've fixed it by limiting the number of files uploaded in one go.
            // I suspect the error was actually caused by the size of the files, rather than the quantity, but this works.
            int maxSimultaneousUploads = 500;
            for (int i = 0; i < Mathf.Min(filesRemaining.Count, maxSimultaneousUploads); i++) {
                tasks.Add(UploadFileAsync(s3Client, profileAndBucketProperties, filesRemaining[0], ProgressCallback));
                filesRemaining.RemoveAt(0);
            }

            while (tasks.Count > 0) {
                var taskCompleted = await Task.WhenAny(tasks);
                tasks.Remove(taskCompleted);
                if (filesRemaining.Count > 0) {
                    tasks.Add(UploadFileAsync(s3Client, profileAndBucketProperties, filesRemaining[0], ProgressCallback));
                    filesRemaining.RemoveAt(0);
                }
            }


            // foreach(var fileStatus in files) await UploadFile(s3Client, profileAndBucketProperties, fileStatus);

            if (OnComplete != null) OnComplete();
        }

        public static async Task UploadFileAsync(AmazonS3Client s3Client, BuildPipelineEditorWindow.ProfileAndBucketProperties profileAndBucketProperties, ServerHostedFileStatus fileStatus, Action<UploadProgressArgs> progressCallback = null, Action<ServerHostedFileStatus> completeCallback = null) {
            // var uploadPath = profileAndBucketProperties.GetFormattedServerAddressablesPath(fileStatus.buildTarget, BuildInfo.Instance.version.ToString(), fileStatus.relativePath);
            var transferUtility = new TransferUtility(s3Client);
            fileStatus.uploadTaskProgress = new AWSTaskProgress();
            try {
                fileStatus.status = ServerHostedFileStatus.Status.Uploading;
                fileStatus.uploadTaskProgress.progress = 0;
                fileStatus.uploadTaskProgress.bytesTransferred = 0;
                // var uploadStartTime = System.DateTime.Now;

                // Debug.Log("Starting file upload from:\n"+fileStatus.fileInfo.FullName+"\nTo:\n"+s3Client.Config.RegionEndpoint+" "+profileAndBucketProperties.bucketName+": "+uploadPath);
                var transferUtilityRequest = new TransferUtilityUploadRequest {
                    BucketName = profileAndBucketProperties.bucketName,
                    FilePath = fileStatus.fileInfo.FullName,
                    // This only works when ACL is enabled
                    // CannedACL = profileAndBucketProperties.permissions,
                    Key = fileStatus.relativePath,
                };

                if (!string.IsNullOrEmpty(fileStatus.fileContentEncoding)) transferUtilityRequest.Headers.ContentEncoding = fileStatus.fileContentEncoding;
                if (!string.IsNullOrEmpty(fileStatus.fileContentType)) transferUtilityRequest.Headers.ContentType = fileStatus.fileContentType;

                transferUtilityRequest.UploadProgressEvent += (object sender, UploadProgressArgs args) => {
                    fileStatus.uploadTaskProgress.progress = args.PercentDone / 100f;
                    fileStatus.uploadTaskProgress.bytesTransferred = args.TransferredBytes;
                    fileStatus.uploadTaskProgress.bytesTotal = args.TotalBytes;
                    if (progressCallback != null) progressCallback(args);
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
            if (completeCallback != null) completeCallback(fileStatus);
        }













        // File checking
        /*
        static async void ListAllObjectsInBucket() {
            Debug.Log("Start");
            var s3Client = CreateClientFromCredentials(5);
            using (s3Client) {
                var request = new ListObjectsV2Request {
                    BucketName = UploadToS3Settings.Instance.bucketName
                };
                var response = await s3Client.ListObjectsV2Async(request, CancellationToken.None);
                foreach(var x in response.S3Objects) {
                    Debug.Log(x.Key);
                }
            }
            Debug.Log("DONE");
        }
        static async void RemoteFileExists (FileInfo fileInfo, Action<bool> OnComplete) {
            var relativePath = UploadToS3Settings.Instance.GetRelativePath(fileInfo.FullName);
            var testExists = await RemoteFileExists(UploadToS3Settings.Instance.bucketName, relativePath);
            if(OnComplete != null) OnComplete(testExists);
        }
        static async Task<bool> RemoteFileExists(string bucketName, string filePrefix) {
            var s3Client = CreateClientFromCredentials(5);
            {
                var request = new ListObjectsRequest {
                    BucketName = bucketName,
                    Prefix = filePrefix,
                    MaxKeys = 1
                };
                var response = await s3Client.ListObjectsAsync(request, CancellationToken.None);
                return response.S3Objects.Any();
            }
        }
        */


        // Upload a directory
        /*
        async Task InitiateUploadTask()   {
            var transferUtility = new TransferUtility(CreateClientFromCredentials(5));
            await UploadAsync(transferUtility, UploadToS3Settings.Instance.bucketName, UploadToS3Settings.Instance.projectRelativePath, UploadToS3Settings.Instance.searchPattern);
        }
    
        private static async Task UploadAsync(TransferUtility transferUtility, string bucketName, string path, string searchPattern = "*") {
            try {
                var isDirectory = System.IO.Directory.Exists(path);
                var isFile = System.IO.File.Exists(path);
                
                var uploadStartTime = System.DateTime.Now;
                if(isDirectory) {
                    var transferUtilityRequest = new TransferUtilityUploadDirectoryRequest {
                        BucketName = bucketName,
                        Directory = path,
                        StorageClass = S3StorageClass.Standard,
                        CannedACL = S3CannedACL.PublicRead,
                        SearchOption = SearchOption.AllDirectories,
                    };
                    transferUtilityRequest.UploadDirectoryProgressEvent += (object sender, UploadDirectoryProgressArgs args) => {
                        Debug.Log(args);
                    };
                    var log = "Starting directory upload from "+path+" to "+bucketName;
                    if(!string.IsNullOrWhiteSpace(searchPattern)) {
                        transferUtilityRequest.SearchPattern = searchPattern;
                        log += " with pattern "+searchPattern;
                    }
                    Debug.Log(log);
                    await transferUtility.UploadDirectoryAsync(transferUtilityRequest);
                    var duration = System.DateTime.Now - uploadStartTime;
                    Debug.Log("Directory upload completed in "+duration+"! "+path);
                } else if(isFile) {
                    Debug.Log("Starting file upload from "+path+" to "+bucketName);
                    var transferUtilityRequest = new TransferUtilityUploadRequest {
                        BucketName = bucketName,
                        FilePath = path,
                        StorageClass = S3StorageClass.StandardInfrequentAccess,
                        CannedACL = S3CannedACL.PublicRead,
                    };
                    await transferUtility.UploadAsync(transferUtilityRequest);
                    var duration = System.DateTime.Now - uploadStartTime;
                    Debug.Log("File upload completed in "+duration+"! "+path);
                } else {
                    Debug.LogWarning("Path invalid! "+path);
                }
            }
            catch (AmazonS3Exception e) {
                Debug.LogError("Error encountered on server when writing an object: " + e.Message);
            }
            catch (Exception e) {
                Debug.LogError("Unknown encountered on server when writing an object: " + e.Message);
            }
        }
        */
    }
#pragma warning restore 4014

}