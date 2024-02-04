using UnityEngine;

public interface ISlot {
    public Rect slotTriggerScreenRect { get; }
    // Gets the screen rect the slot would have if it contained the given slottable
    public Rect GetSlotTriggerScreenRectWhenContainingItem(ISlottable slottable);
    public Rect GetScreenRectSlotForSlottable(ISlottable slottable);
    public void OnSlottableHoverStart(ISlottable slottable);
    public void OnSlottableHoverEnd(ISlottable slottable);
    public void OnSlottableSlottedStart(ISlottable slottable);
    public void OnSlottableSlottedEnd(ISlottable slottable);
}
