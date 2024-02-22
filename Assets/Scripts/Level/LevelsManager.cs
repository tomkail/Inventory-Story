using System;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

public class LevelsManager : MonoBehaviour {
    [SerializeField] SLayout levelsContainer;
    public List<Level> levels = new List<Level>();
    [Disable] public Level currentLevel;
    public Level visibleLevel => swipeView.targetPage == null ? null : swipeView.targetPage.GetComponent<Level>();
    public SwipeView swipeView;
    public NarrationUIView narrationView;

    void Awake() {
        swipeView.OnChangeTargetPage += OnChangeTargetPage;
    }

    void OnChangeTargetPage(RectTransform previouspage, RectTransform newpage) {
        var previousLevelController = previouspage != null ? previouspage.GetComponent<Level>() : null;
        if (previousLevelController != null) {
            previousLevelController.UnsetAsVisibleLevel();
        }
        var newLevelController = newpage != null ? newpage.GetComponent<Level>() : null;
        if (newLevelController != null) {
            newLevelController.SetAsVisibleLevel();
        }
    }

    public void Clear() {
        SetCurrentLevel(null);
        swipeView.targetPage = null;
        for (var index = levels.Count - 1; index >= 0; index--) {
            var level = levels[index];
            swipeView.pages.Remove(level.layout.rectTransform);
            level.Clear();
            Destroy(level.gameObject);
            levels.RemoveAt(index);
        }
    }

    void Update() {
        swipeView.interactable = !currentLevel.scanModeFlags.HasFlag(ScanModeStateFlags.Active);
    }
    
    public void PerformContent(ScriptContent content) {
        if (content is BackgroundInstruction parsedInstruction) currentLevel.LoadBackground(parsedInstruction);
        // else if (content is TitleInstruction titleInstruction) SetTitle(titleInstruction);
        // else if (content is SceneInstruction sceneInstruction) SetScene(sceneInstruction);
        else if (content is DialogueInstruction dialogueInstruction) HandleDialogue(dialogueInstruction);
    }
    
    void HandleDialogue(DialogueInstruction dialogueInstruction) {
        narrationView.ShowText(dialogueInstruction.text);
        VOController.Instance.StreamAndCache(dialogueInstruction.text);
    }
    /*
    void SetTitle(TitleInstruction titleInstruction) {
        Debug.Log(titleInstruction.text);
        titleText.text = titleInstruction.text;
    }
    */

    // void SetScene(SceneInstruction sceneInstruction) {
    //     SaveLoadManager.Save();
    //
    //     var newLevelController = Instantiate(PrefabDatabase.Instance.levelPrefab, levelsContainer.transform);
    //     newLevelController.Init(sceneInstruction);
    //     
    //     levels.Add(newLevelController);
    //     swipeView.pages.Add(newLevelController.layout.rectTransform);
    //     
    //     LayoutLevels();
    //     SetCurrentLevel(newLevelController);
    //     SetVisibleLevel(newLevelController);
    // }

    public void LoadSavedLevels(List<LevelState> savedLevelStates) {
        foreach (var savedLevelState in savedLevelStates) {
            var newLevelController = Instantiate(PrefabDatabase.Instance.fallbackLevelPrefab, levelsContainer.transform);
            newLevelController.Init(savedLevelState);
            
            levels.Add(newLevelController);
            swipeView.pages.Add(newLevelController.layout.rectTransform);
            
            LayoutLevels();
            // SetCurrentLevel(newLevelController);
            // SetVisibleLevel(newLevelController);
        }
    }
    
    public void LoadAndStartNewLevel(LevelLoadParams levelLoadParams) {
        var newLevelController = Instantiate(PrefabDatabase.Instance.TryFindLevel(levelLoadParams.levelId), levelsContainer.transform);
        newLevelController.Init(levelLoadParams);
        
        levels.Add(newLevelController);
        swipeView.pages.Add(newLevelController.layout.rectTransform);
        
        LayoutLevels();
        SetCurrentLevel(newLevelController);
        SetVisibleLevel(newLevelController);
    }

    void SetCurrentLevel(Level newCurrentLevel) {
        if (currentLevel == newCurrentLevel) return;
        if (currentLevel != null) {
            currentLevel.UnsetAsCurrentLevel();
        }
        currentLevel = newCurrentLevel;
        if (currentLevel != null) {
            currentLevel.SetAsCurrentLevel();
        }
    }

    void SetVisibleLevel(Level level) {
        if(levels.Count == 1) swipeView.GoToPageImmediate(level.layout.rectTransform);
        else swipeView.GoToPageSmooth(level.layout.rectTransform);
    }

    void LayoutLevels() {
        var layoutItems = new List<LayoutItem>();
        foreach (var level in levels) {
            layoutItems.Add(new LayoutItem(LayoutItemParams.Fixed(level.layout.height), level.layout));
        }
        levelsContainer.height = SLayoutUtils.AutoLayoutWithDynamicSizing(levelsContainer, layoutItems, SLayoutUtils.Axis.Y, 40, 0, 0, 0);
    }
}