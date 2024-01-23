using System;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoSingleton<GameController> {
    public TextAsset storyJson;
    public Story story => StoryController.Instance.story;
    
    InkListChangeHandler levelItems;
    InkListChangeHandler currentItems;
    InkListChangeHandler currentAnswerSet;
    
    public RectTransform itemContainer;
    public Image background;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI voText;
    public ItemView itemViewPrefab;
    public List<ItemView> itemViews = new List<ItemView>();
    public LevelRequiredItemsSlotGroup slotGroup;
    // public Button confirmButton;
    
    void Start () {
        StoryController.Instance.InitStory(storyJson);
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
        levelItems.OnChange += OnChangeLevelItems;
        levelItems.RefreshValue(story, false);
        
        // currentItems = new InkListChangeHandler("currentItems");
        // currentItems.AddVariableObserver(story);
        // currentItems.OnChange += OnChangeCurrentItems;
        // currentItems.RefreshValue(story, false);
        
        currentAnswerSet = new InkListChangeHandler("levelSolutionItems");
        currentAnswerSet.AddVariableObserver(story);
        currentAnswerSet.OnChange += OnChangeCurrentLevelAnswerSet;
        currentAnswerSet.RefreshValue(story, false);
        
        StoryController.Instance.Begin();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Backspace)) Restart();
    }

    void Restart() {
        Clear();
        Start();
    }

    void Clear() {
        StoryController.Instance.OnParsedInstructions -= OnParsedStoryInstructions;
        StoryController.Instance.EndStory();
        foreach(var itemView in itemViews) Destroy(itemView.gameObject);
        itemViews.Clear();
        slotGroup.Clear();
    }

    void OnParsedStoryInstructions() {
        foreach (var content in StoryController.Instance.contents) PerformContent(content);
    }

    void PerformContent(ScriptContent content) {
        if (content is BackgroundInstruction parsedInstruction) LoadBackground(parsedInstruction);
        else if (content is TitleInstruction titleInstruction) SetTitle(titleInstruction);
        else if (content is SceneInstruction sceneInstruction) SetScene(sceneInstruction);
        else if (content is DialogueInstruction dialogueInstruction) HandleDialogue(dialogueInstruction);
    }

    void HandleDialogue(DialogueInstruction dialogueInstruction) {
        Debug.Log(dialogueInstruction.speaker + ": " + dialogueInstruction.text);
        voText.text = dialogueInstruction.text;
    }

    void LoadBackground(BackgroundInstruction backgroundInstruction) {
        Debug.Log(backgroundInstruction.assetPath);
        background.sprite = Resources.Load<Sprite>(backgroundInstruction.assetPath);
    }

    void SetTitle(TitleInstruction titleInstruction) {
        Debug.Log(titleInstruction.text);
        titleText.text = titleInstruction.text;
    }

    void SetScene(SceneInstruction sceneInstruction) {
        // story.variablesState[levelItems.variableName] = new InkList();
        // story.variablesState[currentItems.variableName] = new InkList();
        // story.variablesState[currentAnswerSet.variableName] = new InkList();
        // levelItems.RefreshValue(story, false);
        // currentItems.Reset();
        // currentItems.RefreshValue(story, false);
        // currentAnswerSet.Reset();
        // currentAnswerSet.RefreshValue(story, false);
    }


    void OnChangeLevelItems(IReadOnlyList<InkListItem> currentlistitems, IReadOnlyList<InkListItem> itemsadded, IReadOnlyList<InkListItem> itemsremoved) {
        foreach(var item in itemsremoved) {
            Debug.Log("Removed item: " + item);
            DestroyItemView(item);
        }
        foreach(var item in itemsadded) {
            Debug.Log("Added item: " + item);
            CreateItemView(item);
        }
    }
    
    void OnChangeCurrentLevelAnswerSet(IReadOnlyList<InkListItem> currentlistitems, IReadOnlyList<InkListItem> itemsadded, IReadOnlyList<InkListItem> itemsremoved) {
        slotGroup.Init(currentAnswerSet.inkList);
    }

    void CreateItemView(InkListItem inkListItem) {
        var item = Instantiate(itemViewPrefab, itemContainer);
        itemViews.Add(item);
        item.Init(inkListItem);
        item.layout.position = new Vector2(Random.Range(0, item.layout.parentRect.width-item.layout.width), Random.Range(0, item.layout.parentRect.height-item.layout.height));
        item.draggable.SetPositionImmediate(item.layout.rectTransform.anchoredPosition);
        slotGroup.draggableGroup.draggables.Add(item.draggable);
    }
    
    void DestroyItemView(InkListItem inkListItem) {
        itemViews.Where(view => view.inkListItem.Equals(inkListItem)).ToList().ForEach(view => {
            slotGroup.draggableGroup.draggables.Remove(view.draggable);
            itemViews.Remove(view);
            Destroy(view.gameObject);
        });
    }
    
    

    public void OnCompleteDrag(ItemView item) {
        if (IEnumerableX.GetChanges(currentAnswerSet.currentListItems, slotGroup.slottedItems.Select(x => x.inkListItem), out var missingItems, out var wrongItems)) {
            foreach (var wrongItemList in wrongItems) {
                var wrongItem = itemViews.FirstOrDefault(x => x.inkListItem.Equals(wrongItemList));
                if(wrongItem == null) continue;
                wrongItem.draggable.SetDragTargetPosition(wrongItem.layout.rectTransform.anchoredPosition + Vector2.up * 300, false);
                var slot = slotGroup.draggableGroup.slots.FirstOrDefault(x => x.slottedDraggable == wrongItem.draggable);
                slotGroup.draggableGroup.Unslot(slot);
            }
        } else {
            var choice = story.currentChoices.FirstOrDefault(x => x.text.Contains("SOLVED"));
            if(choice != null) StoryController.Instance.MakeChoice(choice.index);
        }
    }

    public bool CanInteractWithItem(InkListItem inkListItem) {
        return story.currentChoices.FirstOrDefault(x => x.text.Contains(inkListItem.itemName)) != null;
    }

    public void InteractWithItem(InkListItem inkListItem) {
        var choice = story.currentChoices.FirstOrDefault(x => x.text.Contains(inkListItem.itemName));
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
