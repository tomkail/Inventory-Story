using System.Linq;
using ThisOtherThing.UI.ShapeUtils;
using UnityEngine;

public class LevelSubPanel : LevelPanelBase {
    [Disable] public LevelPanelBase parentPanel;
    public ZoomedInImage zoomedInImage;
    
    [Space]
    public ThisOtherThing.UI.Shapes.Line lineRenderer;
    public int numRects = 4;
    
    ItemModel itemModel => level.levelState.itemStates.FirstOrDefault(x => x.inkListItemFullName == gameObject.name || x.inkListItemName == gameObject.name);
    ItemView targetItem => itemModel == null ? null : level.FindItemWithModel(itemModel);
    public float showProgress { get; set; }

    Rect expandedRect;

    public override void OnLoadLevel() {
        base.OnLoadLevel();
        expandedRect = layout.rect;
        // if (!isTopLevelPanel) {
        //     itemModel = 
        //     if(itemModel == null) Debug.LogWarning($"No item model found for level panel: {gameObject.name}", this);
        //     else targetItem = level.FindItemWithModel(itemModel);
        // }
    }


    [EasyButtons.Button]
    public void Show() {
        if (parentPanel != null && zoomedInImage != null) {
            zoomedInImage.targetRect = targetItem == null ? null : targetItem.layout.rectTransform;
            zoomedInImage.target = parentPanel.background;
        }
            
        gameObject.SetActive(true);
        layout.CancelAnimations();
        layout.AnimateCustom(0.5f, (progress) => {
            showProgress = progress;
            
        });
        layout.Animate(0.5f, EasingFunction.Ease.EaseInOutQuad, LayoutShow);
    }

    [EasyButtons.Button]
    public void Hide() {
        layout.CancelAnimations();
        layout.Animate(0.25f, EasingFunction.Ease.EaseInOutQuad, LayoutHide).Then(() => {
            gameObject.SetActive(false);
        });
    }

    void LayoutShow() {
        layout.groupAlpha = 1;
        layout.rect = expandedRect;
    }
    void LayoutHide() {
        layout.groupAlpha = 0;
        if(targetItem != null)
            layout.rect = layout.ScreenToSLayoutRect(targetItem.layout.GetScreenRect());
    }

    public void ShowImmediate() {
        gameObject.SetActive(true);
        LayoutShow();
    }
    public void HideImmediate() {
        gameObject.SetActive(false);
        LayoutHide();
    }

    void UpdateAnim() {
        var startScreenRect = targetItem.layout.GetScreenRect();
        var targetScreenRect = layout.GetScreenTargetRect(expandedRect);
        
        RectTransformX.ScreenRectToLocalRectInRectangle((RectTransform)lineRenderer.rectTransform.parent, startScreenRect, layout.rootCanvas.worldCamera, out Rect startRect);
        RectTransformX.ScreenRectToLocalRectInRectangle((RectTransform)lineRenderer.rectTransform.parent, targetScreenRect, layout.rootCanvas.worldCamera, out Rect endRect);
        
        var lineWeight = lineRenderer.OutlineProperties.LineWeight;
        var lineSpace = lineRenderer.OutlineProperties.LineWeight;
        var range = new Range(lineWeight * 0.5f, rectTransform.rect.width - lineWeight * 0.5f);
        
        lineRenderer.PointListsProperties.PointListProperties = new PointsList.PointListProperties[numRects];
        var pivotOffset = -(rectTransform.pivot) * rectTransform.rect.size;
        for (int i = 0; i < numRects; i++) {
            var x = i / ((float) numRects - 1);
            var rect = RectX.Lerp(startRect, endRect, x);
            var corners = rect.Corners();
            lineRenderer.PointListsProperties.PointListProperties[i] = new PointsList.PointListProperties();
            lineRenderer.PointListsProperties.PointListProperties[i].Positions = new Vector2[4];
            lineRenderer.PointListsProperties.PointListProperties[i].Positions[0] = corners[0] + pivotOffset;
            lineRenderer.PointListsProperties.PointListProperties[i].Positions[1] = corners[1] + pivotOffset;
            lineRenderer.PointListsProperties.PointListProperties[i].Positions[2] = corners[2] + pivotOffset;
            lineRenderer.PointListsProperties.PointListProperties[i].Positions[3] = corners[3] + pivotOffset;
            lineRenderer.PointListsProperties.PointListProperties[i].SetPoints();
        }
        lineRenderer.ForceMeshUpdate();
    }
}