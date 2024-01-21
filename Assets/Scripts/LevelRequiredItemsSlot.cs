using UnityEngine;

public class LevelRequiredItemsSlot : MonoBehaviour {
    public SLayout layout => GetComponent<SLayout>();
    public DraggableSlot slot => GetComponent<DraggableSlot>();
}