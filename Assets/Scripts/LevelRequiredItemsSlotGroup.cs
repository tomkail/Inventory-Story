using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using Unity.VisualScripting;
using UnityEngine;

public class LevelRequiredItemsSlotGroup : MonoBehaviour {
    public SLayout layout => GetComponent<SLayout>();
    public DraggableGroup draggableGroup => GetComponent<DraggableGroup>();
    public IEnumerable<ItemView> slottedItems {
        get {
            foreach (var slot in slots) {
                if (slot.slot.slottedDraggable == null) continue;
                var itemView = slot.slot.slottedDraggable.GetComponent<ItemView>();
                if (itemView != null) yield return itemView;
            }
        }
    }

    public LevelRequiredItemsSlot slotPrefab;
    public List<LevelRequiredItemsSlot> slots = new List<LevelRequiredItemsSlot>();

    public void Init(InkList inkList) {
        Clear();
        foreach(var item in inkList.Keys) {
            var slot = Instantiate(slotPrefab, transform);
            slots.Add(slot);
            draggableGroup.slots.Add(slot.slot);
        }

        Layout();
    }

    void Layout() {
        layout.width = SLayoutUtils.AutoLayoutWithSpacing(layout, slots.Select(x => x.layout).ToArray(), SLayoutUtils.Axis.X, 10, false, 0, 0);
        layout.centerX = layout.parentRect.width / 2;
    }

    public void Clear() {
        for (var index = slots.Count - 1; index >= 0; index--) {
            var slot = slots[index];
            draggableGroup.slots.Remove(slot.slot);
            Destroy(slot.gameObject);
            slots.Remove(slot);
        }
    }
}
