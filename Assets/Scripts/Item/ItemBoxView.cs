using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SLayout))]
public class ItemBoxView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, ISlot {
    public Level level => GetComponentInParent<Level>(true);
    ItemView itemView;
    public SLayout layout => GetComponent<SLayout>();
    public SLayout background ;
    public SLayout labelView;
    public HoverTooltip tooltip => GetComponent<HoverTooltip>();
    ItemDraggableGhostView draggableGhostItemView;
    
    bool hovered;
    public VisualState visualState;
    public enum VisualState {
        Normal,
        Hovered
    }

    public void Init(ItemView itemView) {
        this.itemView = itemView;
        labelView.textMeshPro.text = itemView.itemModel.labelText;
        tooltip.SetTooltipText(itemView.itemModel.tooltipText);
    }

    void Update() {
        tooltip.enabled = level.draggingItemDraggableGhost == null;
    }
    
    public void OnPointerEnter(PointerEventData eventData) {
        hovered = true;
        UpdateSelectionState();
    }

    public void OnPointerExit(PointerEventData eventData) {
        hovered = false;
        UpdateSelectionState();
    }

    public void OnPointerDown(PointerEventData eventData) {
        UpdateSelectionState();
        tooltip.HideTooltip();

        if (eventData.button == PointerEventData.InputButton.Right && itemView.itemModel.isZoomable) {
            itemView.StartZoomingOnContainer();
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right && itemView.itemModel.isZoomable) {
            itemView.EndZoomingOnContainer();
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        // if (GameController.Instance.CanInteractWithItem(itemView.itemModel)) {
        //     GameController.Instance.InteractWithItem(itemView.itemModel);
        // }
    }
    
    // This is fired after OnPointerDown, after the eventData has been set up sufficiently that the drag (and other events like clicks) can be re-directed
    public void OnInitializePotentialDrag(PointerEventData eventData) {
        if(eventData.button == PointerEventData.InputButton.Left) {
            draggableGhostItemView = level.CreateDraggableGhostItemView(itemView.itemModel);
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)draggableGhostItemView.draggable.rectTransform.parent, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
            draggableGhostItemView.draggable.SetPositionImmediate(localPoint + draggableGhostItemView.draggable.rectTransform.GetLocalToAnchoredPositionOffset() + new Vector2(-draggableGhostItemView.draggable.rectTransform.rect.width * 0.5f, draggableGhostItemView.draggable.rectTransform.rect.height * 0.5f));
            
            EventSystemX.RedirectPotentialDrag(eventData, draggableGhostItemView.gameObject);
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData) {
        // draggableGhostItemView = Level.CreateDraggableGhostItemView(itemView.itemModel);
        // RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)draggableGhostItemView.draggable.rectTransform.parent, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        // draggableGhostItemView.draggable.SetPositionImmediate(localPoint + draggableGhostItemView.draggable.rectTransform.GetLocalToAnchoredPositionOffset() + new Vector2(-draggableGhostItemView.draggable.rectTransform.rect.width * 0.5f, draggableGhostItemView.draggable.rectTransform.rect.height * 0.5f));
        //
        // EventSystemX.RedirectPotentialDrag(eventData, draggableGhostItemView.gameObject);
        
        UpdateSelectionState();
    }

    public void OnEndDrag(PointerEventData eventData) {
        UpdateSelectionState();
    }

    public void OnDrag(PointerEventData eventData) {
        UpdateSelectionState();
    }

    public void OnDrop(PointerEventData eventData) {
        UpdateSelectionState();
    }
    
    void UpdateSelectionState() {
        if(hovered) visualState = VisualState.Hovered;
        else visualState = VisualState.Normal;
        background.Animate(Styling.FastAnimationTime, Layout);
        void Layout() {
            if(visualState == VisualState.Hovered) {
                background.fillColor = Color.white.WithAlpha(0.15f);
                background.outlineColor = Color.white;
            }
            if(visualState == VisualState.Normal) {
                background.fillColor = Color.white.WithAlpha(0.05f);
                background.outlineColor = Color.white;
            }
        }
    }
    
    // ISlot
    public ISlottable hoveredSlottable { get; private set; }
    public Rect slotTriggerScreenRect => layout.rectTransform.GetScreenRect();
    public Rect GetSlotTriggerScreenRectWhenContainingItem(ISlottable slottable) {
        if (!(slottable is ItemDraggableGhostView slottableItem)) return default;
        return layout.GetScreenRect();
    }

    public Rect GetScreenRectSlotForSlottable(ISlottable slottable) {
        if (!(slottable is ItemDraggableGhostView slottableItem)) return default;
        return layout.GetScreenRect();
    }

    public void OnSlottableHoverStart(ISlottable slottable) {
        hoveredSlottable = slottable;
    }
    
    public void OnSlottableHoverEnd(ISlottable slottable) {
        hoveredSlottable = null;
    }

    public void OnSlottableSlottedStart(ISlottable slottable) {
    }

    public void OnSlottableSlottedEnd(ISlottable slottable) {
    }
}