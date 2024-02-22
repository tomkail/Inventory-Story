using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTooltip : UIBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [Disable] public TooltipView tooltip;
    public new Camera camera => GetComponentInParent<Canvas>().rootCanvas.worldCamera;
    public RectTransform rectTransform => (RectTransform)transform;
    public string tooltipText { get; private set; }
    
    public void OnPointerEnter(PointerEventData eventData) {
        TryCreateTooltipIfNotShowing();
    }

    public void OnPointerExit(PointerEventData eventData) {
        HideTooltip();
    }

    protected override void OnDisable() {
        base.OnDisable();
        HideTooltip();
    }

    void Update() {
        if(tooltip != null) {
            var tooltipPositionParams = TooltipViewManager.Instance.GetPositionParamsFromScreenRect(rectTransform.GetScreenRect(), TooltipViewManager.GetPreferredArrowDirection(rectTransform.GetScreenRect()));
            tooltip.tooltipParams.positionParams = tooltipPositionParams;
            tooltip.RefreshPosition();
        }
    }

    public void SetTooltipText(string tooltipText, bool refreshCurrentTooltip = true) {
        this.tooltipText = tooltipText;
        if(refreshCurrentTooltip && tooltip != null) {
            HideTooltip();
            TryCreateTooltipIfNotShowing();
        }
    }

    public void TryCreateTooltipIfNotShowing() {
        if(tooltip != null) return;
        if(string.IsNullOrWhiteSpace(tooltipText)) return;
        var tooltipPositionParams = TooltipViewManager.Instance.GetPositionParamsFromScreenRect(rectTransform.GetScreenRect(), TooltipViewManager.GetPreferredArrowDirection(rectTransform.GetScreenRect()));
        if(tooltip == null) {
            var tooltipContent = TooltipViewManager.Instance.CreateTooltipLabelContent(new TooltipLine(tooltipText, TooltipLine.TooltipLineType.Normal));
            tooltip = TooltipViewManager.Instance.CreateAndShow(new TooltipParams(this, tooltipPositionParams, tooltipContent.layout, null));
        }
    }

    public void HideTooltip() {
        if(TooltipViewManager.Instance != null) TooltipViewManager.Instance.Hide(tooltip);
        tooltip = null;
    }
}