using UnityEngine;
using UnityEditor;
using System;
using System.CodeDom;
using System.Linq;
using System.IO;

public class SaveEditorWindow : EditorWindow {
    const float fileFieldHeight = 20;
	[System.Serializable]
	public class SaveEditorWindowSettings : SerializedEditorSettings<SaveEditorWindowSettings> {
        public Vector2 scrollPosition;
        public Vector2 saveScrollPosition;
        
        public bool showingAutoDevSaves = true;
        public Vector2 autoDevScrollPosition;

        public bool showingManualDevSaves = true;
        public Vector2 manualDevScrollPosition;
        public string manualSaveFileName;
        
        public bool showingSelectedSave = true;

        public string selectedFile;
        public string manualSaveDirectory;
    }
    
    static SaveFolder gameSave;
    static SaveFolder autoDev;
    static SaveFolder manualDev;
    
    class SaveFolder {
        public string directoryPath;
        public DirectoryInfo directoryInfo;
        public FileInfo[] fileInfos;
        FileSystemWatcher watcher;
        public Action OnChange;

        static string searchPattern => "*."+SaveLoadManager.fileExtension;

        public SaveFolder (string directoryPath) {
            this.directoryPath = directoryPath;
            directoryInfo = new DirectoryInfo(directoryPath);
            RefreshFiles();
            // On OSX this causes an exception on exiting play mode.
            watcher = new FileSystemWatcher(directoryPath);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = searchPattern;
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnRenamed;
            watcher.EnableRaisingEvents = true;
        }

        public void RefreshFiles () {
            if(!directoryInfo.Exists) Debug.LogError("No directory at "+directoryInfo.FullName);
            fileInfos = directoryInfo.GetFiles(searchPattern);
            fileInfos = fileInfos.OrderByDescending(p => p.CreationTime).ToArray();
        }

        void OnChanged(object source, FileSystemEventArgs e) {
            RefreshFiles();
            OnChange?.Invoke();
        }
        void OnRenamed(object source, RenamedEventArgs e) {
            RefreshFiles();
            OnChange?.Invoke();
        }

        public void Dispose() {
            if (watcher == null) return;
            watcher.Changed -= OnChanged;
            watcher.Created -= OnChanged;
            watcher.Deleted -= OnChanged;
            watcher.Renamed -= OnRenamed;
            watcher.Dispose();
            watcher = null;
        }
    }

    public static string selectedFileJSON;
    public static SaveState selectedFileSaveState;

    const string windowTitle = "Saves";
	
	[MenuItem(GameEditorUtils.menuItemPath+"/Saves", false, 2400)]
	static void Init () {
		SaveEditorWindow window = EditorWindow.GetWindow(typeof(SaveEditorWindow), false, windowTitle) as SaveEditorWindow;
		window.titleContent = new GUIContent(windowTitle);
	}

    SaveEditorWindow () {
        EditorApplication.delayCall += () => {
            OnSetSelectedFile();
        };
    }

    void OnEnable() {
        CreateFolderTrackersIfMissing();
    }

    void CreateFolderTrackersIfMissing () {
        if(gameSave == null && Directory.Exists(SaveLoadManager.saveDirectory)) {
            gameSave = new SaveFolder(SaveLoadManager.saveDirectory);
            gameSave.OnChange += Repaint;
        }    
        if(autoDev == null && Directory.Exists(SaveLoadManager.autoDevSaveDirectory)) {
            autoDev = new SaveFolder(SaveLoadManager.autoDevSaveDirectory);
            autoDev.OnChange += Repaint;
        }    
        if(manualDev == null && Directory.Exists(SaveEditorWindowSettings.Instance.manualSaveDirectory)) {
            manualDev = new SaveFolder(SaveEditorWindowSettings.Instance.manualSaveDirectory);
            manualDev.OnChange += Repaint;
        }    
    }
    void OnDisable () {
        Deconstruct();
    }
    
    void OnDestroy () {
        Deconstruct();
    }

    void OnFocus() {
        gameSave?.RefreshFiles();
        autoDev?.RefreshFiles();
        manualDev?.RefreshFiles();
    }

    object _lock = new object();
    void Deconstruct() {
        lock (_lock) {
            if (gameSave != null) {
                gameSave.OnChange -= Repaint;
                gameSave.Dispose();
                gameSave = null;
            }

            if (autoDev != null) {
                autoDev.OnChange -= Repaint;
                autoDev.Dispose();
                autoDev = null;
            }

            if (manualDev != null) {
                manualDev.OnChange -= Repaint;
                manualDev.Dispose();
                manualDev = null;
            }
        }
    }

