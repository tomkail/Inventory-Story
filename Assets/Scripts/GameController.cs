using System;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoSingleton<GameController> {
    public TextAsset storyJson;
    public Story story => StoryController.Instance.story;

    public InkListChangeHandler levelItems;
    // InkListChangeHandler currentItems;
    // InkListChangeHandler currentAnswerSet;

    public SceneController sceneController;
    // public Button confirmButton;

    protected override void Awake() {
        base.Awake();
        SaveLoadManager.OnLoadGameSaveState += OnLoadGameSaveState;
        SaveLoadManager.RequestGameSaveJSON += RequestGameSaveJSON;
    }

    string RequestGameSaveJSON() {
        return story.state.ToJson();
    }

    void OnLoadGameSaveState(string saveStateJson) {
        Clear();
        BeginGame(saveStateJson);
    }

    void Start () {
        Clear();
        BeginGame(SaveLoadManager.ReadFromSaveFile());
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Backspace)) Restart();
        if (Input.GetKeyDown(KeyCode.P)) Backstep();
    }


    void BeginGame(string saveStateJson = null) {
        StoryController.Instance.InitStory(storyJson, saveStateJson);
        StoryController.Instance.OnParsedInstructions += OnParsedStoryInstructions;
        // confirmButton.onClick.AddListener(() => {
        //     
        // });
        
        // slotGroup.draggableGroup.OnSlottedDraggable += (draggable, slot) => {
        //     // TryCompleteLevel();
        //     var choice = story.currentChoices.FirstOrDefault(x => x.text.Contains(draggable.GetComponent<ItemView>().inkListItem.itemName));
        //     if (choice != null) {
        //         StoryController.Instance.MakeChoice(choice.index);
        //     }
        // };
        // slotGroup.draggableGroup.OnUnslottedDraggable += (draggable, slot) => {
        //     var choice = story.currentChoices.FirstOrDefault(x => x.text.Contains(draggable.GetComponent<ItemView>().inkListItem.itemName));
        //     if (choice != null) {
        //         StoryController.Instance.MakeChoice(choice.index);
        //     }
        // };
        
        
        levelItems = new InkListChangeHandler("levelItems");
        levelItems.AddVariableObserver(story);
        levelItems.OnChange += sceneController.OnChangeLevelItems;
        levelItems.RefreshValue(story, false);
        
        // currentItems = new InkListChangeHandler("currentItems");
        // currentItems.AddVariableObserver(story);
        // currentItems.OnChange += OnChangeCurrentItems;
        // currentItems.RefreshValue(story, false);

        story.ObserveVariable("levelSolutionItemCount", (varName, newValue) => {
            sceneController.OnChangeCurrentLevelAnswerSet((int) newValue);
        });

        /*
        currentAnswerSet = new InkListChangeHandler("levelSolutionItems");
        currentAnswerSet.AddVariableObserver(story);
        currentAnswerSet.OnChange += OnChangeCurrentLevelAnswerSet;
        currentAnswerSet.RefreshValue(story, false);
        */
        
        StoryController.Instance.Begin();
        sceneController.slotGroup.Init((int)story.variablesState["levelSolutionItemCount"]);
    }
    
    void Restart() {
        Clear();
        Start();
    }

    void Backstep()
    {
        var choice = story.currentChoices.FirstOrDefault(x => x.text.Contains($"BACK", StringComparison.OrdinalIgnoreCase));
        if (choice != null) StoryController.Instance.MakeChoice(choice.index);
    }

    void Clear() {
        StoryController.Instance.OnParsedInstructions -= OnParsedStoryInstructions;
        StoryController.Instance.EndStory();
        sceneController.Clear();
    }

    void OnParsedStoryInstructions() {
        foreach (var content in StoryController.Instance.contents) sceneController.PerformContent(content);
    }

    public bool CanInteractWithItem(InkListItem inkListItem) {
        return story.currentChoices.FirstOrDefault(x => x.text.Contains($"{inkListItem.itemName}", StringComparison.OrdinalIgnoreCase)) != null;
    }

    public void InteractWithItem(InkListItem inkListItem) {
        var choice = story.currentChoices.FirstOrDefault(x => x.text.Contains($"{inkListItem.itemName}", StringComparison.OrdinalIgnoreCase));
        if(choice != null) StoryController.Instance.MakeChoice(choice.index);
    }
    
    public void CombineItems(InkListItem inkListItemA, InkListItem inkListItemB) {
        story.RunInkFunction<List<InkListItem>>("interact", InkListItemToInkList(inkListItemA), InkListItemToInkList(inkListItemB));
    }

    public string GetItemName(InkListItem inkListItem) {
        return story.RunInkFunction<string>("getItemName", InkListItemToInkList(inkListItem));
    }
    public string GetItemTooltip(InkListItem inkListItem) {
        return story.RunInkFunction<string>("getItemTooltip", InkListItemToInkList(inkListItem));
    }
    
    public InkList InkListItemToInkList(InkListItem inkListItem) {
        return InkList.FromString(inkListItem.fullName, story);
    }
}
