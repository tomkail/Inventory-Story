using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SLayout))]
public class LevelController : MonoBehaviour {
    public Story story => GameController.Instance.story;
    public SLayout layout => GetComponent<SLayout>();
    public RectTransform itemContainer { get; private set; }
    public List<ItemView> itemViews = new List<ItemView>();
    public ItemView draggingItem => itemViews.FirstOrDefault(x => x.draggable.dragging);
    public LevelRequiredItemsSlotGroup slotGroup;
    public ItemSpawnLocation[] itemSpawnLocations => GetComponentsInChildren<ItemSpawnLocation>();


    public InkListChangeHandler levelItemsObserver;
    
    public Action OnInit;
    public Action OnSetAsCurrentLevel;
    public Action OnUnsetAsCurrentLevel;
    public Action OnSetVisibleLevel;
    public Action OnUnsetVisibleLevel;
    
    public LevelState levelState;

    void Awake() {
        CreateItemContainer();
    }

    public void Init(SceneInstruction sceneInstruction) {
        levelState = new LevelState() {
            titleText = sceneInstruction.title,
            dateText = sceneInstruction.date
        };
        OnInit?.Invoke();
    }

    public void Init(LevelState levelState) {
        this.levelState = levelState;
        OnInit?.Invoke();
    }
    
    public void Init(LevelLoadParams levelLoadParams) {
        levelItemsObserver = new InkListChangeHandler("levelItems");
        levelItemsObserver.OnChange += OnChangeLevelItems;
        
        this.levelState = new LevelState() {
            sceneId = levelLoadParams.sceneId.fullName,
            titleText = levelLoadParams.titleText,
            dateText = levelLoadParams.dateText
        };
        OnInit?.Invoke();
    }

    public void SetAsCurrentLevel() {
        levelItemsObserver.AddVariableObserver(story);
        story.ObserveVariable("levelSolutionItemCount", OnChangeSolutionItemCount);
        
        slotGroup.Init((int)story.variablesState["levelSolutionItemCount"]);
        levelItemsObserver.RefreshValue(story, false);
        
        OnSetAsCurrentLevel?.Invoke();
    }

    public void UnsetAsCurrentLevel() {
        levelItemsObserver.RemoveVariableObserver(story);
        story.RemoveVariableObserver(OnChangeSolutionItemCount);
        
        OnUnsetAsCurrentLevel?.Invoke();
    }
    
    public void SetAsVisibleLevel() {
        OnSetVisibleLevel?.Invoke();
    }
    
    public void UnsetAsVisibleLevel() {
        OnUnsetVisibleLevel?.Invoke();
    }

    public Vector3 GetWorldSpawnLocationForItem(InkListItem item) {
        var spawnLocation = itemSpawnLocations.FirstOrDefault(x => string.Equals(x.gameObject.name, item.fullName, StringComparison.OrdinalIgnoreCase));
        if(spawnLocation == null) spawnLocation = itemSpawnLocations.FirstOrDefault(x => string.Equals(x.gameObject.name, item.itemName, StringComparison.OrdinalIgnoreCase));
        if(spawnLocation != null) return spawnLocation.transform.position;
        var randomLocalPos = new Vector2(Random.Range(itemContainer.rect.xMin, itemContainer.rect.xMax), Random.Range(itemContainer.rect.yMin, itemContainer.rect.yMax));
        return itemContainer.TransformPoint(randomLocalPos);
    }

    public void OnChangeLevelItems(IReadOnlyList<InkListItem> currentlistitems, IReadOnlyList<InkListItem> itemsadded, IReadOnlyList<InkListItem> itemsremoved) {
        foreach(var item in itemsremoved) {
            Debug.Log("Removed item: " + item);
            DestroyItemView(item);
        }
        foreach(var item in itemsadded) {
            Debug.Log("Added item: " + item);
            CreateItemView(item);
        }
    }

    void OnChangeSolutionItemCount(string variablename, object newValue) {
        OnChangeCurrentLevelAnswerSet((int)StoryController.TryCoerce<int>(newValue));
    }

    public void OnChangeCurrentLevelAnswerSet(int newSlotCount) {// IReadOnlyList<InkListItem> currentlistitems, IReadOnlyList<InkListItem> itemsadded, IReadOnlyList<InkListItem> itemsremoved) {
        slotGroup.Init(newSlotCount);
    }