    static DateTime dateTimeNow;
            
	void OnGUI () {
        dateTimeNow = System.DateTime.Now;
        Repaint();
        
		EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
		SaveEditorWindowSettings.Instance.scrollPosition = EditorGUILayout.BeginScrollView(SaveEditorWindowSettings.Instance.scrollPosition);
		DoSaveStates();
        GUILayout.Space(30);
        DrawSelectedSave();

		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
		SaveEditorWindowSettings.Save();
	}

	void DoSaveStates () {
		EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(!canSave);        
        if (GUILayout.Button("Copy To Clipboard")) {
            GUIUtility.systemCopyBuffer = SaveLoadManager.GetSaveStateJSON();
        }

        EditorGUI.BeginDisabledGroup(GUIUtility.systemCopyBuffer == string.Empty);
		if(GUILayout.Button("Load from clipboard")) {
            SaveLoadManager.LoadSaveState(GUIUtility.systemCopyBuffer);
		}
		EditorGUI.EndDisabledGroup();
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        
        DrawDefaultSaves();
        DrawAutoSaves();
        DrawManualSaves();
	}

    void DrawDefaultSaves () {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Default Saves");
        DrawClearButton(gameSave);
        EditorGUILayout.EndHorizontal();
		
        FolderPathDrawer.FolderPathLayout(SaveLoadManager.saveDirectory, "Default Saves", FolderPathAttribute.RelativeTo.Root, false, false);
        DrawFilesInDirectory(gameSave, ref SaveEditorWindowSettings.Instance.saveScrollPosition, 1, false, false, true);
        EditorGUILayout.EndVertical();
    }

    void DrawAutoSaves () {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
        SaveEditorWindowSettings.Instance.showingAutoDevSaves = EditorGUILayout.Foldout(SaveEditorWindowSettings.Instance.showingAutoDevSaves, "Auto Dev Saves", true);
        DrawClearButton(autoDev);
        EditorGUILayout.EndHorizontal();

        if(SaveEditorWindowSettings.Instance.showingAutoDevSaves) {
            FolderPathDrawer.FolderPathLayout(SaveLoadManager.autoDevSaveDirectory, GUIContent.none, FolderPathAttribute.RelativeTo.Root, false, false);
            if(!Directory.Exists(SaveLoadManager.autoDevSaveDirectory)) {
                Directory.CreateDirectory(SaveLoadManager.autoDevSaveDirectory);
                if(autoDev != null) autoDev.Dispose();
                autoDev = new SaveFolder(SaveLoadManager.autoDevSaveDirectory);
                EditorGUIUtility.ExitGUI();
            }
            DrawFilesInDirectory(autoDev, ref SaveEditorWindowSettings.Instance.autoDevScrollPosition, 6, true, true, true);
        }
        EditorGUILayout.EndVertical();
    }

