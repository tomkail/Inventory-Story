using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public SLayout layout => GetComponent<SLayout>();
    public SLayout background;
    public Draggable draggable => GetComponent<Draggable>();
    public HoverTooltip tooltip => GetComponent<HoverTooltip>();

    public InkListItem inkListItem;
    public TextMeshProUGUI text;
    public string description;
    
    bool hovered;
    public SelectionState selectionState;
    public enum SelectionState {
        Normal,
        Hovered,
        Dragging,
    }
    void Awake() {
        draggable.OnStartDragging += OnStartDragging;
        draggable.OnStopDragging += OnStopDragging;
        draggable.OnClicked += OnClicked;
    }

    public void Init(InkListItem inkListItem) {
        this.inkListItem = inkListItem;
        gameObject.name = $"Item: {this.inkListItem.itemName}";
        text.text = GameController.Instance.GetItemName(inkListItem);
        tooltip.tooltipText = description = GameController.Instance.GetItemTooltip(inkListItem);
        UpdateSelectionState();
    }

    void OnClicked(Draggable arg1, PointerEventData arg2) {
        if (GameController.Instance.CanInteractWithItem(inkListItem)) {
            GameController.Instance.InteractWithItem(inkListItem);
            tooltip.HideTooltip();
        }
    }
    
    void OnStartDragging() {
        transform.SetAsLastSibling();
        tooltip.enabled = false;
        UpdateSelectionState();
    }

    void OnStopDragging() {
        tooltip.enabled = true;
        UpdateSelectionState();
        GameController.Instance.OnCompleteDrag(this);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        transform.SetAsLastSibling();
        hovered = true;
        UpdateSelectionState();
    }

    public void OnPointerExit(PointerEventData eventData) {
        hovered = false;
        UpdateSelectionState();
    }

    void UpdateSelectionState() {
        if (draggable.dragging) selectionState = SelectionState.Dragging;
        else if(hovered) selectionState = SelectionState.Hovered;
        else selectionState = SelectionState.Normal;
        background.Animate(Styling.FastAnimationTime, Layout);
        void Layout() {
            if(selectionState == SelectionState.Dragging) {
                background.fillColor = new Color(0.25f,0.25f,0.25f,1);
                background.outlineColor = new Color(1,1,1,0.5f);
            }
            if(selectionState == SelectionState.Hovered) {
                background.fillColor = Color.black;
                background.outlineColor = new Color(1,1,1,0.5f);
            }
            if(selectionState == SelectionState.Normal) {
                background.fillColor = Color.black;
                background.outlineColor = new Color(1,1,1,0);
            }
        }
    }
}