    void CreateItemView(InkListItem inkListItem) {
        var item = Instantiate(PrefabDatabase.Instance.itemViewPrefab, itemContainer);
        itemViews.Add(item);
        item.Init(inkListItem);
        var itemWorldPosition = GetWorldSpawnLocationForItem(inkListItem);
        item.targetLocalPoint = item.layout.parentRectTransform.InverseTransformPoint(itemWorldPosition);
        var localSpawnPoint = item.targetLocalPoint + RandomX.onUnitCircle * Random.Range(100, 200);
        localSpawnPoint = item.GetClampedAnchoredPositionInsideParent(localSpawnPoint);
        item.SetWorldPosition(item.layout.parentRectTransform.TransformPoint(localSpawnPoint));
        
        slotGroup.draggableGroup.draggables.Add(item.draggable);
    }

    void DestroyItemView(InkListItem inkListItem) {
        itemViews.Where(view => view.inkListItem.Equals(inkListItem)).ToList().ForEach(view => {
            slotGroup.draggableGroup.draggables.Remove(view.draggable);
            itemViews.Remove(view);
            Destroy(view.gameObject);
        });
    }
    
    public void OnCompleteItemDrag(ItemView item) {
        /*
        if (IEnumerableX.GetChanges(currentAnswerSet.currentListItems, slotGroup.slottedItems.Select(x => x.inkListItem), out var missingItems, out var wrongItems)) {
            foreach (var wrongItemList in wrongItems) {
                var wrongItem = itemViews.FirstOrDefault(x => x.inkListItem.Equals(wrongItemList));
                if(wrongItem == null) continue;
                wrongItem.draggable.SetDragTargetPosition(wrongItem.layout.rectTransform.anchoredPosition + Vector2.up * 300, false);
                var slot = slotGroup.draggableGroup.slots.FirstOrDefault(x => x.slottedDraggable == wrongItem.draggable);
                slotGroup.draggableGroup.Unslot(slot);
            }
        } else {
        */

        // Set the ink state for "slotted items"
        var inkList = new InkList();
        var slottedItemLists = slotGroup.slottedItems.Select(itemView => InkList.FromString(itemView.inkListItem.fullName, StoryController.Instance.story)).ToList();
        foreach (var slottedItem in slottedItemLists)
            inkList = inkList.Union(slottedItem);
        story.variablesState["currentItems"] = inkList;

        // have we got all slots filled?
        if (inkList.Count == slotGroup.slots.Count) {

            // Check - have we solved the level?
            if (story.RunInkFunction<bool>("checkForSolution")) {
                var choice = story.currentChoices.FirstOrDefault(x => x.text.Contains("SOLVED"));
                if(choice != null)
                    StoryController.Instance.MakeChoice(choice.index);
            } else {
                foreach (var itemList in (InkList)story.variablesState["currentItems"]) {
                    var itemView = itemViews.FirstOrDefault(x => x.inkListItem.Equals(itemList.Key));
                    if (itemView == null) continue;
                    itemView.draggable.SetDragTargetPosition(itemView.layout.rectTransform.anchoredPosition + MathX.DegreesToVector2(Random.Range(-45, 45)) * 300, false);
                    var slot = slotGroup.draggableGroup.slots.FirstOrDefault(x => x.slottedDraggable == itemView.draggable);
                    slotGroup.draggableGroup.Unslot(slot);
                }
            }
        }
    }

    public void LoadBackground(BackgroundInstruction backgroundInstruction) {
        Debug.Log(backgroundInstruction.assetPath);
        // background.sprite = Resources.Load<Sprite>(backgroundInstruction.assetPath);
    }
    
    void CreateItemContainer() {
        itemContainer = new GameObject("Items").AddComponent<RectTransform>();
        itemContainer.gameObject.AddComponent<SLayout>();
        itemContainer.SetParent(transform, false);
        itemContainer.SetAsLastSibling();
        itemContainer.anchorMin = new Vector2(0, 0);
        itemContainer.anchorMax = new Vector2(1,1);
        itemContainer.sizeDelta = new Vector2(0,0);
    }
    
    public void Clear() {
        foreach(var itemView in itemViews) Destroy(itemView.gameObject);
        itemViews.Clear();
        slotGroup.Clear();
    }
}
