using UnityEngine;

public class LevelRequiredItemsSlot : MonoBehaviour, ISlot {
    public SLayout layout => GetComponent<SLayout>();
    // public DraggableSlot slot => GetComponent<DraggableSlot>();
    public ISlottable hoveredSlottable;
    public ISlottable heldSlottable;

    void Awake() {
        Layout();
    }
    public Rect slotTriggerScreenRect => layout.GetScreenRect();
    public Rect GetSlotTriggerScreenRectWhenContainingItem(ISlottable slottable) {
        if (slottable is Component component && component.transform is RectTransform rectTransform) {
            var centerOffset = (layout.rectTransform.rect.size-rectTransform.rect.size)*0.5f;
            var localRect = new Rect(centerOffset.x,centerOffset.y,rectTransform.rect.width,rectTransform.rect.height);
            return layout.LocalToScreenRect(localRect);
        }
        return layout.GetScreenRect();
    }

    public Rect GetScreenRectSlotForSlottable(ISlottable slottable) {
        if (slottable is Component component && component.transform is RectTransform rectTransform) {
            var centerOffset = (layout.rectTransform.rect.size-rectTransform.rect.size)*0.5f;
            var localRect = new Rect(centerOffset.x,centerOffset.y,rectTransform.rect.width,rectTransform.rect.height);
            return layout.LocalToScreenRect(localRect);
        }
        return layout.GetScreenRect();
    }

    public void OnSlottableHoverStart(ISlottable slottable) {
        hoveredSlottable = slottable;
        layout.Animate(Styling.StandardAnimationTime, EasingFunction.Ease.EaseInOutQuad, Layout);
    }

    public void OnSlottableHoverEnd(ISlottable slottable) {
        hoveredSlottable = null;
        layout.Animate(Styling.StandardAnimationTime, EasingFunction.Ease.EaseInOutQuad, Layout);
    }
    
    public void OnSlottableSlottedStart(ISlottable slottable) {
        heldSlottable = slottable;
        layout.Animate(Styling.StandardAnimationTime, EasingFunction.Ease.EaseInOutQuad, Layout);
        // GameController.Instance.sceneController.currentLevelController.OnSlotItem(slottable as ItemView);
    }
    
    public void OnSlottableSlottedEnd(ISlottable slottable) {
        heldSlottable = null;
        layout.Animate(Styling.StandardAnimationTime, EasingFunction.Ease.EaseInOutQuad, Layout);
    }

    void Layout() {
        if (hoveredSlottable is ItemView hoveredItemView) {
            layout.rect = layout.ScreenToSLayoutRect(GetSlotTriggerScreenRectWhenContainingItem(hoveredItemView));
        } else if (heldSlottable is ItemView heldItemView) {
            layout.rect = layout.ScreenToSLayoutRect(GetSlotTriggerScreenRectWhenContainingItem(heldItemView));
        } else {
            layout.size = new Vector2(80,80);
        }
    }
}