using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityX.Geometry;
using UnityEditorInternal;

public class SaveEditorWindow : EditorWindow {
	[System.Serializable]
	public class SaveEditorWindowSettings : SerializedEditorSettings<SaveEditorWindowSettings> {
        public Vector2 scrollPosition;
        public Vector2 saveScrollPosition;
        public Vector2 autoDevScrollPosition;
        public Vector2 manualDevScrollPosition;
        public string saveDirectory;
        public string manualSaveFileName;
        public string selectedFile;
    }
    public static string selectedFileJSON;
    public static SaveState selectedFileSaveState;

	private const string windowTitle = "Saves";
	
	[MenuItem(GameEditorUtils.menuItemPath+"/Saves", false, 2400)]
	static void Init () {
		SaveEditorWindow window = EditorWindow.GetWindow(typeof(SaveEditorWindow), false, windowTitle) as SaveEditorWindow;
		window.titleContent = new GUIContent(windowTitle);
        OnSetSelectedFile();
	}

	void OnGUI () {
		Repaint();
		GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
		SaveEditorWindowSettings.Instance.scrollPosition = GUILayout.BeginScrollView(SaveEditorWindowSettings.Instance.scrollPosition);
		
		DoSaveStates();
        GUILayout.Space(30);
        DrawSelectedSave();

		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		SaveEditorWindowSettings.Save();
	}

	void DoSaveStates () {
		GUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(false);
        if (GUILayout.Button("Copy To Clipboard")) {
            GUIUtility.systemCopyBuffer = SaveLoadManager.GetSaveStateJSON();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(GUIUtility.systemCopyBuffer == string.Empty);
		if(GUILayout.Button("Load from clipboard")) {
            SaveLoadManager.LoadSaveState(GUIUtility.systemCopyBuffer);
		}
		EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();
        

        GUILayout.Space(30);

		FolderPathDrawer.FolderPathLayout(SaveLoadManager.saveDirectory, "Default Saves", FolderPathAttribute.RelativeTo.Root, false, false);
        if(Directory.Exists(SaveEditorWindowSettings.Instance.saveDirectory)) DrawFilesInDirectory(SaveLoadManager.saveDirectory, ref SaveEditorWindowSettings.Instance.saveScrollPosition);
        else Directory.CreateDirectory(SaveLoadManager.saveDirectory);
        
        GUILayout.Space(30);


		FolderPathDrawer.FolderPathLayout(SaveLoadManager.autoDevSaveDirectory, "Auto Dev Saves", FolderPathAttribute.RelativeTo.Root, false, false);
        if(Directory.Exists(SaveLoadManager.autoDevSaveDirectory)) DrawFilesInDirectory(SaveLoadManager.autoDevSaveDirectory, ref SaveEditorWindowSettings.Instance.autoDevScrollPosition, 6);
        else Directory.CreateDirectory(SaveLoadManager.autoDevSaveDirectory);
        if(GUILayout.Button("Clear")) DirectoryX.DeleteAllContents(new DirectoryInfo(SaveLoadManager.autoDevSaveDirectory), false);
        GUILayout.Space(30);


        if(string.IsNullOrWhiteSpace(SaveEditorWindowSettings.Instance.saveDirectory)) {
            SaveEditorWindowSettings.Instance.saveDirectory = SaveLoadManager.defaultManualDevSaveDirectory;
            if(!Directory.Exists(SaveEditorWindowSettings.Instance.saveDirectory)) Directory.CreateDirectory(SaveEditorWindowSettings.Instance.saveDirectory);
        }
		SaveEditorWindowSettings.Instance.saveDirectory = FolderPathDrawer.FolderPathLayout(SaveEditorWindowSettings.Instance.saveDirectory, "Manual Dev Saves", FolderPathAttribute.RelativeTo.Root);
		if(Directory.Exists(SaveEditorWindowSettings.Instance.saveDirectory)) {
			DrawFilesInDirectory(SaveEditorWindowSettings.Instance.saveDirectory, ref SaveEditorWindowSettings.Instance.manualDevScrollPosition, 6);
            if(GUILayout.Button("Clear")) DirectoryX.DeleteAllContents(new DirectoryInfo(SaveEditorWindowSettings.Instance.saveDirectory), false);
		} else {
			EditorGUILayout.HelpBox("No folder selected", MessageType.Info);
		}
        
        GUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(!canSave);
        if (GUILayout.Button("Auto Save")) {
            var dateTime = System.DateTime.Now;
            var fileName = string.Format("AutoSave_{0}", dateTime.Year+"_"+dateTime.Month+"_"+dateTime.Day+"_"+dateTime.Hour.ToString("00")+"_"+dateTime.Minute.ToString("00")+"_"+dateTime.Second.ToString("00"));
            CreateStoryStateTextFile(SaveLoadManager.GetSaveStateJSON(), fileName);
        }
        if (GUILayout.Button("Save As")) {
            var fileName = EditorUtility.SaveFilePanel("Save", SaveEditorWindowSettings.Instance.saveDirectory, SaveEditorWindowSettings.Instance.manualSaveFileName, SaveLoadManager.fileExtension);
            if(!fileName.IsNullOrEmpty()) {
                SaveEditorWindowSettings.Instance.manualSaveFileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
                CreateStoryStateTextFile(SaveLoadManager.GetSaveStateJSON(), SaveEditorWindowSettings.Instance.manualSaveFileName);
            }
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();
	}

    static void DrawSelectedSave () {
        if(selectedFileSaveState != null) {
            EditorGUILayout.LabelField(SaveEditorWindowSettings.Instance.selectedFile);
            // EditorGUILayout.LabelField("Game Meta Info");
            // EditorGUILayout.TextArea(selectedFileSaveState.gameMetaInfo, GUILayout.MaxHeight(100));
            //
            // EditorGUILayout.LabelField("Save Meta Info");
            // EditorGUILayout.TextArea(selectedFileSaveState.saveMetaInfo, GUILayout.MaxHeight(100));

            EditorGUILayout.LabelField("Game JSON");
            EditorGUILayout.TextArea(selectedFileSaveState.storySaveJson, GUILayout.MaxHeight(100));
        }
    }

    static void DrawFilesInDirectory (string saveDirectory, ref Vector2 scrollPosition, int maxNumToShow = 1) {
        var files = Directory.GetFiles(saveDirectory, "*"+SaveLoadManager.fileExtension);
        files = files.OrderByDescending(x => File.GetLastWriteTime(x)).ToArray();
        maxNumToShow = Mathf.Min(maxNumToShow, files.Length);

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(false), GUILayout.MaxHeight(maxNumToShow * 28));
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        foreach(var filePath in files) {
            DrawLoadableFile(filePath);
        }
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
    }

    static void DrawLoadableFile (string filePath) {
        float lastWriteTime = (float)(System.DateTime.Now - File.GetLastWriteTime(filePath)).TotalSeconds;
        var l = Mathf.InverseLerp(2, 0, lastWriteTime);
        var color = Color.Lerp(GUI.color, Color.green, l);
        OnGUIX.BeginColor(color);

        EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.ExpandWidth(true));

        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var newFileName = EditorGUILayout.TextField(fileName);
        if(newFileName != fileName) {
            var dir = Path.GetDirectoryName(filePath);
            var ext = Path.GetExtension(filePath);
            File.Move(Path.Combine(dir, fileName+ext), Path.Combine(dir, newFileName+ext));
        }

        EditorGUILayout.LabelField(File.GetLastWriteTime(filePath).ToLongTimeString(), GUILayout.Width(70));

        EditorGUI.BeginDisabledGroup(SaveEditorWindowSettings.Instance.selectedFile == filePath);
        if(GUILayout.Button("Select", GUILayout.Width(48))) {
            SaveEditorWindowSettings.Instance.selectedFile = filePath;
            OnSetSelectedFile();
        }
        EditorGUI.EndDisabledGroup();

        if(GUILayout.Button("Info", GUILayout.Width(40))) {
            EditorUtility.DisplayDialog("Save Info", GetFileInfo(filePath), "Ok", null);
        }
        EditorGUI.BeginDisabledGroup(!SaveLoadManager.canLoadState);
        if(GUILayout.Button("Load", GUILayout.Width(40))) {
            var text = File.ReadAllText(filePath);
            SaveLoadManager.LoadSaveState(text);
        }
        EditorGUI.EndDisabledGroup();
        if(GUILayout.Button("Delete", GUILayout.Width(48))) {
            File.Delete(filePath);
        }
        if(GUILayout.Button("Open", GUILayout.Width(40))) {
            System.Diagnostics.Process.Start(filePath);
        }
        EditorGUILayout.EndHorizontal();
        OnGUIX.EndColor();
    }

