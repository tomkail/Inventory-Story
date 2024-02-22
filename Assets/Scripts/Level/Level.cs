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

[Flags]
public enum ScanModeStateFlags {
    None = 0,
    All = ~0,
    Permissable = 1 << 1,
    Usable = 1 << 2,
    Active = 1 << 3,
}
[RequireComponent(typeof(SLayout))]
public class Level : MonoBehaviour {
    public Story story => GameController.Instance.story;
    public GraphicRaycaster graphicRaycaster => GetComponent<GraphicRaycaster>();
    public SLayout layout => GetComponent<SLayout>();
    public LevelViewport viewport => GetComponentInChildren<LevelViewport>(true);
    public RectTransform itemContainer => transform.Find("Item Container") as RectTransform;
    public SLayout overlay => transform.Find("Overlay").GetComponent<SLayout>();
    public SLayout scannerOverlay => transform.Find("Scanner Overlay").GetComponent<SLayout>();
    public Scanner scanner => GetComponentInChildren<Scanner>(true);
    
    public List<ItemView> itemViews = new();
    public ItemDraggableGhostView draggingItemDraggableGhost => ItemDraggableGhostView.currentlyDraggingItem;
    
    public LevelRequiredItemsSlotGroup slotGroup;
    
    public ItemSpawnLocationManager itemSpawnLocationManager => GetComponentInChildren<ItemSpawnLocationManager>();
    
    public InkListChangeHandler levelItemsObserver;
    
    public Action OnInit;
    public Action OnSetAsCurrentLevel;
    public Action OnUnsetAsCurrentLevel;
    public Action OnSetVisibleLevel;
    public Action OnUnsetVisibleLevel;

    public LevelState levelState;

    [EnumFlagsButtonGroup]
    [SerializeField]ScanModeStateFlags _scanModeFlags;
    public ScanModeStateFlags scanModeFlags {
        get => _scanModeFlags;
        set {
            if (_scanModeFlags == value) return;
            _scanModeFlags = value;
            onChangeScanModeFlags?.Invoke(_scanModeFlags);
        }
    }
    public Action<ScanModeStateFlags> onChangeScanModeFlags;
    
    public bool isCurrentLevel => GameController.Instance.levelsManager.currentLevel == this;
    public bool isVisibleLevel => GameController.Instance.levelsManager.visibleLevel == this;

    public void OnClickScanModeButton() {
        var newScanModeFlags = scanModeFlags ^= ScanModeStateFlags.Active;
        scanModeFlags = ValidateScanModeFlags(newScanModeFlags);
    }

    // public void Init(SceneInstruction sceneInstruction) {
    //     levelState = new LevelState() {
    //         titleText = sceneInstruction.title,
    //         dateText = sceneInstruction.date
    //     };
    //     OnInit?.Invoke();
    // }

    // This is called for new levels regardless of how they're created (this could probably just be run on Awake)
    void Init() {
        levelItemsObserver = new InkListChangeHandler("levelItems");
        levelItemsObserver.OnChange += OnChangeLevelItems;
        
        graphicRaycaster.enabled = false;
    }
    
    // Creates a new level from level load params (probably passed from ink)
    public void Init(LevelLoadParams levelLoadParams) {
        Init();
        
        levelState = new LevelState() {
            uuid = Guid.NewGuid().ToString(),
            sceneId = levelLoadParams.levelId.fullName,
            titleText = levelLoadParams.titleText,
            dateText = levelLoadParams.dateText
        };
        OnInit?.Invoke();
    }

    // Creates a new level from level state (probably passed from save)
    public void Init(LevelState levelState) {
        Init();
        
        this.levelState = levelState;
        foreach (var itemModel in levelState.itemStates) {
            CreateItemView(itemModel);
        }
        OnInit?.Invoke();
    }

    
    // Sets this level as the current level to be solved
    public void SetAsCurrentLevel() {
        levelItemsObserver.AddVariableObserver(story);
        story.ObserveVariable("levelSolutionItemCount", OnChangeSolutionItemCount);
        
        slotGroup.Init((int)story.variablesState["levelSolutionItemCount"]);
        // This causes issues when this is called on level change, since the items in ink are current the items for last level.
        // This causes the level to believe it should have a view for those items.
        // We really want the items array to be blank at this point and then fill it when the observer triggers.
        // levelItemsObserver.RefreshValue(story, false);
        
        graphicRaycaster.enabled = true;
        
        OnSetAsCurrentLevel?.Invoke();
    }

    public void UnsetAsCurrentLevel() {
        levelItemsObserver.RemoveVariableObserver(story);
        story.RemoveVariableObserver(OnChangeSolutionItemCount);
        
        graphicRaycaster.enabled = false;
        
        OnUnsetAsCurrentLevel?.Invoke();
    }
    
    // Sets this level as the one that the player is currently looking at. Non-current levels can be viewed but can't be interacted with.
    public void SetAsVisibleLevel() {
        OnSetVisibleLevel?.Invoke();
    }
    
    public void UnsetAsVisibleLevel() {
        OnUnsetVisibleLevel?.Invoke();
    }
    
    
    

    void Update() {
        overlay.groupAlpha = Mathf.Lerp(0.0f, 0.6f, Mathf.InverseLerp(0, layout.rectTransform.rect.size.y, Mathf.Abs(GameController.Instance.levelsManager.swipeView.GetPageVectorToViewportPivot(layout.rectTransform).y)));
        
        scanModeFlags = ValidateScanModeFlags(scanModeFlags);
        if (scanModeFlags.HasFlag(ScanModeStateFlags.Usable) && Input.GetKeyDown(KeyCode.Space)) OnClickScanModeButton();
        
        scannerOverlay.gameObject.SetActive(scanModeFlags.HasFlag(ScanModeStateFlags.Active));
        scanner.gameObject.SetActive(scanModeFlags.HasFlag(ScanModeStateFlags.Active));
        
    }


