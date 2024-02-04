using UnityEngine;

[RequireComponent(typeof(SLayout))]
public class DraggableSlot : MonoBehaviour {
	public RectTransform rectTransform => (RectTransform)transform;
	public SLayout layout => GetComponent<SLayout>();
    public Draggable slottedDraggable;
}