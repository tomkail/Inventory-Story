using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoSingleton<GameController> {
    public TextAsset storyJson;
    public Story story => StoryController.Instance.story;
    
    public InkListChangeHandler currentLevelItems = new InkListChangeHandler("currentItems");
    public InkListChangeHandler currentAnswerSet = new InkListChangeHandler("currentAnswerSet");
    
    public RectTransform itemContainer;
    public Image background;
    public TextMeshProUGUI titleText;
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
        
        slotGroup.draggableGroup.OnSlottedDraggable += (draggable, slot) => {
            // TryCompleteLevel();
            var choice = story.currentChoices.FirstOrDefault(x => x.text.Contains(draggable.GetComponent<ItemView>().inkListItem.itemName));
            if (choice != null) {
                StoryController.Instance.MakeChoice(choice.index);
            }
        };
        slotGroup.draggableGroup.OnUnslottedDraggable += (draggable, slot) => {
            var choice = story.currentChoices.FirstOrDefault(x => x.text.Contains(draggable.GetComponent<ItemView>().inkListItem.itemName));
            if (choice != null) {
                StoryController.Instance.MakeChoice(choice.index);
            }
        };
        
        currentLevelItems.AddVariableObserver(story);
        currentLevelItems.OnChange += OnChangeCurrentLevelItems;
        currentLevelItems.RefreshValue(story, false);
        
        currentAnswerSet.AddVariableObserver(story);
        currentAnswerSet.OnChange += OnChangeCurrentLevelAnswerSet;
        currentAnswerSet.RefreshValue(story, false);
        
        StoryController.Instance.Begin();
    }

    void OnParsedStoryInstructions() {
        foreach (var content in StoryController.Instance.contents) PerformContent(content);
    }

    void PerformContent(ScriptContent content) {
        if (content is BackgroundInstruction parsedInstruction) LoadBackground(parsedInstruction);
        if (content is TitleInstruction titleInstruction) SetTitle(titleInstruction);
    }

    void LoadBackground(BackgroundInstruction backgroundInstruction) {
        Debug.Log(backgroundInstruction.assetPath);
        background.sprite = Resources.Load<Sprite>(backgroundInstruction.assetPath);
    }

    void SetTitle(TitleInstruction titleInstruction) {
        Debug.Log(titleInstruction.text);
        titleText.text = titleInstruction.text;
    }


    void OnChangeCurrentLevelItems(IReadOnlyList<InkListItem> currentlistitems, IReadOnlyList<InkListItem> itemsadded, IReadOnlyList<InkListItem> itemsremoved) {
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
        item.layout.position = new Vector2(Random.Range(0, item.layout.parentRect.width), Random.Range(0, item.layout.parentRect.height));
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

    public void InteractWithItem(InkListItem inkListItem) {
        var choice = story.currentChoices.FirstOrDefault(x => x.text.Contains(inkListItem.itemName));
        StoryController.Instance.MakeChoice(choice.index);
        // story.RunInkFunction<List<InkListItem>>("interact", InkListItemToInkList(inkListItem));
    }
    
    public void CombineItems(InkListItem inkListItemA, InkListItem inkListItemB) {
        story.RunInkFunction<List<InkListItem>>("interact", InkListItemToInkList(inkListItemA), InkListItemToInkList(inkListItemB));
    }

    public string GetItemTooltip(InkListItem inkListItem) {
        return story.RunInkFunction<string>("describe", InkListItemToInkList(inkListItem));
    }
    
    public InkList InkListItemToInkList(InkListItem inkListItem) {
        return InkList.FromString(inkListItem.fullName, story);
    }
}
