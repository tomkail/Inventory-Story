using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ink.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoSingleton<GameController> {
    public TextAsset storyJson;
    public Story story => StoryController.Instance.story;

    public SceneController sceneController;
    // public Button confirmButton;

    protected override void Awake() {
        base.Awake();
        SaveLoadManager.OnLoadGameSaveState += OnLoadGameSaveState;
        SaveLoadManager.RequestGameSaveJSON += RequestGameSaveJSON;
    }

    string RequestGameSaveJSON() {
        SaveState saveState = new SaveState();
    
        System.Text.StringBuilder saveDescriptionSB = new System.Text.StringBuilder();
        // saveDescriptionSB.AppendLine("Game state is "+GameController.Instance.gameModel.levelName+" turn "+GameController.Instance.gameModel.currentTurn+". ");
        saveState.saveDescription = saveDescriptionSB.ToString();

        saveState.gameMetaInfo = new GameMetaInformation();
        saveState.gameMetaInfo.gameName = Application.productName;
    
        saveState.saveMetaInfo = new SaveMetaInformation();
        saveState.saveMetaInfo.saveVersion = new SaveVersion(SaveVersion.buildSaveVersion);
        saveState.saveMetaInfo.saveDateTime = System.DateTime.Now;

        saveState.storySaveJson = story.state.ToJson();

        foreach (var level in sceneController.levels) {
            var levelItemStates = new List<LevelItemState>();
            foreach (var item in level.itemViews) {
                levelItemStates.Add(new LevelItemState() {
                    inkListItem = item.inkListItem,
                    position = item.layout.position,
                });
            }
            saveState.levelStates.Add(new LevelState() {
                itemStates = levelItemStates
            });
        }
    
        return JsonUtility.ToJson(saveState);
    }

    void OnLoadGameSaveState(string saveStateJson) {
        Clear();
        var saveState = JsonUtility.FromJson<SaveState>(saveStateJson);
        BeginSavedGame(saveState);
    }

    void Start () {
        Clear();
        var saveStateJson = SaveLoadManager.ReadFromSaveFile();
        var saveState = JsonUtility.FromJson<SaveState>(saveStateJson);
        BeginSavedGame(saveState);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Backspace)) Restart();
        if (Input.GetKeyDown(KeyCode.P)) Backstep();
    }


    void BeginSavedGame(SaveState saveState) {
        try {
            StoryController.Instance.InitStory(storyJson, saveState.storySaveJson);
            StoryController.Instance.OnParsedInstructions += OnParsedStoryInstructions;
            
            StoryController.Instance.Begin();
        } catch (Exception e) {
            Debug.LogError("Error loading saved game: " + e);
            BeginNewGame();
        }
    }
    
    void BeginNewGame() {
        try {
            StoryController.Instance.InitStory(storyJson);
            StoryController.Instance.OnParsedInstructions += OnParsedStoryInstructions;
            
            StoryController.Instance.Begin();
        } catch (Exception e) {
            Debug.LogError("Error starting new game: " + e);
        }
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
        sceneController.Clear();
        StoryController.Instance.OnParsedInstructions -= OnParsedStoryInstructions;
        StoryController.Instance.EndStory();
    }

    void OnParsedStoryInstructions() {
        foreach (var content in StoryController.Instance.contents) sceneController.PerformContent(content);
    }

    public bool CanInteractWithItem(InkListItem inkListItem) {
        return story.currentChoices.FirstOrDefault(x => x.text.Contains($"{inkListItem.itemName}", StringComparison.OrdinalIgnoreCase)) != null;
    }

    public bool InteractWithItem(InkListItem inkListItem) {
        foreach (var choice in StoryController.Instance.choices.OfType<ItemInteractChoiceInstruction>()) {
            if (choice.MatchesItem(inkListItem)) {
                StoryController.Instance.MakeChoice(choice.storyChoice.index);
                return true;
            }
        }
        return false;
    }
    
    public bool CombineItems(InkListItem inkListItemA, InkListItem inkListItemB) {
        foreach (var choice in StoryController.Instance.choices.OfType<CombineItemsChoiceInstruction>()) {
            if (choice.MatchesItems(inkListItemA, inkListItemB)) {
                StoryController.Instance.MakeChoice(choice.storyChoice.index);
                return true;
            }
        }
        return false;
    }

    public string GetItemName(InkListItem inkListItem) {
        var str = story.RunInkFunction<string>("getItemName", InkListItemToInkList(inkListItem));
        if (str.ToLower() != str) {
            // No item name exists for this item
            // Debug.LogWarning("getItemName didn't return improved name for " + inkListItem.itemName);
            StringBuilder sb = new StringBuilder();
            sb.Append(char.ToUpper(str[0])); // Ensure the first character is uppercase.
            for (int i = 1; i < str.Length; i++) {
                if (char.IsUpper(str[i])) {
                    sb.Append(' ');
                    sb.Append(char.ToLower(str[i]));
                } else {
                    sb.Append(str[i]);
                }
            }
            return sb.ToString();
        }
        return InkStylingUtility.ProcessText(str);
    }
    public string GetItemTooltip(InkListItem inkListItem) {
        var str = story.RunInkFunction<string>("getItemTooltip", InkListItemToInkList(inkListItem));
        return InkStylingUtility.ProcessText(str);
    }
    
    public InkList InkListItemToInkList(InkListItem inkListItem) {
        return InkList.FromString(inkListItem.fullName, story);
    }
}
