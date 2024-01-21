using System.Collections.Generic;
using UnityEngine;

public class DraggableGroupTester : MonoBehaviour {
    public DraggableGroup draggableGroup;
    public List<Draggable> draggables;
    public List<DraggableSlot> slots;

    void OnEnable () {
        for(int i = 0; i < slots.Count; i++) {
            draggableGroup.slots.Add(slots[i]);
        }
        for(int i = 0; i < draggables.Count; i++) {
            draggableGroup.draggables.Add(draggables[i]);
        }
        for(int i = 0; i < draggableGroup.draggables.Count; i++) {
            if(i < draggableGroup.slots.Count)
                draggableGroup.Slot(draggableGroup.draggables[i], draggableGroup.slots[i]);
        }
    }
}