using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SetInParentTest : MonoBehaviour {
    SLayout layout => GetComponent<SLayout>();
    RectTransform rectTransform => layout.rectTransform;
    void Update() {
        var rectSize = layout.size;
        var pivotOffset = rectSize * (rectTransform.pivot);
        var localContainerRect = ((RectTransform)transform.parent).rect;
        localContainerRect = new Rect(localContainerRect.x+pivotOffset.x, localContainerRect.y+pivotOffset.y, localContainerRect.width-rectSize.x, localContainerRect.height-rectSize.y);
            
        var randomLocalPos = new Vector2(Random.Range(localContainerRect.xMin, localContainerRect.xMax), Random.Range(localContainerRect.yMin, localContainerRect.yMax));
        rectTransform.localPosition = randomLocalPos;

    }
}
