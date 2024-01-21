using System;
using Ink.Runtime;
using TMPro;
using Unity.VisualScripting;
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
        text.text = inkListItem.itemName;
        tooltip.tooltipText = description = GameController.Instance.GetItemTooltip(inkListItem);
        UpdateSelectionState();
    }

    void OnClicked(Draggable arg1, PointerEventData arg2) {
        tooltip.HideTooltip();
        GameController.Instance.InteractWithItem(inkListItem);
    }
    
    void OnStartDragging() {
        tooltip.enabled = false;
        UpdateSelectionState();
    }

    void OnStopDragging() {
        tooltip.enabled = true;
        UpdateSelectionState();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Debug.Log("OnPointerEnter");
        hovered = true;
        UpdateSelectionState();
    }

    public void OnPointerExit(PointerEventData eventData) {
        Debug.Log("OnPointerExit");
        hovered = false;
        UpdateSelectionState();
    }

    void UpdateSelectionState() {
        if (draggable.dragging) selectionState = SelectionState.Dragging;
        else if(hovered) selectionState = SelectionState.Hovered;
        else selectionState = SelectionState.Normal;
        background.Animate(Styling.FastAnimationTime, () => {
            Layout();
        });
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