namespace BuildPipeline {
    [System.Serializable]
    public class UploadAddressablesStep : BuildPipelineStep {
        public UploadAddressablesStep() {
            name = "Upload Addressables";
        }

#if ADDRESSABLES

    /*
    public class ProfileAndBucketProperties {
        // The name of the profile as specified in Unity.
        public string iamAccessKeyId;
        public string iamSecretKey;

        public string profileName;
        public string profileID => AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetProfileId(profileName);
        // The AWS bucket name
        public string bucketName => AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetValueByName(profileID, "Bucket");
        public string regionString => AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetValueByName(profileID, "Region");
        public string buildPlatform => AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetValueByName(profileID, "BuildTarget");
        public string localBuildOutputPath => GetBuildOutputPath(bucketName);
        // public string serverBucketPlatformDirectoryPath => GetServerBucketDirectoryPath(bucketName);
        
        // Such as: resources/unity/[BuildTarget]/current
        // public string bucketFolderRoot;
        public string remoteBucketFolderRoot => AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetValueByName(profileID, "BucketRelativePath");
        
        // Such as: resources/unity/[BuildTarget]/current/(FilePath)
        // BuildTarget is webgl, ios, etc.
        // FilePath is the file name, or any other relative path
        public string remoteAddressablesPath => remoteBucketFolderRoot+"/addressables/(FilePath)";
        
        public string standaloneIosAppID;
        public string appIconPath;
        public Texture2D appIcon => AssetDatabase.LoadAssetAtPath<Texture2D>(appIconPath);
        
        public S3CannedACL permissions;

        public ProfileAndBucketProperties (string iamAccessKeyId, string iamSecretKey, string profileName, string standaloneIosAppID, string appIconPath, S3CannedACL permissions) {
            this.iamAccessKeyId = iamAccessKeyId;
            this.iamSecretKey = iamSecretKey;
            this.profileName = profileName;
            this.standaloneIosAppID = standaloneIosAppID;
            this.appIconPath = appIconPath;
            this.permissions = permissions;
        }

        public AmazonS3Client CreateClient () {
            var credentials = new BasicAWSCredentials(iamAccessKeyId, iamSecretKey);
            var bucketRegion = RegionEndpoint.GetBySystemName(regionString);
            var s3Config = new AmazonS3Config
            {
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
        
        public string GetFormattedServerPath (string buildTarget) {
            return AddressableAssetSettingsDefaultObject.Settings.profileSettings.EvaluateString(profileID, remoteBucketFolderRoot.Replace("[BuildTarget]", buildTarget));
        }
        public string GetFormattedServerAddressablesPath (string buildTarget, string filePath) {
            return AddressableAssetSettingsDefaultObject.Settings.profileSettings.EvaluateString(profileID, remoteAddressablesPath.Replace("[BuildTarget]", buildTarget).Replace("(FilePath)", filePath));
        }

        // Gets the path that Addressables should build the addressable state .bin file to.
        public string GetTargetAddressablesOutputBinPath () {
            return Path.Combine(Application.dataPath, "AddressableAssetsData", "Content States", bucketName, PlatformMappingService.GetPlatformPathSubFolder(), "addressables_content_state.bin").Replace("\\","/");
        }

        // Gets the build output path
        // On Windows we build an extra folder to contain the build, since additional files are placed in the same folder.
        // On OSX the .app is basically a folder in itself and on other platforms builds output to a folder
        public string GetBuildPath (BuildTarget buildTarget) {
            string buildDirectoryPath = string.Empty;
            string buildPath = string.Empty;
            var appName = PlayerSettings.productName.Replace(" ","_");

            buildDirectoryPath = Path.Combine(defaultDir, bucketName);
            buildDirectoryPath = Path.GetFullPath(Path.Combine(buildDirectoryPath, GetBuildTargetString(buildTarget)));
            buildDirectoryPath = Path.Combine(buildDirectoryPath, BuildInfo.Instance.version.ToString());

            if(buildTarget == BuildTarget.StandaloneWindows || buildTarget == BuildTarget.StandaloneWindows64) {
                buildPath = Path.Combine(buildDirectoryPath, appName, Path.ChangeExtension(appName, ".exe"));
            } else if(buildTarget == BuildTarget.StandaloneOSX) {
                buildPath = Path.Combine(buildDirectoryPath, Path.ChangeExtension(appName, ".app"));
            } else if(buildTarget == BuildTarget.WebGL) {
                buildPath = Path.Combine(buildDirectoryPath, "WebGL");
            } else {
                buildPath = Path.Combine(buildDirectoryPath, appName);
            }
            return buildPath.Replace("\\","/");
        }

        public string GetBuildURL (BuildTarget buildTarget) {
            return "https://"+bucketName+".s3."+regionString+".amazonaws.com/"+GetFormattedServerPath(buildTarget.ToString())+"/build/index.html";
        }

        // Utility to get a path relative to another path
        public static string GetRelativePath (string path, string rootPath) {
            return path.Substring(rootPath.Length).Replace("\\","/");
        }

        // Gets the path of the ServerData folder that a Addressable builds for each platform are copied into. 
        public static string GetBuildOutputPath (string bucketName) {
            return Path.Combine(Directory.GetCurrentDirectory(), "ServerData", bucketName).Replace("\\","/")+"/";
        }
        // Gets the path of the ServerData folder that an Addressable build for a given platform is copied into. 
        public static string GetBuildOutputPathForPlatform (string bucketName, string platform) {
            return Path.Combine(Directory.GetCurrentDirectory(), "ServerData", bucketName, platform).Replace("\\","/")+"/";
        }
    }
    
    static string ProfileNameToBucketName (string profileName) {
        foreach(var p in profileAndBucketProperties)
            if(p.profileName == profileName) 
                return p.bucketName;
        return "default";
    }
    static string BucketNameToProfileName (string bucketName) {
        foreach(var p in profileAndBucketProperties)
            if(p.bucketName == bucketName) 
                return p.profileName;
        return "default";
    }
    static string ProfileIDToBucketName (string profileID) {
        var profileName = AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetProfileName(profileID);
        return ProfileNameToBucketName(profileName);
    }
    static ProfileAndBucketProperties GetProfilePropertiesFromProfileID (string profileID) {
        foreach(var p in profileAndBucketProperties)
            if(p.profileID == profileID) 
                return p;
        return null;
    }
    public static ProfileAndBucketProperties GetProfilePropertiesFromBucketName (string bucketName) {
        foreach(var p in profileAndBucketProperties)
            if(p.bucketName == bucketName) 
                return p;
        return null;
    }
    */
    
    
    static async Task UploadAddressables () {
        uploadAddressablesProgress.Start("Preparing Addressables upload...");

        var buildTargetPath = GetBuildTargetString(targetBuildPlatform.buildTarget);
        var bucketPlatformAddressablesFolderPath = ProfileAndBucketProperties.GetBuildOutputPathForPlatform(activeProfileProperties.bucketName, buildTargetPath);
        if(Directory.Exists(bucketPlatformAddressablesFolderPath)) {
            var files = new DirectoryInfo(bucketPlatformAddressablesFolderPath).GetFiles("*", SearchOption.AllDirectories);
            
            var s3Client = activeProfileProperties.CreateClient();
            var fileStatuses = GetFileStatuses(files, bucketPlatformAddressablesFolderPath, buildTargetPath);
            
            if(addressablesUploaderWindow == null) addressablesUploaderWindow = ServerHostedFileWindow.Init();
            addressablesUploaderWindow.SetFiles(activeProfileProperties, fileStatuses);
            
            foreach(var fileStatus in fileStatuses) 
                if(fileStatus.requiresCheck) 
                    fileStatus.status = ServerHostedFileStatus.Status.QueuedForCheck;
            await AWSUtils.RefreshStatusOfFiles(s3Client, activeProfileProperties, fileStatuses, false, (progress) => {
                uploadAddressablesProgress.Update("Checking status "+(progress*100)+"%", progress*0.01f);
            });

            var filesToUpload = fileStatuses.Where(x => x.requiresUpload);
            await AWSUtils.UploadFiles(activeProfileProperties, filesToUpload.ToArray(), (progress) => {
                uploadAddressablesProgress.Update("Uploading "+(progress*100)+"%", 1+progress*0.99f);
            });
            uploadAddressablesProgress.Complete("Uploaded Addressables!");
        } else {
            uploadAddressablesProgress.Fail("Failed uploading addressables because path "+bucketPlatformAddressablesFolderPath+" was empty!");
        }
    }
#endif
    }
}