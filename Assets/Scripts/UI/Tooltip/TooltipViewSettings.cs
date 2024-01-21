using UnityEngine;

public class TooltipViewSettings : ScriptableObject {
    public float targetDistance = 20;
    [Space]
    public RectOffset arrowMargin;
    [Tooltip("The distance from the edge of the tooltip that the arrow can pushed to when the tooltip is pushing against the boundaries of its container.")]
    public float arrowEdgeMargin;
    [Space]
    public float tooltipFadeInTime = 0.15f;
    public float tooltipFadeOutTime = 0.25f;
    [Space]
    public Color disabledColor = new Color(0f,0f,0f,1);
    public Color interactableColor = new Color(0.2f,0.2f,0.2f,1);
    public Color hoveredColor = new Color(0.1f,0.1f,0.1f,1);
}
