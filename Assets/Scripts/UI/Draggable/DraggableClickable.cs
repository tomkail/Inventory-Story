using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Simple base class that uses the standard Unity EventSystem callbacks, and distinguishes between drag and click,
/// used in the timeline and the map for clicking and dragging for panning around.
/// </summary>
public class DraggableClickable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public bool pointerIsOver;
    public bool dragging;
    public Vector2 dragVelocity;

    public event Action<Vector2> onClick;
    public event Action onBeginDrag;
    public event Action<Vector2> onDragWithVelocity;

    protected virtual void OnEnable()
    {
        pointerIsOver = false;
    }

    protected virtual void OnDisable() {}

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerIsOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerIsOver = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (dragging) return;

        if (onClick != null)
            onClick(Vector2.Scale(screenSizeScalar, eventData.position));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;

        if (onBeginDrag != null)
            onBeginDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragVelocity = Vector2.Scale(screenSizeScalar, eventData.delta / Time.unscaledDeltaTime);

        if (onDragWithVelocity != null)
            onDragWithVelocity(dragVelocity);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        dragVelocity = Vector2.zero;
    }

    Vector2 screenSizeScalar {
        get {
            if (_rootCanvasTransform == null)
                _rootCanvasTransform = GetComponentInParent<Canvas>().rootCanvas.transform as RectTransform;

            var rootCanvasSize = _rootCanvasTransform.rect.size;
            var scalar = new Vector2(
                rootCanvasSize.x / Screen.width,
                rootCanvasSize.y / Screen.height
            );

            return scalar;
        }
    }

    RectTransform _rootCanvasTransform;
}
