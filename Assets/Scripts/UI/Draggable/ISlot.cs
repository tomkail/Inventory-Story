using UnityEngine;

public interface ISlot {
    public Rect slotTriggerScreenRect { get; }
    // Gets the screen rect that triggers a slot for a given slottable. This might be different from the final size of the slot once the object is slotted.
    public Rect GetSlotTriggerScreenRectWhenContainingItem(ISlottable slottable);
    // Gets the screen rect the slot would have if it contained the given slottable.
    public Rect GetScreenRectSlotForSlottable(ISlottable slottable);
    public void OnSlottableHoverStart(ISlottable slottable);
    public void OnSlottableHoverEnd(ISlottable slottable);
    public void OnSlottableSlottedStart(ISlottable slottable);
    public void OnSlottableSlottedEnd(ISlottable slottable);
}
