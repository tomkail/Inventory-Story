using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDraggableGhostView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISlot, ISlottable {
    public const float minWidth = 160;
    public const float maxWidth = 340;
    public static readonly Vector2 margin = new Vector2(16,8);
    
    public Level level => GetComponentInParent<Level>(true);
    // public ItemView itemView;
    public SLayout layout => GetComponent<SLayout>();
    public SLayout background;
    public Draggable draggable => GetComponent<Draggable>();
    
    public ItemModel itemModel;
    public SLayout labelLayout;
    
    bool hovered;
    public SelectionState selectionState;
    public static ItemDraggableGhostView currentlyDraggingItem;

    public enum SelectionState {
        Normal,
        Hovered,
        Dragging,
    }

    // ISlot
    public ISlottable hoveredSlottable { get; private set; }

    // ISlottable
    public ISlot hoveredSlot { get; private set; }
    public ISlot containerSlot { get; private set; }

    public IEnumerable<ISlot> GetEnterableSlots() {
        var itemSlots = GameController.Instance.levelsManager.currentLevel.itemViews.Where(targetItemView => targetItemView.itemModel.state == ItemModel.State.Showing && !targetItemView.itemModel.Equals(itemModel)).Select(x => x.boxView);
        return itemSlots.Concat(GameController.Instance.levelsManager.currentLevel.slotGroup.slots.Cast<ISlot>());
    }

    public Vector2 draggableScreenPoint => draggable.lastPointerEventData.position;
    public Camera draggableScreenCamera => draggable.lastPointerEventData.enterEventCamera;

    void Awake() {
        draggable.OnStartDragging += OnStartDragging;
        draggable.OnStopDragging += OnStopDragging;
        draggable.OnDragged += OnDragged;
    }

    public void Init(ItemModel itemModel) {
        this.itemModel = itemModel;
        gameObject.name = $"Draggable Ghost Item View: {this.itemModel.inkListItemFullName}";
        labelLayout.textMeshPro.text = itemModel.labelText;
        Layout();
        UpdateSelectionState();
    }

    [Button]
    public void SetWorldPosition(Vector3 worldPosition) {
        layout.position = layout.WorldToSLayoutPosition(worldPosition);
        draggable.SetPositionImmediate(layout.rectTransform.anchoredPosition);
    }

    void Update() {
        UpdateDrag();
    }
    void LateUpdate() {
        if (containerSlot != null) 
            draggable.SetPositionImmediate(GetDraggableTargetPositionInSlot(containerSlot));
    }

    void OnGUI() {
        if (draggable.dragging) {
            foreach(var slot in GetEnterableSlots()) {
                OnGUIX.BeginColor(slot == hoveredSlot ? Color.cyan.WithAlpha(0.2f) : Color.gray.WithAlpha(0.2f));
                GUI.DrawTexture(OnGUIX.ScreenToGUIRect(slot.GetSlotTriggerScreenRectWhenContainingItem(this)), Texture2D.whiteTexture);
                OnGUIX.EndColor();
            }
        }
    }
    
    [Button]
    public void Layout() {
        var minContentWidth = minWidth + margin.x * 2;
        var maxContentWidth = maxWidth - margin.x * 2;
        labelLayout.textMeshPro.ApplyTightPreferredSize(maxWidth-margin.x * 2);

        if (hoveredSlottable != null) {
            var screenRect = GetSlotTriggerScreenRectWhenContainingItem(hoveredSlottable);
            layout.position = layout.ScreenToSLayoutRect(screenRect).position;
            layout.size = layout.ScreenToSLayoutRect(screenRect).size;
        } else {
            // if(slottedSlot)
            layout.rect = new Rect(layout.targetRect.BottomLeft(), GetRegularSize());
        }

        labelLayout.center = layout.rect.size * 0.5f;
    }

    Vector2 GetRegularSize() {
        var minContentWidth = minWidth + margin.x * 2;
        var maxContentWidth = maxWidth - margin.x * 2;
        var labelSize = labelLayout.textMeshPro.GetTightPreferredValues(maxWidth-margin.x * 2);
        var width = SLayoutUtils.AutoLayoutWithDynamicSizing(layout, new List<LayoutItem>() {new (LayoutItemParams.Fixed(Mathf.Clamp(labelSize.x, minContentWidth, maxContentWidth)))}, SLayoutUtils.Axis.X, 0, margin.x, margin.x, 0);
        var height = SLayoutUtils.AutoLayoutWithDynamicSizing(layout, new List<LayoutItem>() {new (LayoutItemParams.Fixed(labelSize.y))}, SLayoutUtils.Axis.Y, 0, margin.y, margin.y, 0);
        return new Vector2(width, height);
    }
    
    void OnStartDragging() {
        currentlyDraggingItem = this;
        transform.SetAsLastSibling();
        UpdateSelectionState();
        if (containerSlot != null) {
            ExitSlot();
        }
    }

    void OnStopDragging() {
        UpdateSelectionState();
        if (hoveredSlot != null) {
            hoveredSlot.OnSlottableHoverEnd(this);
            EnterSlot(hoveredSlot);
        } else {
            Destroy(gameObject);
        }
        currentlyDraggingItem = null;
    }

    public void EnterSlot(ISlot slot) {
        if (containerSlot != null) {
            Debug.LogWarning($"{transform.HierarchyPath()} Trying to enter slot when already in slot {containerSlot}!");
            return;
        } 
        containerSlot = slot;
        containerSlot.OnSlottableSlottedStart(this);
        if (containerSlot is LevelRequiredItemsSlot levelSlot) {
            level.OnSlotItem(this);
        } else if (containerSlot is ItemDraggableGhostView slottedItem) {
            if (!GameController.Instance.CombineItems(itemModel.inkListItem, slottedItem.itemModel.inkListItem)) {
                // ExitSlot();
                // draggable.SetDragTargetPosition(draggable.positionAtLastDragStart, false);
            }
            Destroy(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void ExitSlot() {
        if (containerSlot == null) {
            Debug.LogWarning($"{transform.HierarchyPath()} Trying to exit slot when not in slot!");
            return;
        }
        containerSlot.OnSlottableSlottedEnd(this);
        containerSlot = null;
    }

    void OnDragged(Draggable draggable, PointerEventData e) {
        draggable.dragTargetPosition = RectTransformX.GetClampedAnchoredPositionInsideScreenRect(draggable.rectTransform, draggable.dragTargetPosition, level.layout.GetScreenRect(), layout.rootCanvas.worldCamera);
        if (hoveredSlot != null) {
            draggable.dragTargetPosition = GetDraggableTargetPositionInSlot(hoveredSlot);
        }
    }
    
    Vector2 GetDraggableTargetPositionInSlot(ISlot slot) {
        RectTransformX.ScreenRectToLocalRectInRectangle((RectTransform)layout.rectTransform.parent, hoveredSlot.GetScreenRectSlotForSlottable(this), Camera.main, out Rect localRect);
        var centerOffset = (localRect.size - layout.rectTransform.rect.size)*0.5f;
        centerOffset.y = -centerOffset.y;
        return layout.rectTransform.GetLocalToAnchoredPositionOffset() + localRect.GetPointFromNormalizedPoint(layout.rectTransform.pivot) + centerOffset;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (level.draggingItemDraggableGhost != this) return;
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



    void UpdateDrag() {
        if (draggable.dragging) {
            var newHoveredSlot = GetNewHoveredSlot();
            if (newHoveredSlot != hoveredSlot) {
                if (hoveredSlot != null) {
                    hoveredSlot.OnSlottableHoverEnd(this);
                }

                hoveredSlot = newHoveredSlot;
                
                if (hoveredSlot != null) {
                    hoveredSlot.OnSlottableHoverStart(this);
                    // if(slot is ItemView hoveredItem) {
                    //     if(hoveredItem != this) {
                    //         hoveredItem.OnSlottableEnter(this);
                    //     }
                    // }
                }
            }
            // hoveredSlot = slot;
            // if(forceIntoSlotByDistance)
            //     Slot(draggable, slot);
        }
    }

    ISlot GetNewHoveredSlot() {
        foreach(var slot in GetEnterableSlots()) {
            if(slot.GetSlotTriggerScreenRectWhenContainingItem(this).Contains(draggableScreenPoint)) {
                return slot;
            }
        }
        return null;
    }


    public Rect slotTriggerScreenRect => layout.rectTransform.GetScreenRect();
    public Rect GetSlotTriggerScreenRectWhenContainingItem(ISlottable slottable) {
        if (!(slottable is ItemDraggableGhostView slottableItem)) return default;
        var regularSize = GetRegularSize();
        var localRect = new Rect(0,0,regularSize.x,regularSize.y+10+slottableItem.layout.targetHeight+10);
        return layout.LocalToScreenRect(localRect);
    }

    public Rect GetScreenRectSlotForSlottable(ISlottable slottable) {
        if (!(slottable is ItemDraggableGhostView slottableItem)) return default;
        var regularSize = GetRegularSize();
        var localRect = new Rect(0,regularSize.y+10,regularSize.x,slottableItem.layout.targetHeight);
        return layout.LocalToScreenRect(localRect);
    }

    public void OnSlottableHoverStart(ISlottable slottable) {
        hoveredSlottable = slottable;
        layout.Animate(Styling.StandardAnimationTime, EasingFunction.Ease.EaseInOutQuad, Layout);
    }
    
    public void OnSlottableHoverEnd(ISlottable slottable) {
        hoveredSlottable = null;
        layout.Animate(Styling.StandardAnimationTime, EasingFunction.Ease.EaseInOutQuad, Layout);
    }

    public void OnSlottableSlottedStart(ISlottable slottable) {
        layout.Animate(Styling.StandardAnimationTime, EasingFunction.Ease.EaseInOutQuad, Layout);
    }

    public void OnSlottableSlottedEnd(ISlottable slottable) {
        layout.Animate(Styling.StandardAnimationTime, EasingFunction.Ease.EaseInOutQuad, Layout);
    }
}