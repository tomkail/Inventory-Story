using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using Ink.Runtime;
using Unity.VisualScripting;
using UnityEngine;

public class LevelRequiredItemsSlotGroup : MonoBehaviour {
    public SLayout layout => GetComponent<SLayout>();
    // public DraggableGroup draggableGroup => GetComponent<DraggableGroup>();
    public IEnumerable<ItemView> slottedItems {
        get {
            foreach (var slot in slots) {
                if(slot.heldSlottable != null && slot.heldSlottable is ItemView heldItem)
                    yield return heldItem;
            }
        }
    }

    public LevelRequiredItemsSlot slotPrefab;
    public List<LevelRequiredItemsSlot> slots = new List<LevelRequiredItemsSlot>();

    public void Init(int newSlotCount) {
        Clear();
        for (var i = 0; i < newSlotCount ; i ++) {
            var slot = Instantiate(slotPrefab, transform);
            slots.Add(slot);
            // draggableGroup.slots.Add(slot.heldSlottable);
        }

        Layout();
    }

    [Button]
    void Layout() {
        SLayoutUtils.AutoLayoutWithSpacing(layout, slots.Select(x => x.layout).ToArray(), SLayoutUtils.Axis.X, 20, false, 0, 0, 0.5f);
    }

    public void Clear() {
        for (var index = slots.Count - 1; index >= 0; index--) {
            var slot = slots[index];
            // draggableGroup.slots.Remove(slot.heldSlottable);
            Destroy(slot.gameObject);
            slots.Remove(slot);
        }
    }
}
