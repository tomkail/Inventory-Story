using UnityEditor;
using UnityEngine;

namespace BuildPipeline {
    public class BuildAddressablesStep : BuildPipelineStep {
        public bool forceNewAddressablesBuild;
        public BuildAddressablesStep() {
            name = "Build Addressables";
        }

        public override void DrawSettings() {
            forceNewAddressablesBuild = EditorGUILayout.Toggle(new GUIContent("Reset Addressables Build State", "If true, instead of updating the current Addressables state, building Addressables will create a new state. Keep this false unless you know what you're doing."), forceNewAddressablesBuild);
        }
        
        
    #if ADDRESSABLES
        static bool UpdateAddressables () {
            // BuildScript.buildCompleted += (AddressableAssetBuildResult r) => {
            //     Debug.Log("COMPELTE "+r.OutputPath);
            // };
            // Debug.Log(Addressables.BuildPath);
            // Debug.Log(ContentUpdateScript.GetContentStateDataPath(false));
            
            var path = activeProfileProperties.GetTargetAddressablesOutputBinPath();
            
            // Get the AddressableAssetSettings file
            var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            addressableSettings.OptimizeCatalogSize = !BuildPipelineEditorWindowSettings.Instance.settings.development;

            // for (int i = 0; i < addressableAssetSettings.DataBuilders.Count; i++)
            // {
            //     var m = addressableAssetSettings.GetDataBuilder(i);
            //     if (m.CanBuildData<AddressablesPlayerBuildResult>())
            //     {
            //         Debug.Log(m.Name);
            //         Debug.Log(AssetDatabase.GetAssetPath((UnityEngine.Object)m));
            //         // AddressablesPlayerBuildResultBuilderExists = true;
            //         // menu.AddItem(new GUIContent("New Build/" + m.Name), false, OnBuildScript, i);
            //     }
            // }
            // return;
            // 
            
            // var id = addressableAssetSettings.profileSettings.GetProfileId(desiredProfileSelection);
            // addressableAssetSettings.activeProfileId = id;
            // Debug.Log("changing profileID to " + addressableAssetSettings.activeProfileId);
            
            // var path = ContentUpdateScript.GetContentStateDataPath(true);
            if (!forceNewAddressablesBuild && File.Exists(path)) {
                return UpdateAddressableContent(path, addressableSettings);
            } else {
                return BuildNewAddressableContent(path, addressableSettings);
            }
        }

        static bool UpdateAddressableContent (string path, AddressableAssetSettings addressableAssetSettings) {
            buildAddressablesProgress.Start("Updating Addressables Content "+addressableAssetSettings+" at "+path);
            var result = ContentUpdateScript.BuildContentUpdate(addressableAssetSettings, path);
            var output = false;
            if(result == null) {
                Debug.LogError("Failed to build content update!\nBuilding new content instead.");
                output = BuildNewAddressableContent(path, addressableAssetSettings);
            } else if (result.Error != null) {
                Debug.LogError(result.Error+"\nBuilding new content instead.");
                output = BuildNewAddressableContent(path, addressableAssetSettings);
            } else {
                output = true;
            }
            buildAddressablesProgress.Complete("Updated Addressables Content at "+addressableAssetSettings.RemoteCatalogBuildPath.GetValue(addressableAssetSettings));
            return output;
        }

        static bool BuildNewAddressableContent (string path, AddressableAssetSettings addressableAssetSettings) {
            buildAddressablesProgress.Start("Creating new Addressables Content at " +addressableAssetSettings.RemoteCatalogBuildPath.GetValue(addressableAssetSettings));

            // Delete existing addressable files, since they're no longer required on the server (we may also wish to remove them from the server, but they don't do much harm and removing them might prevent live builds working)
            var buildTargetPath = GetBuildTargetString(targetBuildPlatform.buildTarget);
            var bucketPlatformAddressablesFolderPath = ProfileAndBucketProperties.GetBuildOutputPathForPlatform(activeProfileProperties.bucketName, buildTargetPath);
            DeleteAllContents(new DirectoryInfo(bucketPlatformAddressablesFolderPath), false);

            try {
                AddressableAssetSettings.BuildPlayerContent();
            }
            // THIS DOESNT CATCH! ARGH! 
            catch {
                Debug.Log("BUILD PLAYER CONTENT FAILED!");
                return false;
            }

            // Ideally the build script would just put it here but eh.
            var outputBinDirPath = new FileInfo(path).Directory;
            // Delete any existing data, because we're totally rebuilding!
            if(outputBinDirPath.Exists)
                outputBinDirPath.Delete(true);
            var d = Directory.CreateDirectory(outputBinDirPath.FullName);
            // Where addressable content state is built to by default. We move them from here. 
            // Path_To_Project/Assets/AddressableAssetsData/PlatformMappingService.GetPlatformPathSubFolder()/addressables_content_state.bin
            var builtAddressablesBinPath = Path.Combine(Application.dataPath, "AddressableAssetsData", PlatformMappingService.GetPlatformPathSubFolder(), "addressables_content_state.bin");
            
            if(!File.Exists(builtAddressablesBinPath)) 
                return false;
            
            // Where we'd like to move the content state to. This enables us to store states for several buckets/versions.
            // Path_To_Project/Assets/AddressableAssetsData/Content States/bucketName/PlatformMappingService.GetPlatformPathSubFolder()/addressables_content_state.bin
            var targetAddressablesBinPath = outputBinDirPath.FullName+"/addressables_content_state.bin";
            // Debug.Log(builtAddressablesBinPath+"\n"+targetAddressablesBinPath);
            File.Move(builtAddressablesBinPath, targetAddressablesBinPath);

            buildAddressablesProgress.Complete("Built new Addressables Content at "+addressableAssetSettings.RemoteCatalogBuildPath.GetValue(addressableAssetSettings));
            return true;
        }
    #endif
    }
}
