using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using System.Linq;

// A tooltip! Tooltips have content which determines their size, and have a TooltipParams that defines where they should sit on screen. 
[RequireComponent(typeof(SLayout))]
[RequireComponent(typeof(Prototype))]
[RequireComponent(typeof(CanvasGroup))]
[ExecuteInEditMode]
public class TooltipView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField]
    TooltipViewSettings settings = null;
    public SLayout layout;
    [SerializeField]
    SLayout contentContainer = null;
    [SerializeField]
    SLayout body = null;
    [SerializeField]
    SLayout arrowImage = null;
    
    public TooltipParams tooltipParams = null;
    public object owner => tooltipParams.owner;

    [SerializeField, Disable]
    SLayout containedLayout;

    [Disable]
    public bool fullyShown;
    [Disable]
    public bool fullyHidden;
    [Disable]
    public bool showing;
    [Disable]
    public bool hiding;
    bool _hovered;
    public bool hovered => _hovered;
    float _timeShown;
    public float timeShown => _timeShown;
    public Color targetColor {
        get {
            if(tooltipParams.onClick == null) return settings.disabledColor;
            else if(hovered) return settings.hoveredColor;
            else return settings.interactableColor;
        }
    }
    void OnDisable () {
        if(Application.isPlaying) {
            fullyShown = false;
            fullyHidden = true;
            showing = false;
            hiding = false;
            _hovered = false;
            _timeShown = 0;
            layout.groupAlpha = 0;
            layout.CancelAnimations();
        }
    }

    void Update () {
        RefreshPosition();
        _timeShown += Time.deltaTime;

        if(body.targetColor != targetColor) {
            body.Animate(0.1f, () => {
                body.color = arrowImage.color = targetColor; 
            });
        }
    }

    public void Init (TooltipParams tooltipParams) {
        this.tooltipParams = tooltipParams;

        containedLayout = tooltipParams.containedLayout;
        tooltipParams.containedLayout.transform.SetParent(contentContainer.transform);
        
        contentContainer.size = tooltipParams.containedLayout.size;
        
        layout.width = contentContainer.width + (tooltipParams.padding == null ? 0 : tooltipParams.padding.horizontal);
        layout.height = contentContainer.height + (tooltipParams.padding == null ? 0 : tooltipParams.padding.vertical);
        
        contentContainer.position = tooltipParams.padding == null ? Vector2.zero : new Vector2(tooltipParams.padding.left, tooltipParams.padding.top);

        if(tooltipParams.containedLayout != null) {
            tooltipParams.containedLayout.centerX = tooltipParams.containedLayout.parent.width * 0.5f;
            tooltipParams.containedLayout.y = 0;
            tooltipParams.containedLayout.transform.localPosition = new Vector3(tooltipParams.containedLayout.transform.localPosition.x, tooltipParams.containedLayout.transform.localPosition.y, 0);
        }

        layout.canvasGroup.interactable = layout.canvasGroup.blocksRaycasts = tooltipParams.onClick != null;
        layout.color = arrowImage.color = targetColor; 

        RefreshPosition();
    }

    public void RefreshPosition () {
        var pivotPosition = layout.CanvasToSLayoutSpace(tooltipParams.positionParams.pivotPosition);
        if(tooltipParams.positionParams.arrowDirection == TooltipParams.TooltipPositionParams.ArrowDirection.Top) {
            layout.centerX = pivotPosition.x;
            layout.topY = pivotPosition.y + settings.targetDistance;

            arrowImage.centerX = layout.width * 0.5f;
            arrowImage.topY = -arrowImage.height + settings.arrowMargin.bottom;
            arrowImage.rotation = 180;
        } else if(tooltipParams.positionParams.arrowDirection == TooltipParams.TooltipPositionParams.ArrowDirection.Right) {
            layout.rightX = pivotPosition.x - settings.targetDistance;
            layout.centerY = pivotPosition.y;
            
            arrowImage.centerX = layout.width + arrowImage.height * 0.5f - settings.arrowMargin.left;
            arrowImage.centerY = layout.height * 0.5f;
            arrowImage.rotation = 90;
        } else if(tooltipParams.positionParams.arrowDirection == TooltipParams.TooltipPositionParams.ArrowDirection.Bottom) {
            layout.centerX = pivotPosition.x;
            layout.bottomY = pivotPosition.y - settings.targetDistance;
            
            arrowImage.centerX = layout.width * 0.5f;
            arrowImage.y = layout.height - settings.arrowMargin.top;
            arrowImage.rotation = 0;
        } else if(tooltipParams.positionParams.arrowDirection == TooltipParams.TooltipPositionParams.ArrowDirection.Left) {
            layout.x = pivotPosition.x + settings.targetDistance;
            layout.centerY = pivotPosition.y;

            arrowImage.centerX = -arrowImage.height * 0.5f + settings.arrowMargin.right;
            arrowImage.centerY = layout.height * 0.5f;
            arrowImage.rotation = -90;
        }

        // Clamp inside the parent rect
        var newRect = ClampInsideKeepSize(layout.rect, new Rect(0,0,layout.parentRect.width,layout.parentRect.height));
        var offset = newRect.position - layout.rect.position;
        var clampedArrowImageOffset = Vector2.zero;
        {
            if(offset.x != 0 && (tooltipParams.positionParams.arrowDirection == TooltipParams.TooltipPositionParams.ArrowDirection.Top || tooltipParams.positionParams.arrowDirection == TooltipParams.TooltipPositionParams.ArrowDirection.Bottom)) {
                var minX = (-layout.width+arrowImage.width)*0.5f+settings.arrowEdgeMargin;
                var maxX = (layout.width-arrowImage.width)*0.5f-settings.arrowEdgeMargin;
                if(minX < maxX) clampedArrowImageOffset.x = Mathf.Clamp(offset.x, minX, maxX);
            }
            if(offset.y != 0 && (tooltipParams.positionParams.arrowDirection == TooltipParams.TooltipPositionParams.ArrowDirection.Left || tooltipParams.positionParams.arrowDirection == TooltipParams.TooltipPositionParams.ArrowDirection.Right)) {
                var minY = (-layout.height+arrowImage.height)*0.5f+settings.arrowEdgeMargin;
                var maxY = (layout.height-arrowImage.height)*0.5f-settings.arrowEdgeMargin;
                if(minY < maxY) clampedArrowImageOffset.y = Mathf.Clamp(offset.y, minY, maxY);
            }
            arrowImage.position -= clampedArrowImageOffset;
        }
        layout.position += offset;
    }

    public void Show () {
        if(fullyShown || showing) return;
        hiding = fullyHidden = false;
        FullyHiddenEnd();

        showing = true;
        layout.CancelAnimations();
        
        var startY = layout.y - 20;
        var endY = layout.y;
        layout.y = startY;
        layout.groupAlpha = 0;
        layout.Animate(settings.tooltipFadeInTime, () => {
            layout.y = endY;
            layout.groupAlpha = 1;
        }).Then(() => {
            showing = false;
            fullyShown = true;
            FullyShownBegin();
        });
    }

    public void Hide () {
        if(fullyHidden || hiding) return;
        showing = fullyShown = false;
        FullyShownEnd();
        
        hiding = true;
        layout.CancelAnimations();
        layout.Animate(settings.tooltipFadeOutTime, () => {
            layout.groupAlpha = 0;
        }).Then(() => {
            hiding = false;
            fullyHidden = true;
            FullyHiddenBegin();
        });
    }

    protected virtual void FullyShownBegin () {}
    protected virtual void FullyShownEnd () {
        if(tooltipParams.onFullyShownEnd != null) tooltipParams.onFullyShownEnd(containedLayout);
    }
    protected virtual void FullyHiddenBegin () {
        GetComponent<Prototype>().ReturnToPool();
        if(tooltipParams.onFullyHidden != null) tooltipParams.onFullyHidden(containedLayout);
    }
    protected virtual void FullyHiddenEnd () {
        if(tooltipParams.onFullyHiddenEnd != null) tooltipParams.onFullyHiddenEnd(containedLayout);
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
        if(tooltipParams.onClick != null) tooltipParams.onClick();
    }

    public void OnPointerEnter(PointerEventData pointerEventData) {
        _hovered = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
        _hovered = false;
    }


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