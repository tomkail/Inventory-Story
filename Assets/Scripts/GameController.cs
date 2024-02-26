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

    public LevelsManager levelsManager;
    public ProjectorScreenPostProcessor projectorPostProcessor;
    // public Button confirmButton;

    public GameSettings gameSettings;

    protected override void Awake() {
        base.Awake();
        SaveLoadManager.OnLoadGameSaveState += OnLoadGameSaveState;
        SaveLoadManager.RequestGameSave += RequestGameSave;
    }

    SaveState RequestGameSave() {
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

        foreach (var level in levelsManager.levels) {
            level.UpdateLevelState();
            saveState.levelStates.Add(level.levelState);
        }

        return saveState;
    }

    void OnLoadGameSaveState(SaveState saveState) {
        Clear();
        BeginSavedGame(saveState);
    }

    void Start () {
        Clear();
        var saveStateJson = SaveLoadManager.ReadFromSaveFile();
        var saveState = JsonUtility.FromJson<SaveState>(saveStateJson);
        if (saveState != null) {
            BeginSavedGame(saveState);
        } else {
            BeginNewGame();
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Backspace)) Restart();
        if (Input.GetKeyDown(KeyCode.P)) Backstep();
    }


    void BeginSavedGame(SaveState saveState) {
        DebugX.Log("<color=#337DFF>Begin Saved Game</color>");
        try {
            levelsManager.LoadSavedLevels(saveState.levelStates);
            StoryController.Instance.InitStory(storyJson, saveState.storySaveJson);
            StoryController.Instance.OnParsedInstructions += OnParsedStoryInstructions;
            
            StoryController.Instance.Begin();
        } catch (Exception e) {
            Debug.LogError("Error loading saved game: " + e);
            StoryController.Instance.EndStory();
            BeginNewGame();
        }
    }
    
    void BeginNewGame() {
        DebugX.Log("<color=#337DFF>Begin New Game</color>");
        try {
            StoryController.Instance.InitStory(storyJson);
            StoryController.Instance.OnParsedInstructions += OnParsedStoryInstructions;
            
            StoryController.Instance.Begin();
        } catch (Exception e) {
            Debug.LogError("Error starting new game: " + e);
        }
    }

    void Restart() {
        DebugX.Log("<color=#337DFF>Restart</color>");
        Clear();
        BeginNewGame();
    }

    void Backstep()
    {
        var choice = story.currentChoices.FirstOrDefault(x => x.text.Contains($"BACK", StringComparison.OrdinalIgnoreCase));
        if (choice != null) StoryController.Instance.MakeChoice(choice.index);
    }

    void Clear() {
        levelsManager.Clear();
        StoryController.Instance.OnParsedInstructions -= OnParsedStoryInstructions;
        StoryController.Instance.EndStory();
    }

    void OnParsedStoryInstructions() {
        foreach (var content in StoryController.Instance.contents) levelsManager.PerformContent(content);
    }

    // public bool CanInteractWithItem(ItemModel itemModel) {
    //     return story.currentChoices.FirstOrDefault(x => x.text.Contains($"{itemModel.inkListItemName}", StringComparison.OrdinalIgnoreCase)) != null;
    // }

    // public bool InteractWithItem(ItemModel itemModel) {
    //     foreach (var choice in StoryController.Instance.choices.OfType<ItemInteractChoiceInstruction>()) {
    //         if (choice.MatchesItem(itemModel.inkListItem)) {
    //             StoryController.Instance.MakeChoice(choice.storyChoice.index);
    //             return true;
    //         }
    //     }
    //     return false;
    // }
    
    public bool CombineItems(InkListItem inkListItemA, InkListItem inkListItemB) {
        foreach (var choice in StoryController.Instance.choices.OfType<CombineItemsChoiceInstruction>()) {
            if (choice.MatchesItems(inkListItemA, inkListItemB)) {
                StoryController.Instance.MakeChoice(choice.storyChoice.index);
                return true;
            }
        }
        return false;
    }
}
