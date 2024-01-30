using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneController : MonoBehaviour {
    [SerializeField] SLayout levelsContainer;
    public List<LevelController> levels = new List<LevelController>();
    public LevelController currentLevelController;
    public SwipeView swipeView;
    public TextMeshProUGUI voText;

    void Awake() {
        swipeView.OnChangeTargetPage += OnChangeTargetPage;
    }

    void OnChangeTargetPage(RectTransform previouspage, RectTransform newpage) {
        var previousLevelController = previouspage != null ? previouspage.GetComponent<LevelController>() : null;
        if (previousLevelController != null) {
            previousLevelController.UnsetAsVisibleLevel();
        }
        var newLevelController = newpage != null ? newpage.GetComponent<LevelController>() : null;
        if (newLevelController != null) {
            newLevelController.SetAsVisibleLevel();
        }
    }

    public void Clear() {
        if(currentLevelController != null) currentLevelController.Clear();
    }
    
    public void PerformContent(ScriptContent content) {
        if (content is BackgroundInstruction parsedInstruction) currentLevelController.LoadBackground(parsedInstruction);
        // else if (content is TitleInstruction titleInstruction) SetTitle(titleInstruction);
        else if (content is SceneInstruction sceneInstruction) SetScene(sceneInstruction);
        else if (content is DialogueInstruction dialogueInstruction) HandleDialogue(dialogueInstruction);
    }

    void HandleDialogue(DialogueInstruction dialogueInstruction) {
        Debug.Log(dialogueInstruction.speaker + ": " + dialogueInstruction.text);
        voText.text = dialogueInstruction.text;
        VOController.Instance.StreamAndCache(dialogueInstruction.text);
    }
    /*
    void SetTitle(TitleInstruction titleInstruction) {
        Debug.Log(titleInstruction.text);
        titleText.text = titleInstruction.text;
    }
    */

    void SetScene(SceneInstruction sceneInstruction) {
        SaveLoadManager.Save();

        var newLevelController = Instantiate(PrefabDatabase.Instance.levelPrefab, levelsContainer.transform);
        newLevelController.Init(sceneInstruction);
        
        levels.Add(newLevelController);
        swipeView.pages.Add(newLevelController.layout.rectTransform);
        
        LayoutLevels();
        SetCurrentLevel(newLevelController);
        SetVisibleLevel(newLevelController);
    }

    void SetCurrentLevel(LevelController newCurrentLevel) {
        if (currentLevelController == newCurrentLevel) return;
        if (currentLevelController != null) {
            currentLevelController.UnsetAsCurrentLevel();
        }
        currentLevelController = newCurrentLevel;
        if (currentLevelController != null) {
            currentLevelController.SetAsCurrentLevel();
        }
    }

    void SetVisibleLevel(LevelController levelController) {
        swipeView.GoToPageSmooth(levelController.layout.rectTransform);
    }

    void LayoutLevels() {
        var layoutItems = new List<LayoutItem>();
        foreach (var level in levels) {
            layoutItems.Add(new LayoutItem(LayoutItemParams.Fixed(level.layout.height), level.layout));
        }
        levelsContainer.height = SLayoutUtils.AutoLayoutWithDynamicSizing(levelsContainer, layoutItems, SLayoutUtils.Axis.Y, 40, 0, 0, 0);
    }
}