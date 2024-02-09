using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SLayout))]
public class LevelController : MonoBehaviour {
    public Story story => GameController.Instance.story;
    public GraphicRaycaster graphicRaycaster => GetComponent<GraphicRaycaster>();
    public SLayout layout => GetComponent<SLayout>();
    public RectTransform itemContainer => transform.Find("Item Container") as RectTransform;
    public SLayout overlay => transform.Find("Overlay").GetComponent<SLayout>();
    
    public List<ItemView> itemViews = new();
    public ItemLabelView draggingItemLabel => itemViews.Select(x => x.labelView).FirstOrDefault(x => x.draggable.dragging);
    
    public LevelRequiredItemsSlotGroup slotGroup;
    
    public ItemSpawnLocationManager itemSpawnLocationManager => GetComponentInChildren<ItemSpawnLocationManager>();
    
    public InkListChangeHandler levelItemsObserver;
    
    public Action OnInit;
    public Action OnSetAsCurrentLevel;
    public Action OnUnsetAsCurrentLevel;
    public Action OnSetVisibleLevel;
    public Action OnUnsetVisibleLevel;
    
    public LevelState levelState;

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
        
        graphicRaycaster.enabled = false;
        
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
        
        graphicRaycaster.enabled = true;
        
        OnSetAsCurrentLevel?.Invoke();
    }

    public void UnsetAsCurrentLevel() {
        levelItemsObserver.RemoveVariableObserver(story);
        story.RemoveVariableObserver(OnChangeSolutionItemCount);
        
        graphicRaycaster.enabled = false;
        
        OnUnsetAsCurrentLevel?.Invoke();
    }
    
    public void SetAsVisibleLevel() {
        OnSetVisibleLevel?.Invoke();
    }
    
    public void UnsetAsVisibleLevel() {
        OnUnsetVisibleLevel?.Invoke();
    }

    void Update() {
        overlay.groupAlpha = Mathf.Lerp(0.0f, 0.6f, Mathf.InverseLerp(0, layout.rectTransform.rect.size.y, Mathf.Abs(GameController.Instance.sceneController.swipeView.GetPageVectorToViewportPivot(layout.rectTransform).y)));
    }

    public ItemSpawnLocation GetSpawnPointLocationForItem(InkListItem item) {
        var spawnLocation = itemSpawnLocationManager.FindForItem(item);
        if (spawnLocation == null) {
            Debug.LogWarning($"No spawn point in level {levelState.sceneId} for item {item.fullName}. Creating temporary.");
            spawnLocation = itemSpawnLocationManager.CreateSpawnLocation(item.fullName);    
            //
            // spawnLocation.rectTransform.anchoredPosition = RectTransformX.GetClampedAnchoredPositionInsideScreenRect(spawnLocation.rectTransform, spawnLocation.rectTransform.anchoredPosition, spawnLocationsRectTransform.GetScreenRect(), layout.rootCanvas.worldCamera);
        }

        return spawnLocation;
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
        var itemSpawnLocation = GetSpawnPointLocationForItem(inkListItem);
        item.Init(inkListItem, itemSpawnLocation);
    }

    void DestroyItemView(InkListItem inkListItem) {
        itemViews.Where(view => view.inkListItem.Equals(inkListItem)).ToList().ForEach(view => {
            // slotGroup.draggableGroup.draggables.Remove(view.draggable);
            itemViews.Remove(view);
            Destroy(view.gameObject);
        });
    }
    
    public void OnSlotItem(ItemLabelView itemLabel) {
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
                    var itemView = itemViews.Select(x => x.labelView).FirstOrDefault(x => x.inkListItem.Equals(itemList.Key));
                    if (itemView == null) continue;
                    itemView.ExitSlot();

                    // var newPosition = itemView.layout.rectTransform.anchoredPosition + MathX.DegreesToVector2(Random.Range(-45, 45)) * 300;
                    itemView.draggable.SetDragTargetPosition(itemView.draggable.positionAtLastDragStart, false);
                    // var slot = slotGroup.slots.FirstOrDefault(x => x.heldSlottable == (ISlottable)itemView);
                    
                    // slotGroup.draggableGroup.Unslot(slot);
                }
            }
        }
    }

    public void LoadBackground(BackgroundInstruction backgroundInstruction) {
        Debug.Log(backgroundInstruction.assetPath);
        // background.sprite = Resources.Load<Sprite>(backgroundInstruction.assetPath);
    }
    
    // void CreateItemContainer() {
    //     itemContainer = new GameObject("Items").AddComponent<RectTransform>();
    //     itemContainer.gameObject.AddComponent<SLayout>();
    //     itemContainer.SetParent(transform, false);
    //     itemContainer.SetAsLastSibling();
    //     itemContainer.anchorMin = new Vector2(0, 0);
    //     itemContainer.anchorMax = new Vector2(1,1);
    //     itemContainer.sizeDelta = new Vector2(0,0);
    // }
    
    public void Clear() {
        foreach(var itemView in itemViews) Destroy(itemView.gameObject);
        itemViews.Clear();
        slotGroup.Clear();
    }
}