    void DrawManualSaves () {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
        SaveEditorWindowSettings.Instance.showingManualDevSaves = EditorGUILayout.Foldout(SaveEditorWindowSettings.Instance.showingManualDevSaves, "Manual Dev Saves", true);

        EditorGUI.BeginDisabledGroup(!canSave);
        if (GUILayout.Button("Auto Save", EditorStyles.toolbarButton, GUILayout.Width(80))) {
            var fileName = string.Format("AutoSave_{0}", dateTimeNow.Year+"_"+dateTimeNow.Month+"_"+dateTimeNow.Day+"_"+dateTimeNow.Hour.ToString("00")+"_"+dateTimeNow.Minute.ToString("00")+"_"+dateTimeNow.Second.ToString("00"));
            MoveToManualSaves(autoDev.fileInfos.First().FullName);
            EditorGUIUtility.ExitGUI();
        }
        if (GUILayout.Button("Save As", EditorStyles.toolbarButton, GUILayout.Width(60))) {
            var fileName = EditorUtility.SaveFilePanel("Save", SaveEditorWindowSettings.Instance.manualSaveDirectory, SaveEditorWindowSettings.Instance.manualSaveFileName, SaveLoadManager.fileExtension);
            if(!fileName.IsNullOrEmpty()) {
                SaveEditorWindowSettings.Instance.manualSaveFileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
                MoveToManualSaves(autoDev.fileInfos.First().FullName);
                EditorGUIUtility.ExitGUI();
            }
        }
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(10);
        EditorGUI.BeginDisabledGroup(!Directory.Exists(SaveEditorWindowSettings.Instance.manualSaveDirectory));
        DrawClearButton(manualDev);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndHorizontal();

        if(SaveEditorWindowSettings.Instance.showingManualDevSaves) {
            var oldManualSaveDirectory = SaveEditorWindowSettings.Instance.manualSaveDirectory;
            if(string.IsNullOrWhiteSpace(SaveEditorWindowSettings.Instance.manualSaveDirectory)) {
                SaveEditorWindowSettings.Instance.manualSaveDirectory = SaveLoadManager.defaultManualDevSaveDirectory;
                if(!Directory.Exists(SaveEditorWindowSettings.Instance.manualSaveDirectory)) {
                    Directory.CreateDirectory(SaveEditorWindowSettings.Instance.manualSaveDirectory);
                }
            }
            SaveEditorWindowSettings.Instance.manualSaveDirectory = FolderPathDrawer.FolderPathLayout(SaveEditorWindowSettings.Instance.manualSaveDirectory, GUIContent.none, FolderPathAttribute.RelativeTo.Root);
            if(SaveEditorWindowSettings.Instance.manualSaveDirectory != oldManualSaveDirectory && Directory.Exists(SaveEditorWindowSettings.Instance.manualSaveDirectory)) {
                if(manualDev != null) manualDev.Dispose();
                manualDev = new SaveFolder(SaveEditorWindowSettings.Instance.manualSaveDirectory);
                EditorGUIUtility.ExitGUI();
            }
            
            if(Directory.Exists(SaveEditorWindowSettings.Instance.manualSaveDirectory)) {
                DrawFilesInDirectory(manualDev, ref SaveEditorWindowSettings.Instance.manualDevScrollPosition, 6, true, true, false);
            } else {
                EditorGUILayout.HelpBox("No valid folder selected", MessageType.Info);
            }
        }
        EditorGUILayout.EndVertical();
    }

