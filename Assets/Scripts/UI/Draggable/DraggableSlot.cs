using UnityEngine;

[RequireComponent(typeof(SLayout))]
public class DraggableSlot : MonoBehaviour {
	public RectTransform rectTransform {
		get {
			return (RectTransform)transform;
		}
	}
	public SLayout layout => GetComponent<SLayout>();
    public Draggable slottedDraggable;
}