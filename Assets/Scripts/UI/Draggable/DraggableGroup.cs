using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableGroup : MonoBehaviour
{
    public bool forceIntoSlotByDistance;
    public List<Draggable> draggables;

    [System.Serializable]
    public class HeldDraggable {
        public Draggable draggable;
        public DraggableSlot lastSlot;
        public DraggableSlot hoveredSlot;

        public HeldDraggable (Draggable draggable, DraggableSlot lastSlot) {
            this.draggable = draggable;
            this.lastSlot = lastSlot;
        }
    }

    public List<HeldDraggable> heldDraggables;
    public List<DraggableSlot> slots;

    public System.Action<Draggable, DraggableSlot> OnSlottedDraggable;
    public System.Action<Draggable, DraggableSlot> OnUnslottedDraggable;
    
    public void Clear () {
        draggables.Clear();
        slots.Clear();
        heldDraggables.Clear();
    }
    
    void OnDraggedDraggable (Draggable draggable, PointerEventData eventData) {
        TryDoSlot(draggable);
    }

    void LateUpdate () {
        foreach(var draggable in draggables) {
            if(draggable.dragging) {
                if(!IsHeld(draggable)) {
                    heldDraggables.Add(new HeldDraggable(draggable, FindSlotContainingDraggable(draggable)));
                    draggable.OnDragged += OnDraggedDraggable;
                    TryDoSlot(draggable);
                }
            } else {
                var heldDraggable = GetHeldDraggable(draggable);
                if(heldDraggable != null) {
                    heldDraggables.Remove(heldDraggable);
                    draggable.OnDragged -= OnDraggedDraggable;
                }
            }
        }

        RefreshTargetPositionsFromSlots();
    }

    HeldDraggable GetHeldDraggable (Draggable draggable) {
        foreach(var heldDraggable in heldDraggables)
            if(heldDraggable.draggable == draggable) 
                return heldDraggable;
        return null;
    }
    
    bool IsHeld (Draggable draggable) {
        foreach(var heldDraggable in heldDraggables)
            if(heldDraggable.draggable == draggable) 
                return true;
        return false;
    }

    void TryDoSlot (Draggable draggable) {
        if(draggable.lastPointerEventData == null) return;

        var heldDraggable = GetHeldDraggable(draggable);
        heldDraggable.hoveredSlot = null;
        
        var camera = ((RectTransform)transform).GetComponentInParent<Canvas>().rootCanvas.worldCamera;
        foreach(var slot in slots) {
            if(RectTransformUtility.RectangleContainsScreenPoint(slot.rectTransform, draggable.lastPointerEventData.position, camera)) {
                heldDraggable.hoveredSlot = slot;
                if(forceIntoSlotByDistance)
                    Slot(draggable, slot);
            }
        }
        
       
        
        if(forceIntoSlotByDistance) {
            var validSlots = slots.Where(s => s.slottedDraggable == null || (heldDraggable != null && (heldDraggable.lastSlot == s || heldDraggable.hoveredSlot == s)));
            var closestSlot = FindClosestDraggableSlot(draggable.lastPointerEventData.enterEventCamera, draggable.lastPointerEventData.position, validSlots);
            if(closestSlot != null) {
                Slot(draggable, closestSlot);
            }
        } else {
            if(heldDraggable.hoveredSlot != null) {
                Slot(draggable, heldDraggable.hoveredSlot);
            } else {
                var currentSlot = FindSlotContainingDraggable(draggable);
                if(currentSlot != null)
                    UnslotInternal(draggable, currentSlot);
            }
        }

        // if(hoveredSlot)
            RefreshTargetPositionFromSlot(draggable);
        // var currentSlot = FindSlotContainingDraggable(draggable);
        // if(currentSlot != null)
    }
    public void Slot (Draggable draggable, DraggableSlot slot) {
        if(slot.slottedDraggable == draggable) return;
        
        var currentSlot = FindSlotContainingDraggable(draggable);
        // If there's something in this slot already try to fit it somewhere else
        var otherDraggable = slot.slottedDraggable;
        if(otherDraggable != null && otherDraggable != draggable) {
            var validSlots = slots.Where(s => s.slottedDraggable == null || s == currentSlot);
            var closestValidSlot = FindClosestDraggableSlot(draggable.lastPointerEventData.enterEventCamera, draggable.lastPointerEventData.position, validSlots);
            if(closestValidSlot == null) return;
            SlotInternal(otherDraggable, closestValidSlot);
        }

        SlotInternal(draggable, slot);
    }

    public void Unslot(DraggableSlot slot) {
        if(slot.slottedDraggable == null) return;
        UnslotInternal(slot.slottedDraggable, slot);
    }

    void SlotInternal (Draggable draggable, DraggableSlot slot) {
        var currentSlot = FindSlotContainingDraggable(draggable);
        if(currentSlot == slot) return;
        if(currentSlot != null) UnslotInternal(draggable, currentSlot);
        slot.slottedDraggable = draggable;
        

        var heldDraggable = GetHeldDraggable(draggable);
        if(heldDraggable != null) heldDraggable.lastSlot = slot;
        // Debug.Log("SLOT "+draggable.gameObject.name+" in "+slot.gameObject.name);
        RefreshTargetPositionFromSlot(draggable);
        
        if(OnSlottedDraggable != null) OnSlottedDraggable(draggable, slot);
    }

    void UnslotInternal (Draggable draggable, DraggableSlot slot) {
        slot.slottedDraggable = null;

        if(OnUnslottedDraggable != null) OnUnslottedDraggable(draggable, slot);
        // Debug.Log("UNSLOT "+draggable.gameObject.name+" from "+slot.gameObject.name);
    }
    
    

    public void RefreshTargetPositionsFromSlots() {
        foreach(var slot in slots) {
            if(slot.slottedDraggable != null) {
                RefreshTargetPositionFromSlot(slot.slottedDraggable);
            }
        }
    }
    public void RefreshTargetPositionFromSlot (Draggable draggable) {
        var slot = FindSlotContainingDraggable(draggable);
        if(slot == null) return;
        draggable.dragTargetPosition = GetPositionInSlotForDraggable(draggable, slot);
    }

    DraggableSlot FindSlotContainingDraggable (Draggable draggable) {
        return slots.FirstOrDefault(s => s.slottedDraggable == draggable);
    } 
    DraggableSlot FindClosestDraggableSlot (Camera camera, Vector2 screenPos, IEnumerable<DraggableSlot> slots) {
	    return slots.Best(slot => {
            var slotScreenPoint = RectTransformUtility.WorldToScreenPoint(camera, slot.transform.position);
            return Vector2.Distance(screenPos, slotScreenPoint);
        }, (other, currentBest) => other < currentBest, Mathf.Infinity, null);
    }

    Vector2 GetPositionInSlotForDraggable (Draggable draggable, DraggableSlot slot) {
        Camera camera = 
            (draggable.lastPointerEventData == null ? null : draggable.lastPointerEventData.pressEventCamera) ??
            gameObject.GetComponentInParent<Canvas>().worldCamera ??
            null;
        
        var slotTargetPoint = slot.rectTransform.TransformPoint(slot.rectTransform.rect.center); //slot.transform.position;
        var slotScreenPoint = RectTransformUtility.WorldToScreenPoint(camera, slotTargetPoint);
        Vector2 slotTargetPosition;
        var container = (RectTransform)draggable.rectTransform.parent;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(container, slotScreenPoint, camera, out slotTargetPosition);
        
        var containerOffset = (container.pivot - Vector2.one * 0.5f) * container.rect.size;
        slotTargetPosition += containerOffset;

        var anchorOffset = (draggable.rectTransform.AnchorCenter() - Vector2.one * 0.5f) * container.rect.size;
        slotTargetPosition -= anchorOffset;

        var draggableOffset = (draggable.rectTransform.pivot - Vector2.one * 0.5f) * draggable.rectTransform.rect.size;
        slotTargetPosition += draggableOffset;
        
        return slotTargetPosition;
    }
}