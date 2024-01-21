using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TooltipParams {
    [System.Serializable]
    public struct TooltipPositionParams {
        public Vector2 pivotPosition;
        public ArrowDirection arrowDirection;
        public enum ArrowDirection {
            Top,
            Bottom,
            Left,
            Right
        }
    }

    public object owner;
    public TooltipPositionParams positionParams;

    public SLayout containedLayout;
    public System.Action<SLayout> onFullyHiddenEnd;
    public System.Action<SLayout> onFullyShownEnd;
    public System.Action<SLayout> onFullyHidden;
    public System.Action onClick;
    
    public RectOffset padding;
    
    public TooltipParams (object owner, TooltipPositionParams positionParams, SLayout containedLayout, System.Action<SLayout> onFullyHidden) {
        this.owner = owner;
        this.positionParams = positionParams;
        this.containedLayout = containedLayout;
        this.onFullyHidden = onFullyHidden;
    }
}