    ScanModeStateFlags ValidateScanModeFlags(ScanModeStateFlags newScanModeFlags) {
        newScanModeFlags = (ScanModeStateFlags)FlagsX.SetFlag((int)newScanModeFlags, (int)ScanModeStateFlags.Permissable, true);
        newScanModeFlags = (ScanModeStateFlags)FlagsX.SetFlag((int)newScanModeFlags, (int)ScanModeStateFlags.Usable, newScanModeFlags.HasFlag(ScanModeStateFlags.Permissable) && levelState.itemStates.Any(x => x.state == ItemModel.State.Searchable));
        newScanModeFlags = (ScanModeStateFlags)FlagsX.SetFlag((int)newScanModeFlags, (int)ScanModeStateFlags.Active, newScanModeFlags.HasFlag(ScanModeStateFlags.Usable) && newScanModeFlags.HasFlag(ScanModeStateFlags.Active) && isCurrentLevel && isVisibleLevel);
        return newScanModeFlags;
    }
    
    

    public ItemSpawnLocation GetSpawnPointLocationForItem(ItemModel item) {
        var spawnLocation = itemSpawnLocationManager.FindForItem(item);
        if (spawnLocation == null) {
            Debug.LogWarning($"No spawn point in level \"{gameObject.name}\" ({levelState.sceneId}) for item \"{item.inkListItemFullName}\". Creating temporary.");
            spawnLocation = itemSpawnLocationManager.CreateSpawnLocation(item.inkListItemFullName);    
            //
            // spawnLocation.rectTransform.anchoredPosition = RectTransformX.GetClampedAnchoredPositionInsideScreenRect(spawnLocation.rectTransform, spawnLocation.rectTransform.anchoredPosition, spawnLocationsRectTransform.GetScreenRect(), layout.rootCanvas.worldCamera);
        }

        return spawnLocation;
    }

    public void OnChangeLevelItems(IReadOnlyList<InkListItem> currentlistitems, IReadOnlyList<InkListItem> itemsadded, IReadOnlyList<InkListItem> itemsremoved) {
        foreach(var item in itemsremoved) {
            Debug.Log("Removed item: " + item);
            RemoveItemFromLevel(item);
        }
        foreach(var item in itemsadded) {
            Debug.Log("Added item: " + item);
            AddItemToLevel(item);
        }
    }

    ItemModel TryGetOrCreateItemModel(InkListItem item) {
        var itemModel = levelState.itemStates.FirstOrDefault(x => x.inkListItemFullName == item.fullName);
        if(itemModel == null) {
            itemModel = new ItemModel(item);
            levelState.itemStates.Add(itemModel);
        }
        return itemModel;
    }

    void OnChangeSolutionItemCount(string variablename, object newValue) {
        OnChangeCurrentLevelAnswerSet((int)StoryController.TryCoerce<int>(newValue));
    }

    public void OnChangeCurrentLevelAnswerSet(int newSlotCount) {// IReadOnlyList<InkListItem> currentlistitems, IReadOnlyList<InkListItem> itemsadded, IReadOnlyList<InkListItem> itemsremoved) {
        slotGroup.Init(newSlotCount);
    }

    void AddItemToLevel(InkListItem inkListItem) {
        var item = TryGetOrCreateItemModel(inkListItem);
        if (GameController.Instance.gameSettings.immediatelyScanAllItems) item.state = ItemModel.State.Showing;
        CreateItemView(item);
    }

    void RemoveItemFromLevel(InkListItem inkListItem) {
        var itemModel = levelState.itemStates.First(itemModel => itemModel.inkListItem.fullName == inkListItem.fullName);
        levelState.itemStates.Remove(itemModel);
        DestroyItemView(itemModel);
    }

    void CreateItemView(ItemModel itemModel) {
        var itemView = Instantiate(PrefabDatabase.Instance.itemViewPrefab, itemContainer);
        itemViews.Add(itemView);
        var itemSpawnLocation = GetSpawnPointLocationForItem(itemModel);
        itemView.Init(itemModel, itemSpawnLocation);
    }

    void DestroyItemView(ItemModel itemModel) {
        itemViews.Where(view => view.itemModel == itemModel).ToList().ForEach(view => {
            itemViews.Remove(view);
            Destroy(view.gameObject);
        });
    }
    
    public ItemDraggableGhostView CreateDraggableGhostItemView(ItemModel itemModel) {
        var item = Instantiate(PrefabDatabase.Instance.itemDraggableGhostViewPrefab, itemContainer);
        item.Init(itemModel);
        return item;
    }
    
    public void OnSlotItem(ItemDraggableGhostView itemDraggableGhost) {
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
        var slottedItemLists = slotGroup.slottedItems.Select(itemView => InkList.FromString(itemView.itemModel.inkListItemFullName, StoryController.Instance.story)).ToList();
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
                    var slottedGhostItemView = slotGroup.slottedItems.FirstOrDefault(x => x.itemModel.inkListItem.Equals(itemList.Key));
                    if (slottedGhostItemView == null) continue;
                    slottedGhostItemView.ExitSlot();
                    slottedGhostItemView.draggable.SetDragTargetPosition(slottedGhostItemView.draggable.positionAtLastDragStart, false);
                    
                    Destroy(slottedGhostItemView.gameObject);
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

    // Before saving we refresh the state to make sure it's up to date
    public void UpdateLevelState() {
        // foreach (var item in levelState.itemStates) {
        // }
    }
}
