using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(SLayout))]
public class ModalMenuPage : MonoBehaviour {
    public ModalMenuPageModel model;
    public List<ModalMenuItemViewBase> items = new List<ModalMenuItemViewBase>();

    public SLayout layout => GetComponent<SLayout>();
    // public RectOffset padding;
    public float targetDistance;
    void Update () {
        Layout();
    }

    public void Layout () {
        var y = 0f;
        foreach(var item in items) {
            item.layout.y = y;
            item.layout.width = layout.width;
            item.Layout();
            y += item.layout.height;
        }
        layout.height = y;
        
        RefreshPosition();
    }

    // TODO - Improve performance here. This sets layout positions multiple times in one pass, which means it causes a canvas layout refresh!
    public void RefreshPosition () {
        var pivotPosition = layout.CanvasToSLayoutSpace(model.positionParams.pivotPosition);
        if(model.positionParams.arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Top) {
            layout.centerX = pivotPosition.x;
            layout.topY = pivotPosition.y + targetDistance;

            // arrowImage.centerX = layout.width * 0.5f;
            // arrowImage.topY = -arrowImage.height + settings.arrowMargin.bottom;
            // arrowImage.rotation = 180;
        } else if(model.positionParams.arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Right) {
            layout.rightX = pivotPosition.x - targetDistance;
            layout.centerY = pivotPosition.y;
            
            // arrowImage.centerX = layout.width + arrowImage.height * 0.5f - settings.arrowMargin.left;
            // arrowImage.centerY = layout.height * 0.5f;
            // arrowImage.rotation = 90;
        } else if(model.positionParams.arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Bottom) {
            layout.centerX = pivotPosition.x;
            layout.bottomY = pivotPosition.y - targetDistance;
            
            // arrowImage.centerX = layout.width * 0.5f;
            // arrowImage.y = layout.height - settings.arrowMargin.top;
            // arrowImage.rotation = 0;
        } else if(model.positionParams.arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Left) {
            layout.x = pivotPosition.x + targetDistance;
            layout.centerY = pivotPosition.y;

            // arrowImage.centerX = -arrowImage.height * 0.5f + settings.arrowMargin.right;
            // arrowImage.centerY = layout.height * 0.5f;
            // arrowImage.rotation = -90;
        } else if(model.positionParams.arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.BottomRight) {
            layout.rightX = pivotPosition.x - targetDistance;
            layout.bottomY = pivotPosition.y - targetDistance;
        }

        // Clamp inside the parent rect
        var newRect = ClampInsideKeepSize(layout.rect, new Rect(0,0,layout.parentRect.width,layout.parentRect.height));
        var offset = newRect.position - layout.rect.position;
        // var clampedArrowImageOffset = Vector2.zero;
        // {
        //     if(offset.x != 0 && (model.positionParams.arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Top || model.positionParams.arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Bottom)) {
        //         var minX = (-layout.width+arrowImage.width)*0.5f+settings.arrowEdgeMargin;
        //         var maxX = (layout.width-arrowImage.width)*0.5f-settings.arrowEdgeMargin;
        //         if(minX < maxX) clampedArrowImageOffset.x = Mathf.Clamp(offset.x, minX, maxX);
        //     }
        //     if(offset.y != 0 && (model.positionParams.arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Left || model.positionParams.arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Right)) {
        //         var minY = (-layout.height+arrowImage.height)*0.5f+settings.arrowEdgeMargin;
        //         var maxY = (layout.height-arrowImage.height)*0.5f-settings.arrowEdgeMargin;
        //         if(minY < maxY) clampedArrowImageOffset.y = Mathf.Clamp(offset.y, minY, maxY);
        //     }
        //     arrowImage.position -= clampedArrowImageOffset;
        // }
        layout.position += offset;


        Rect ClampInsideKeepSize(Rect r, Rect container) {
            Rect rect = Rect.zero;
            rect.xMin = Mathf.Max(r.xMin, container.xMin);
            rect.xMax = Mathf.Min(r.xMax, container.xMax);
            rect.yMin = Mathf.Max(r.yMin, container.yMin);
            rect.yMax = Mathf.Min(r.yMax, container.yMax);
            
            if(r.xMin < container.xMin) rect.width += container.xMin - r.xMin;
            if(r.yMin < container.yMin) rect.height += container.yMin - r.yMin;
            if(r.xMax > container.xMax) {
                rect.x -= r.xMax - container.xMax;
                rect.width += r.xMax - container.xMax;
            }
            if(r.yMax > container.yMax) {
                rect.y -= r.yMax - container.yMax;
                rect.height += r.yMax - container.yMax;
            }

            // Finally make sure we're fully contained
            rect.xMin = Mathf.Max(rect.xMin, container.xMin);
            rect.xMax = Mathf.Min(rect.xMax, container.xMax);
            rect.yMin = Mathf.Max(rect.yMin, container.yMin);
            rect.yMax = Mathf.Min(rect.yMax, container.yMax);
            return rect;
        }
    }
}
