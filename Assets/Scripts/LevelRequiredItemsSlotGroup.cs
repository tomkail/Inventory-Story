using System.Collections.Generic;
using EasyButtons;
using UnityEngine;

public class LevelRequiredItemsSlotGroup : MonoBehaviour {
    public SLayout layout => GetComponent<SLayout>();
    // public DraggableGroup draggableGroup => GetComponent<DraggableGroup>();
    public IEnumerable<ItemLabelView> slottedItems {
        get {
            foreach (var slot in slots) {
                if(slot.heldSlottable != null && slot.heldSlottable is ItemLabelView heldItem)
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
    public void Layout() {
        // SLayoutUtils.AutoLayoutWithSpacing(layout, slots.Select(x => x.layout).ToArray(), SLayoutUtils.Axis.X, 20, false, 0, 0, 0.5f);
        var layoutItems = new List<LayoutItemParams>();
        var spacing = layout.LocalToScreenVector(new Vector2(20, 0)).x;
        foreach (var slot in slots) {
            float screenWidth = 0;
            if (slot.hoveredSlottable is ItemLabelView hoveredItemView) {
                screenWidth = slot.GetSlotTriggerScreenRectWhenContainingItem(hoveredItemView).width;
            } else if (slot.heldSlottable is ItemLabelView heldItemView) {
                screenWidth = slot.GetSlotTriggerScreenRectWhenContainingItem(heldItemView).width;
            } else {
                screenWidth = layout.LocalToScreenVector(new Vector2(80, 80)).x;
            }
            // slot.GetScreenRectSlotForSlottable(slot.hoveredSlottable)
            layoutItems.Add(LayoutItemParams.Fixed(screenWidth));
        }
        var ranges = LayoutUtils.GetLayoutRanges(Screen.width, layoutItems, spacing, 0.5f);

        for (var index = 0; index < slots.Count; index++) {
            var slot = slots[index];
            var targetRect = slot.layout.ScreenToSLayoutRect(Rect.MinMaxRect(ranges[index].x, 0, ranges[index].y, 0));
            slot.layout.x = targetRect.x;
            slot.layout.width = targetRect.width;
            slot.layout.centerY = layout.height * 0.5f;
        }
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
