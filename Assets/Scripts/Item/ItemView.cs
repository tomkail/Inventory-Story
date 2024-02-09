using Ink.Runtime;
using UnityEngine;

[RequireComponent(typeof(SLayout))]
public class ItemView : MonoBehaviour {
    public LevelController levelController => GetComponentInParent<LevelController>();
    public SLayout layout => GetComponent<SLayout>();
    public string labelText;
    public string tooltipText;

    public InkListItem inkListItem;
    public ItemSpawnLocation itemSpawnLocation;
    public ItemHintView hintView;
    public ItemLabelView labelView;
    public SLayout rectLayout;
    
    public void Init(InkListItem inkListItem, ItemSpawnLocation itemSpawnLocation) {
        this.inkListItem = inkListItem;
        this.itemSpawnLocation = itemSpawnLocation;
        
        labelText = GameController.Instance.GetItemName(inkListItem);
        tooltipText = GameController.Instance.GetItemTooltip(inkListItem);
        
        labelView = CreateItemLabelView();
        
        gameObject.name = $"Item View: {this.inkListItem.itemName}";
        
        var screenRect = itemSpawnLocation.rectTransform.GetScreenRect();
        layout.rect = layout.ScreenToSLayoutRect(screenRect);

        state = State.Hint;
        Layout();
    }

    public ItemLabelView CreateItemLabelView() {
        var itemLabel = Instantiate(PrefabDatabase.Instance.itemLabelViewPrefab, transform);
        itemLabel.itemSpawnLocation = itemSpawnLocation;
        var localSpawnPoint = (Vector2)itemLabel.layout.parentRectTransform.InverseTransformPoint(itemSpawnLocation.rectTransform.TransformPoint(itemSpawnLocation.rectTransform.rect.center)) + RandomX.onUnitCircle * Random.Range(100, 200);
        localSpawnPoint = itemLabel.draggable.dragTargetPosition = RectTransformX.GetClampedLocalPositionInsideScreenRect(itemLabel.draggable.rectTransform, localSpawnPoint, itemLabel.draggable.viewRect.GetScreenRect(), layout.rootCanvas.worldCamera);
        itemLabel.SetWorldPosition(itemLabel.layout.parentRectTransform.TransformPoint(localSpawnPoint));
        return itemLabel;
    }
    
    void Layout() {
        if (state == State.Hidden) {
            rectLayout.gameObject.SetActive(false);
            hintView.gameObject.SetActive(false);
            labelView.gameObject.SetActive(false);
        } else if (state == State.Hint) {
            rectLayout.gameObject.SetActive(false);
            hintView.gameObject.SetActive(true);
            labelView.gameObject.SetActive(false);
        } else if (state == State.Showing) {
            rectLayout.gameObject.SetActive(true);
            hintView.gameObject.SetActive(false);
            labelView.gameObject.SetActive(true);
        }
    }

    public State state;
    public enum State {
        Hidden,
        Hint,
        Showing
    }
    public void SetState(State state) {
        this.state = state;
        Layout();
    }
}