    static void OnSetSelectedFile () {
        if(SaveEditorWindowSettings.Instance.selectedFile != null && File.Exists(SaveEditorWindowSettings.Instance.selectedFile)) {
            selectedFileJSON = File.ReadAllText(SaveEditorWindowSettings.Instance.selectedFile);
            selectedFileSaveState = JsonUtility.FromJson<SaveState>(selectedFileJSON);
            // selectedFileSaveState.gameMetaInformationJSON = JsonBeautifier.FormatJson(selectedFileSaveState.gameMetaInformationJSON);
            // selectedFileSaveState.saveMetaInformationJSON = JsonBeautifier.FormatJson(selectedFileSaveState.saveMetaInformationJSON);
            selectedFileSaveState.storySaveJson = JsonBeautifier.FormatJson(selectedFileSaveState.storySaveJson);
        } else {
            selectedFileJSON = null;
            selectedFileSaveState = null;
        }
    }

	public static string CreateStoryStateTextFile (string jsonStoryState, string fileName) {
		string fullPathName = Path.Combine(SaveEditorWindowSettings.Instance.saveDirectory, fileName+"."+SaveLoadManager.fileExtension);
		using (StreamWriter outfile = new StreamWriter(fullPathName)) {
			outfile.Write(jsonStoryState);
		}
		return fullPathName;
	}

    public static string GetFileInfo (string filePath) {
        SaveState saveState = JsonUtility.FromJson<SaveState>(File.ReadAllText(filePath));
        return saveState.saveDescription;
    }

    public bool canSave {
        get {
            return SaveLoadManager.canSave;
        }
    }
}