using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
[ExecuteAlways]
public class ZoomedInImage : MonoBehaviour {
    RawImage rawImage => GetComponent<RawImage>();
    public Graphic target;
    // Optional
    [SerializeField, Disable] RectTransform _targetRect;
    public RectTransform targetRect {
        get {
            if (_targetRect == null) return rawImage.rectTransform;
            return _targetRect;
        }
        set => _targetRect = value;
    }
    public ScaleToContainerUtils.ScalingMode scalingMode = ScaleToContainerUtils.ScalingMode.AspectFill;
    public float zoom = 1;
    
    void LateUpdate() {
        if(target == null) return;
        rawImage.texture = target.mainTexture;
        var rect = TransformRectToWithoutRotation(targetRect, targetRect.rect, target.rectTransform);
        rect = RectX.CreateFromCenter(rect.center, ScaleToContainerUtils.Resize(rect.size, rawImage.rectTransform.rect.size, scalingMode));
        rect = RectX.MinMaxRect(target.rectTransform.rect.GetNormalizedPositionInsideRect(rect.min), target.rectTransform.rect.GetNormalizedPositionInsideRect(rect.max));
        rect = RectX.CreateFromCenter(rect.center, rect.size * 1f/zoom);
        rawImage.uvRect = rect;
    }
    
    static Rect TransformRectToWithoutRotation (RectTransform rectTransform, Rect rect, Transform otherRectTransform) {
        static Vector3 TransformPointToWithoutRotation(Transform fromTransform, Vector3 localPoint, Transform toTransform) {
            Matrix4x4 localToWorldMatrix = Matrix4x4.TRS(fromTransform.position, Quaternion.identity, fromTransform.lossyScale);
            Matrix4x4 worldToLocalMatrix = Matrix4x4.TRS(toTransform.position, Quaternion.identity, toTransform.lossyScale).inverse;
            return worldToLocalMatrix.MultiplyPoint3x4(localToWorldMatrix.MultiplyPoint3x4(localPoint));
        }
        return RectX.MinMaxRect (TransformPointToWithoutRotation (rectTransform, rect.min, otherRectTransform), TransformPointToWithoutRotation (rectTransform, rect.max, otherRectTransform));
    }
}
