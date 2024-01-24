﻿using System;
using System.IO;
using UnityEngine;

public static class SaveLoadManager {
    public static SaveVersion saveVersion;
    public static string fileName = "DeadDropGameSave";
    public static string fileExtension = "json";

    public static string saveFileNameAndExt {
		get {
			var fullFileName = fileName;
			fullFileName += "."+fileExtension;
			return fullFileName;
		}
	}
	public static string saveDirectory => Application.persistentDataPath;

	public static string fullSavePath => Path.Combine(saveDirectory, saveFileNameAndExt);

	public static bool canLoadState => true; //Application.isPlaying; // && !RuntimeSceneSetLoader.Instance.loading;

	public static bool canSave => Application.isPlaying;

	public static string autoDevSaveDirectory => Path.GetFullPath(Path.Combine(ApplicationX.projectPath, "Auto Dev Saves"));

	public static string defaultManualDevSaveDirectory => Path.GetFullPath(Path.Combine(ApplicationX.projectPath, "Manual Dev Saves"));

	public static Func<string> RequestGameSaveJSON;
	public static Action<string> OnLoadGameSaveState;

    // public static event Action onTrySaveState;
	// public static event Action onWillSaveState;
	// public static event Action<SaveResultInfo> onDidSaveState;
	// public static event Action<ClearResultInfo> onDidClearState;

    public static void Save () {
        var saveStateJSON = GetSaveStateJSON();

        WriteToSaveFile(saveStateJSON);
        #if UNITY_EDITOR
        WriteToDevSaves(saveStateJSON);
        #endif
        Debug.Log("SAVED GAME");
    }

    static void WriteToSaveFile (string saveStateJSON) {
        var backupSavePath = PathX.GetFullPathWithNewFileName(fullSavePath, Path.GetFileNameWithoutExtension(fullSavePath)+"_Temp");
		if(File.Exists(backupSavePath)) File.Delete(backupSavePath);
        
		File.WriteAllText(backupSavePath, saveStateJSON);

		if(File.Exists(fullSavePath)) File.Delete(fullSavePath);
		File.Move(backupSavePath, fullSavePath);
    }
    
    public static string ReadFromSaveFile () {
	    return !File.Exists(fullSavePath) ? null : File.ReadAllText(fullSavePath);
    }

    static void WriteToDevSaves (string saveStateJSON) {
		DateTime dateTime = DateTime.Now;
        var gameStateString = "AutoSave";
        var dateTimeString = dateTime.Year+"_"+dateTime.Month+"_"+dateTime.Day+"_"+dateTime.Hour.ToString("00")+"_"+dateTime.Minute.ToString("00")+"_"+dateTime.Second.ToString("00");
        var fileName = string.Format("{0}_{1}.txt", gameStateString, dateTimeString);
		if(!Directory.Exists(autoDevSaveDirectory)) Directory.CreateDirectory(autoDevSaveDirectory);
        var devSavePath = Path.Combine(autoDevSaveDirectory, Path.GetFileNameWithoutExtension(fileName)+"."+fileExtension);
		if(File.Exists(devSavePath)) return;
		using (StreamWriter sw = File.CreateText(devSavePath)) sw.Write(saveStateJSON);
    }

    public static string GetSaveStateJSON () {
        return JsonUtility.ToJson(CreateSaveState());
    }


    public static SaveState CreateSaveState () {
        SaveState saveState = new SaveState();
        
        System.Text.StringBuilder saveDescriptionSB = new System.Text.StringBuilder();
        // saveDescriptionSB.AppendLine("Game state is "+GameController.Instance.gameModel.levelName+" turn "+GameController.Instance.gameModel.currentTurn+". ");
        saveState.saveDescription = saveDescriptionSB.ToString();

        var gameMetaInfo = new GameMetaInformation();
        gameMetaInfo.gameName = Application.productName;
        
        var saveMetaInfo = new SaveMetaInformation();
        saveMetaInfo.saveVersion = new SaveVersion(SaveVersion.buildSaveVersion);
        saveMetaInfo.saveDateTime = System.DateTime.Now;

        saveState.gameMetaInformationJSON = JsonUtility.ToJson(gameMetaInfo);
        saveState.saveMetaInformationJSON = JsonUtility.ToJson(saveMetaInfo);
        saveState.gameJSON = RequestGameSaveJSON();
        
        return saveState;
    }

    public static void LoadSaveState (string saveStateJSON) {
        Debug.Assert(canLoadState);

        if (string.IsNullOrEmpty(saveStateJSON)) throw new InvalidSaveException("Save state JSON is null or empty");
        SaveState saveState = JsonUtility.FromJson<SaveState>(saveStateJSON);

        // GameMetaInformation gameMetaInfo = JsonUtility.FromJson<GameMetaInformation>(saveState.gameMetaInformationJSON);
        SaveMetaInformation saveMetaInfo = JsonUtility.FromJson<SaveMetaInformation>(saveState.saveMetaInformationJSON);
        if(saveMetaInfo.saveVersion.version < SaveVersion.minCompatableSaveVersion) {
	        throw new InvalidSaveException("Can't load save because save version "+saveMetaInfo.saveVersion.version+" is less than minimum compatible version "+SaveVersion.minCompatableSaveVersion);
        }
        
        OnLoadGameSaveState(saveState.gameJSON);
    }
    
    public class InvalidSaveException : Exception
    {
	    public InvalidSaveException() { }

	    public InvalidSaveException(string message) : base(message) { }

	    public InvalidSaveException(string message, Exception inner) : base(message, inner) { }
    }
}