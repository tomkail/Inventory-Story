using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTooltip : UIBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public TooltipView tooltip;
    public new Camera camera => GetComponentInParent<Canvas>().rootCanvas.worldCamera;
    public RectTransform rectTransform => (RectTransform)transform;
    public string tooltipText;
    
    public void OnPointerEnter(PointerEventData eventData) {
        Debug.Log("Tooltip Exit");
        TryCreateTooltipIfNotShowing();
    }

    public void OnPointerExit(PointerEventData eventData) {
        Debug.Log("Tooltip Enter");
        HideTooltip();
    }

    protected override void OnDisable() {
        base.OnDisable();
        HideTooltip();
    }

    void Update() {
        if(tooltip != null) {
            var screenRect = camera.WorldToScreenRect(new Bounds(rectTransform.position, rectTransform.localScale));
            var tooltipPositionParams = TooltipViewManager.Instance.GetPositionParamsFromScreenRect(screenRect, TooltipParams.TooltipPositionParams.ArrowDirection.Top);
            tooltip.tooltipParams.positionParams = tooltipPositionParams;
            tooltip.RefreshPosition();
        }
    }

    public void TryCreateTooltipIfNotShowing() {
        if(tooltip != null) return;
        if(string.IsNullOrWhiteSpace(tooltipText)) return;
        var screenRect = camera.WorldToScreenRect(new Bounds(rectTransform.position, rectTransform.localScale));
        var tooltipPositionParams = TooltipViewManager.Instance.GetPositionParamsFromScreenRect(screenRect, TooltipParams.TooltipPositionParams.ArrowDirection.Top);
        if(tooltip == null) {
            var tooltipContent = TooltipViewManager.Instance.CreateTooltipLabelContent(new TooltipLine(tooltipText, TooltipLine.TooltipLineType.Normal));
            tooltip = TooltipViewManager.Instance.CreateAndShow(new TooltipParams(this, tooltipPositionParams, tooltipContent.layout, null));
        }
    }

    public void HideTooltip() {
        TooltipViewManager.Instance.Hide(tooltip);
        tooltip = null;
    }
}