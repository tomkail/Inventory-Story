using System;
using System.Collections.Generic;
using EasyButtons;
using Ink.Runtime;
using ThisOtherThing.UI.Shapes;
using ThisOtherThing.UI.ShapeUtils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
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
        layout.center = layout.WorldToSLayoutPosition(worldPosition);
        draggable.SetPositionImmediate(layout.rectTransform.anchoredPosition);
    }

    void LateUpdate() {
        UpdateLine();
    }

    [Button]
    void Layout() {
        var minContentSize = minWidth + margin * 2;
        var maxContentSize = maxWidth - margin * 2;
        labelLayout.textMeshPro.ApplyTightPreferredSize(maxWidth-margin * 2);
        layout.width = SLayoutUtils.AutoLayoutWithDynamicSizing(layout, new List<LayoutItem>() {new (LayoutItemParams.Fixed(Mathf.Clamp(labelLayout.width, minContentSize, maxContentSize)), labelLayout)}, SLayoutUtils.Axis.X, 0, margin, margin, 0);
        // layout.height = SLayoutUtils.AutoLayoutWithDynamicSizing(layout, new List<LayoutItem>() {new LayoutItem(LayoutItemParams.Fixed(Mathf.Clamp(labelLayout.height, minContentSize, maxContentSize)), labelLayout)}, SLayoutUtils.Axis.Y, 0, margin, margin, 0);
        layout.height = SLayoutUtils.AutoLayoutWithDynamicSizing(layout, new List<LayoutItem>() {LayoutItem.Fixed(labelLayout, SLayoutUtils.Axis.Y)}, SLayoutUtils.Axis.Y, 0, margin, margin, 0);
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
        levelController.OnCompleteItemDrag(this);
    }

    void OnDragged(Draggable draggable, PointerEventData e) {
        draggable.dragTargetPosition = GetClampedAnchoredPositionInsideParent(draggable.dragTargetPosition);
    }

    public Vector2 GetClampedAnchoredPositionInsideParent(Vector2 anchoredPosition) {
        var rect = draggable.viewRect.rect; 
        rect = rect.WithSize(rect.size - layout.size);
        return rect.ClosestPoint(anchoredPosition);
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

    void OnDrawGizmos() {
        UpdateLine();
        // draggable.SetPositionImmediate(draggable.rectTransform.anchoredPosition);
        // var rect = draggable.viewRect.rect; 
        // rect = rect.WithSize(rect.size - layout.size);
        // draggable.dragTargetPosition = rect.ClosestPoint(draggable.dragTargetPosition);
        // draggable.SetPositionImmediate(rect.ClosestPoint(draggable.dragTargetPosition));
    }

    public Vector2 targetLocalPoint = Vector2.zero;
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
}