    static void DrawClearButton (SaveFolder saveFolder) {
        EditorGUI.BeginDisabledGroup(saveFolder == null || saveFolder.fileInfos.IsNullOrEmpty());
        if(GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(42))) {
            DirectoryX.DeleteAllContents(saveFolder.directoryInfo, false);
            saveFolder.RefreshFiles();
            EditorGUIUtility.ExitGUI();
        }
        EditorGUI.EndDisabledGroup();
    }
    static void DrawSelectedSave () {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
        SaveEditorWindowSettings.Instance.showingSelectedSave = EditorGUILayout.Foldout(SaveEditorWindowSettings.Instance.showingSelectedSave, "Selected Save", true);
        if(GUILayout.Button("Deselect", EditorStyles.toolbarButton)) {
            SaveEditorWindowSettings.Instance.selectedFile = null;
        }
        EditorGUILayout.EndHorizontal();
        if(SaveEditorWindowSettings.Instance.showingSelectedSave) {
            if(selectedFileSaveState != null) {
                FilePathDrawer.FilePathLayout(SaveEditorWindowSettings.Instance.selectedFile, "Selected File", FilePathAttribute.RelativeTo.Root, false, false);
                DrawSelectedSaveField("Description", selectedFileSaveState.saveDescription);
                // DrawSelectedSaveField("Game Meta Info", selectedFileSaveState.gameMetaInformationJSON);
                // DrawSelectedSaveField("Save Meta Info", selectedFileSaveState.saveMetaInformationJSON);
                // DrawSelectedSaveField("Game JSON", selectedFileSaveState.gameJSON);
                // DrawSelectedSaveField("Story JSON", selectedFileSaveState.storyJSON);
            } else {
                EditorGUILayout.LabelField("No save selected");
            }
        }
        EditorGUILayout.EndVertical();
    }
    
    static void DrawSelectedSaveField (string title, string content, float height = 80) {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        EditorGUILayout.LabelField(title);
        if(GUILayout.Button("Copy", EditorStyles.toolbarButton)) GUIUtility.systemCopyBuffer = content;
        EditorGUILayout.EndHorizontal();
        GUILayout.TextArea(content, GUILayout.Height(height));
    }
    static float extraHeight = 9;
    static float spacing = 5;

    static void DrawFilesInDirectory (SaveFolder saveFolder, ref Vector2 scrollPosition, int maxNumToShow = 1, bool editableNames = true, bool canReplaceMainSave = true, bool canMoveToManualSaves = true) {
        var files = saveFolder.fileInfos;
        if(files == null || files.Length == 0) return;
        
        maxNumToShow = Mathf.Min(maxNumToShow, files.Length);
        // extraHeight = GUILayout.FloatField(extraHeight.ToString(), extraHeight);
        // GUILayout.Label(scrollPosition.y.ToString());
        // GUILayout.Label(scrollPosition.y.ToString());
        // if(GUILayout.Button("<")) scrollPosition.y--;
        // if(GUILayout.Button(">")) scrollPosition.y++;
        float m_ItemHeight = fileFieldHeight;
        float scrollRectHeight = (maxNumToShow * m_ItemHeight) + ((maxNumToShow-1) * spacing) + extraHeight;
        int numToShow = Mathf.CeilToInt(scrollRectHeight / m_ItemHeight);
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(false), GUILayout.Height(scrollRectHeight));
        int firstIndex = (int)(scrollPosition.y / m_ItemHeight);
        firstIndex = Mathf.Clamp(firstIndex,0,Mathf.Max(0,files.Length-numToShow));
        if(firstIndex * m_ItemHeight > 0) GUILayout.Space(firstIndex * m_ItemHeight);
        var lastIndex = Mathf.Min(files.Length, firstIndex+numToShow);
        for(int i = firstIndex; i < lastIndex; i++) {
            var item = files[i];
            // EditorGUILayout.LabelField(i+" - "+firstIndex+" - "+(lastIndex-1) + " - "+(files.Length-1), GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.Height(20));
            DrawLoadableFile(saveFolder, item, editableNames, canReplaceMainSave, canMoveToManualSaves);
        }
        var numItemsOffScreenAtEnd = Mathf.Max(0, files.Length-firstIndex-numToShow);
        if(numItemsOffScreenAtEnd * m_ItemHeight > 0) GUILayout.Space(numItemsOffScreenAtEnd * m_ItemHeight);
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();
    }

    static void DrawLoadableFile (SaveFolder saveFolder, FileInfo fileInfo, bool editableNames, bool canReplaceMainSave, bool canMoveToManualSaves) {
        var lastWriteTime = fileInfo.LastWriteTime;
        float timeSinceLastWrite = (float)(dateTimeNow - lastWriteTime).TotalSeconds;
        var l = Mathf.InverseLerp(2, 0, timeSinceLastWrite);
        var color = Color.Lerp(GUI.color, Color.green, l);
        OnGUIX.BeginColor(color);

        EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.Height(fileFieldHeight));

        var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
        EditorGUI.BeginDisabledGroup(!editableNames);
        var newFileName = EditorGUILayout.TextField(fileName, GUILayout.Height(fileFieldHeight-5));
        EditorGUI.EndDisabledGroup();
        if(newFileName != fileName) {
            var dir = fileInfo.DirectoryName;
            var ext = Path.GetExtension(fileInfo.FullName);
            var targetPath = Path.Combine(dir, newFileName+ext);
            if(!File.Exists(targetPath))
                fileInfo.MoveTo(targetPath);
        }

        EditorGUILayout.LabelField(new GUIContent(lastWriteTime.ToLongTimeString(), lastWriteTime.ToLongDateString()), EditorStyles.miniBoldLabel, GUILayout.Width(48), GUILayout.Height(fileFieldHeight-5));

        EditorGUI.BeginDisabledGroup(!SaveLoadManager.canLoadState);
		if(GUILayout.Button("Load", EditorStyles.miniButton, GUILayout.Width(45), GUILayout.Height(fileFieldHeight-5))) {
            var text = File.ReadAllText(fileInfo.FullName);
            SaveLoadManager.LoadSaveState(text);
        }
        EditorGUI.EndDisabledGroup();
		if(GUILayout.Button("Delete", EditorStyles.miniButton, GUILayout.Width(45), GUILayout.Height(fileFieldHeight-5))) {
            fileInfo.Delete();
            saveFolder.RefreshFiles();
        }		
        if (GUILayout.Button("Options", EditorStyles.popup, GUILayout.Width(52))) {
			var contextMenu = new GenericMenu();
            if(canReplaceMainSave) {
                contextMenu.AddItem(new GUIContent("Replace current save"), false, ReplaceCurrentSave, fileInfo.FullName);
            }
            if(canMoveToManualSaves) {
                var manualSavePath = Path.Combine(SaveEditorWindowSettings.Instance.manualSaveDirectory, Path.GetFileName(fileInfo.FullName));
                if(!File.Exists(manualSavePath)) contextMenu.AddItem(new GUIContent("Save to manual saves"), false, MoveToManualSaves, fileInfo.FullName);
                else contextMenu.AddDisabledItem(new GUIContent("Save to manual saves"));
            }
            
            if(canReplaceMainSave || canMoveToManualSaves)
                contextMenu.AddSeparator("");

            contextMenu.AddItem(new GUIContent(SaveEditorWindowSettings.Instance.selectedFile == fileInfo.FullName ? "Deselect" : "Select"), false, ToggleSelection, fileInfo.FullName);
            contextMenu.AddItem(new GUIContent("Display Info"), false, DisplayInfo, fileInfo.FullName);

            contextMenu.AddSeparator("");
            
            contextMenu.AddItem(new GUIContent(SystemInfoX.IsWinOS ? "Open File Location":"Reveal in Finder"), false, Find, fileInfo.FullName);
            contextMenu.AddItem(new GUIContent("Open With Default Application"), false, Open, fileInfo.FullName);

            contextMenu.ShowAsContext();
			
		}
        EditorGUILayout.EndHorizontal();
        OnGUIX.EndColor();
    }


		
    static void ToggleSelection (object userData) {
        var filePath = (string)userData;
        if(SaveEditorWindowSettings.Instance.selectedFile == filePath) {
            SaveEditorWindowSettings.Instance.selectedFile = null;
            OnSetSelectedFile();
        } else {
            SaveEditorWindowSettings.Instance.selectedFile = filePath;
            OnSetSelectedFile();
        }
    }


    static void DisplayInfo (object userData) {
        var filePath = (string)userData;
        EditorUtility.DisplayDialog("Save Info", GetFileInfo(filePath), "Ok", null);
    }
    static void Find (object userData) {
        var filePath = (string)userData;
        SystemX.OpenInFileBrowser(filePath);
    }
    static void Open (object userData) {
        var filePath = (string)userData;
        System.Diagnostics.Process.Start(filePath);
    }
    static void MoveToManualSaves (object userData) {
        var filePath = (string)userData;
        MoveToManualSaves(filePath);
    }

    static void MoveToManualSaves (string filePath) {
        var newFilePath = Path.Combine(SaveEditorWindowSettings.Instance.manualSaveDirectory, Path.GetFileName(filePath));
        File.Copy(filePath, newFilePath);
        manualDev.RefreshFiles();
    }

    static void ReplaceCurrentSave (object userData) {
        var filePath = (string)userData;
        File.Copy(filePath, SaveLoadManager.fullSavePath, true);
    }

    static void OnSetSelectedFile () {
        if(SaveEditorWindowSettings.Instance.selectedFile != null && File.Exists(SaveEditorWindowSettings.Instance.selectedFile)) {
            selectedFileJSON = File.ReadAllText(SaveEditorWindowSettings.Instance.selectedFile);
            selectedFileSaveState = JsonUtility.FromJson<SaveState>(selectedFileJSON);
            selectedFileSaveState.saveDescription = JsonBeautifier.FormatJson(selectedFileSaveState.saveDescription);
            // selectedFileSaveState.gameMetaInformationJSON = JsonBeautifier.FormatJson(selectedFileSaveState.gameMetaInformationJSON);
            // selectedFileSaveState.saveMetaInformationJSON = JsonBeautifier.FormatJson(selectedFileSaveState.saveMetaInformationJSON);
            // // selectedFileSaveState.gameJSON = JsonBeautifier.FormatJson(selectedFileSaveState.gameJSON);
            // selectedFileSaveState.storyJSON = JsonBeautifier.FormatJson(selectedFileSaveState.storyJSON);
            SaveEditorWindowSettings.Instance.showingSelectedSave = true;
        } else {
            selectedFileJSON = null;
            selectedFileSaveState = null;
        }
    }

    // This is dangerous because saving needs to happen at a particular time. I've changed it to copy the last auto save.
	public static string WriteManualSaveFile (string jsonStoryState, string fileName) {
		string fullPathName = Path.Combine(SaveEditorWindowSettings.Instance.manualSaveDirectory, fileName+"."+SaveLoadManager.fileExtension);
		using (StreamWriter outfile = new StreamWriter(fullPathName)) {
			outfile.Write(jsonStoryState);
		}
		return fullPathName;
	}

    public static string GetFileInfo (string filePath) {
        SaveState saveState = JsonUtility.FromJson<SaveState>(File.ReadAllText(filePath));
        return saveState.saveDescription;
    }

    public static bool canSave {
        get {
            return SaveLoadManager.canSave;
        }
    }
}