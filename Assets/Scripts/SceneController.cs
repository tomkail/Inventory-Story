using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour {
    public RectTransform itemContainer;
    public Image background;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI voText;
    public ItemView itemViewPrefab;
    public List<ItemView> itemViews = new List<ItemView>();
    public LevelRequiredItemsSlotGroup slotGroup;

    public void Clear() {
        foreach(var itemView in itemViews) Destroy(itemView.gameObject);
        itemViews.Clear();
        slotGroup.Clear();
    }
    
    public void PerformContent(ScriptContent content) {
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
        SaveLoadManager.Save();
        // Clear();
        // GameController.Instance.story.variablesState[GameController.Instance.levelItems.variableName] = new InkList();
        GameController.Instance.levelItems.Reset();
        GameController.Instance.levelItems.RefreshValue(GameController.Instance.story, false);
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

    public void OnChangeCurrentLevelAnswerSet(int newSlotCount) {// IReadOnlyList<InkListItem> currentlistitems, IReadOnlyList<InkListItem> itemsadded, IReadOnlyList<InkListItem> itemsremoved) {
        slotGroup.Init(newSlotCount);
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
        
        var inkList = new InkList();
        var slottedItemLists = slotGroup.slottedItems.Select(itemView => InkList.FromString(itemView.inkListItem.fullName, StoryController.Instance.story)).ToList();
        foreach (var slottedItem in slottedItemLists)
            inkList = inkList.Union(slottedItem);
        GameController.Instance.story.variablesState["currentItems"] = inkList;
        
        if (GameController.Instance.story.RunInkFunction<bool>("checkForSolution")) {
            var choice = GameController.Instance.story.currentChoices.FirstOrDefault(x => x.text.Contains("SOLVED"));
            StoryController.Instance.MakeChoice(choice.index);
        } else {
            foreach (var itemList in (InkList) GameController.Instance.story.variablesState["currentItems"]) {
                var itemView = itemViews.FirstOrDefault(x => x.inkListItem.Equals(itemList.Key));
                if(itemView == null) continue;
                itemView.draggable.SetDragTargetPosition(itemView.layout.rectTransform.anchoredPosition + MathX.DegreesToVector2(Random.Range(-45,45)) * 300, false);
                var slot = slotGroup.draggableGroup.slots.FirstOrDefault(x => x.slottedDraggable == itemView.draggable);
                slotGroup.draggableGroup.Unslot(slot);
            }
        }
    }
}