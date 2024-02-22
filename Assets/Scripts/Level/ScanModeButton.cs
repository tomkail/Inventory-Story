using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScanModeButton : MonoBehaviour, IPointerClickHandler {
    public enum ScanModeButtonState {
        Hidden,
        Disabled,
        Highlighted,
        Toggled,
    }
    public SLayout layout => GetComponent<SLayout>();
    public HoverTooltip hoverTooltip => GetComponent<HoverTooltip>();
    public SLayout fill;
    public SLayout icon;
    public ScanModeButtonState scanModeButtonState;

    [SerializeField] ScanModeButtonVisualState hiddenState;
    [SerializeField] ScanModeButtonVisualState disabledState;
    [SerializeField] ScanModeButtonVisualState highlightedState;
    [SerializeField] ScanModeButtonVisualState toggledState;
    [System.Serializable]
    public struct ScanModeButtonVisualState {
        public float groupAlpha;
        public Color color;
        public Color backgroundColor;
    }
    
    void Update () {
        if(scanModeButtonState == ScanModeButtonState.Highlighted) hoverTooltip.SetTooltipText("Activate [Space] the scanner");
        else if(scanModeButtonState == ScanModeButtonState.Toggled) hoverTooltip.SetTooltipText("Deactivate [Space] the scanner");
        else hoverTooltip.SetTooltipText("");
        
        var newScanModeState = RefreshButtonState();
        if (scanModeButtonState != newScanModeState) {
            scanModeButtonState = newScanModeState;
            layout.Animate(0.2f, () => {
                Layout();
            });
        } 
    }

    [EasyButtons.Button]
    void Layout() {
        ScanModeButtonVisualState visualState = default;
        if (scanModeButtonState == ScanModeButtonState.Hidden) visualState = hiddenState;
        if (scanModeButtonState == ScanModeButtonState.Disabled) visualState = disabledState;
        if (scanModeButtonState == ScanModeButtonState.Highlighted) visualState = highlightedState;
        if (scanModeButtonState == ScanModeButtonState.Toggled) visualState = toggledState;
        
        layout.groupAlpha = visualState.groupAlpha;
        fill.outlineColor = visualState.color;
        fill.fillColor = visualState.backgroundColor;
        icon.color = visualState.color;
        
    }
    
    ScanModeButtonState RefreshButtonState() {
        if (GameController.Instance.levelsManager.currentLevel == null) return ScanModeButtonState.Hidden;
        var scanModeFlags = GameController.Instance.levelsManager.currentLevel.scanModeFlags;
        if (!scanModeFlags.HasFlag(ScanModeStateFlags.Permissable)) return ScanModeButtonState.Hidden;
        if (GameController.Instance.levelsManager.visibleLevel == GameController.Instance.levelsManager.currentLevel) {
            if (!scanModeFlags.HasFlag(ScanModeStateFlags.Usable)) return ScanModeButtonState.Disabled;
            else {
                if (scanModeFlags.HasFlag(ScanModeStateFlags.Active)) return ScanModeButtonState.Toggled;
                else return ScanModeButtonState.Highlighted;
            }
        } else {
            return ScanModeButtonState.Disabled;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        GameController.Instance.levelsManager.currentLevel.OnClickScanModeButton();
    }
}