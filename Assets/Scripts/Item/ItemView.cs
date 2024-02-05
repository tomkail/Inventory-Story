using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using Ink.Runtime;
using ThisOtherThing.UI.ShapeUtils;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISlot, ISlottable {
    public const float minWidth = 160;
    public const float maxWidth = 340;
    public const float margin = 20;
    
    public LevelController levelController => GetComponentInParent<LevelController>(true);
    public SLayout layout => GetComponent<SLayout>();
    public SLayout background;
    public Draggable draggable => GetComponent<Draggable>();
    public HoverTooltip tooltip => GetComponent<HoverTooltip>();
    public ThisOtherThing.UI.Shapes.Line lineRenderer => GetComponentInChildren<ThisOtherThing.UI.Shapes.Line>();

    public InkListItem inkListItem;
    public SLayout labelLayout;
    public string description;
    
    bool hovered;
    public SelectionState selectionState;
    public enum SelectionState {
        Normal,
        Hovered,
        Dragging,
    }
    

    // The target that the line points to
    public Vector2 targetLocalPoint = Vector2.zero;

    // ISlot
    public ISlottable hoveredSlottable { get; private set; }

    // ISlottable
    public ISlot hoveredSlot { get; private set; }
    public ISlot containerSlot { get; private set; }

    public IEnumerable<ISlot> GetEnterableSlots() {
        return GameController.Instance.sceneController.currentLevelController.itemViews.Where(x => x != this).Concat(GameController.Instance.sceneController.currentLevelController.slotGroup.slots.Cast<ISlot>());
    }

    public Vector2 draggableScreenPoint => draggable.lastPointerEventData.position;
    public Camera draggableScreenCamera => draggable.lastPointerEventData.enterEventCamera;

    void Awake() {
        draggable.OnStartDragging += OnStartDragging;
        draggable.OnStopDragging += OnStopDragging;
        draggable.OnDragged += OnDragged;
        draggable.OnClicked += OnClicked;
    }

    public void Init(InkListItem inkListItem) {
        this.inkListItem = inkListItem;
        
        gameObject.name = $"Item: {this.inkListItem.itemName}";
        labelLayout.textMeshPro.text = GameController.Instance.GetItemName(inkListItem);
        tooltip.tooltipText = description = GameController.Instance.GetItemTooltip(inkListItem);
        Layout();
        UpdateSelectionState();
    }

    [Button]
    public void SetWorldPosition(Vector3 worldPosition) {
        layout.position = layout.WorldToSLayoutPosition(worldPosition);
        draggable.SetPositionImmediate(layout.rectTransform.anchoredPosition);
    }

    void Update() {
        tooltip.enabled = levelController.draggingItem == null;
        UpdateDrag();
    }
    void LateUpdate() {
        if (containerSlot != null) {
            RectTransformX.ScreenRectToLocalRectInRectangle((RectTransform)layout.rectTransform.parent, containerSlot.GetScreenRectSlotForSlottable(this), Camera.main, out Rect localRect);
            var centerOffset = (localRect.size - layout.rectTransform.rect.size)*0.5f;
            draggable.SetPositionImmediate(layout.rectTransform.GetLocalToAnchoredPositionOffset() + localRect.GetPointFromNormalizedPoint(layout.rectTransform.pivot) + centerOffset);
        }
        UpdateLine();
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
    void Layout() {
        var minContentSize = minWidth + margin * 2;
        var maxContentSize = maxWidth - margin * 2;
        labelLayout.textMeshPro.ApplyTightPreferredSize(maxWidth-margin * 2);

        if (hoveredSlottable != null) {
            var screenRect = GetSlotTriggerScreenRectWhenContainingItem(hoveredSlottable);
            layout.position = layout.ScreenToSLayoutRect(screenRect).position;
            layout.size = layout.ScreenToSLayoutRect(screenRect).size;
        } else {
            // if(slottedSlot)
            layout.rect = new Rect(layout.targetRect.BottomLeft(), GetRegularSize());
            
        }
    }

    Vector2 GetRegularSize() {
        var minContentSize = minWidth + margin * 2;
        var maxContentSize = maxWidth - margin * 2;
        var labelSize = labelLayout.textMeshPro.GetTightPreferredValues(maxWidth-margin * 2);
        var width = SLayoutUtils.AutoLayoutWithDynamicSizing(layout, new List<LayoutItem>() {new (LayoutItemParams.Fixed(Mathf.Clamp(labelSize.x, minContentSize, maxContentSize)))}, SLayoutUtils.Axis.X, 0, margin, margin, 0);
        var height = SLayoutUtils.AutoLayoutWithDynamicSizing(layout, new List<LayoutItem>() {new (LayoutItemParams.Fixed(labelSize.y))}, SLayoutUtils.Axis.Y, 0, margin, margin, 0);
        return new Vector2(width, height);
    }

    void OnClicked(Draggable arg1, PointerEventData arg2) {
        if (GameController.Instance.CanInteractWithItem(inkListItem)) {
            GameController.Instance.InteractWithItem(inkListItem);
            tooltip.HideTooltip();
        }
    }
    
    void OnStartDragging() {
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
        }
    }

    public void EnterSlot(ISlot slot) {
        if (containerSlot != null) {
            Debug.LogWarning($"{transform.HierarchyPath()} Trying to enter slot when already in slot {containerSlot}!");
            return;
        } 
        containerSlot = slot;
        containerSlot.OnSlottableSlottedStart(this);
        if (containerSlot is LevelRequiredItemsSlot levelSlot) {
            levelController.OnSlotItem(this);
        } else if (containerSlot is ItemView slottedItem) {
            if (!GameController.Instance.CombineItems(inkListItem, slottedItem.inkListItem)) {
                ExitSlot();
                draggable.SetDragTargetPosition(draggable.positionAtLastDragStart, false);
            }
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
        draggable.dragTargetPosition = RectTransformX.GetClampedAnchoredPositionInsideScreenRect(draggable.rectTransform, draggable.dragTargetPosition, draggable.viewRect.GetScreenRect(), layout.rootCanvas.worldCamera);
        if (hoveredSlot != null) {
            RectTransformX.ScreenRectToLocalRectInRectangle((RectTransform)layout.rectTransform.parent, hoveredSlot.GetScreenRectSlotForSlottable(this), Camera.main, out Rect localRect);
            var centerOffset = (localRect.size - layout.rectTransform.rect.size)*0.5f;
            draggable.dragTargetPosition = layout.rectTransform.GetLocalToAnchoredPositionOffset() + localRect.GetPointFromNormalizedPoint(layout.rectTransform.pivot) + centerOffset;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (levelController.draggingItem != this) return;
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
    
    
    void UpdateLine() {
        Vector3 targetWorldPos = transform.parent.TransformPoint(targetLocalPoint);
            
        var endLocalPos = (Vector2)lineRenderer.transform.InverseTransformPoint(targetWorldPos);
        var targetScreenPos = RectTransformUtility.WorldToScreenPoint(layout.rootCanvas.worldCamera, targetWorldPos);
        
        
        var screenRect = layout.GetScreenRect();
        var itemCenterScreenPos = RectTransformUtility.WorldToScreenPoint(layout.rootCanvas.worldCamera, layout.rectTransform.TransformPoint(layout.rectTransform.rect.center));
        var startScreenPos = RectX.SplatVector(screenRect, targetScreenPos-itemCenterScreenPos);
        
        var screenLineLength = Vector2.Distance(startScreenPos, targetScreenPos) * (screenRect.Contains(targetScreenPos) ? -1 : 1);
        lineRenderer.GetComponent<SLayout>().groupAlpha = Mathf.InverseLerp(10, 40, screenLineLength);
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(lineRenderer.rectTransform, startScreenPos, layout.rootCanvas.worldCamera, out var startLocalPos);

        if (startLocalPos != endLocalPos) {
            var lineProperties = new PointsList.PointListProperties();
            lineProperties.Positions = new Vector2[2];
            lineProperties.Positions[0] = startLocalPos;
            lineProperties.Positions[1] = (Vector2)endLocalPos;
            lineProperties.SetPoints();
            
            lineRenderer.PointListsProperties.PointListProperties = new PointsList.PointListProperties[1];
            lineRenderer.PointListsProperties.PointListProperties[0] = lineProperties;
        } 
        
        lineRenderer.ForceMeshUpdate();
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
        if (!(slottable is ItemView slottableItem)) return default;
        var regularSize = GetRegularSize();
        var localRect = new Rect(0,0,regularSize.x,regularSize.y+10+slottableItem.layout.targetHeight+10);
        return layout.LocalToScreenRect(localRect);
    }

    public Rect GetScreenRectSlotForSlottable(ISlottable slottable) {
        if (!(slottable is ItemView slottableItem)) return default;
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
    
    
    
    void OnDrawGizmos() {
        UpdateLine();
        // draggable.SetPositionImmediate(draggable.rectTransform.anchoredPosition);
        // var rect = draggable.viewRect.rect; 
        // rect = rect.WithSize(rect.size - layout.size);
        // draggable.dragTargetPosition = rect.ClosestPoint(draggable.dragTargetPosition);
        // draggable.SetPositionImmediate(rect.ClosestPoint(draggable.dragTargetPosition));
    }
}