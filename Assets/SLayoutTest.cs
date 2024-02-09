using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SLayoutTest : MonoBehaviour {
    
    public RectTransform rt => (RectTransform)transform;
    public SLayout layout;
    
    void Update() {
        // layout.rectTransform.anchoredPosition = GetClampedAnchoredPositionInsideParent(layout.rectTransform.anchoredPosition);
        // layout.rectTransform.localPosition = RectTransformX.GetClampedLocalPositionInsideScreenRect(layout.rectTransform, layout.rectTransform.localPosition, rt.GetScreenRect(), layout.canvas.rootCanvas.worldCamera);
        layout.rectTransform.anchoredPosition = RectTransformX.GetClampedAnchoredPositionInsideScreenRect(layout.rectTransform, layout.rectTransform.anchoredPosition, rt.GetScreenRect(), layout.canvas.rootCanvas.worldCamera);
    }
}
