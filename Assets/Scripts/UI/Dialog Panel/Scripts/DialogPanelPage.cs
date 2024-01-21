using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EasyButtons;

[RequireComponent(typeof(SLayout))]
public class DialogPanelPage : MonoBehaviour {
    public DialogPanelModel model;
    public List<DialogPageItemViewBase> items = new List<DialogPageItemViewBase>();
    
    public SLayout layout => GetComponent<SLayout>();
    
    public Button closeButton;
    // public List<SLayout> items = new List<SLayout>();
    
    // public float offset;
    float selectedViewLayoutOffset;
    float localSpaceSelectedLayoutOffsetVelocity;
    
    const float margin = 24f;
    const float spacing = 14f;

    public bool layoutDirty { get; private set; }

    void Update () {
        layout.centerY = layout.parentRect.height * 0.5f;
        layout.centerX = layout.parentRect.width * 0.5f;
        
        if(Application.isPlaying) {
			selectedViewLayoutOffset = Mathf.SmoothDamp(selectedViewLayoutOffset, VirtualKeyboardManager.GetOffsetToShowSelectedInputField(layout), ref localSpaceSelectedLayoutOffsetVelocity, 0.1f);
			layout.y += selectedViewLayoutOffset * VirtualKeyboardManager.Instance.showAmount;
		}

        if (layoutDirty) {
	        layout.Animate(Styling.StandardAnimationTime, Layout);
        }
    }

    [Button("Layout")]
    public void Layout() {
	    layoutDirty = false;
	    var y = margin;
	    for (int i = 0; i < items.Count; i++) {
		    var view = items[i];
		    view.layout.x = margin;
		    view.layout.width = layout.width - margin*2;
		    view.layout.y = y;

		    view.Layout();
		    
		    // Todo - remove this - should occur inthe layout for the view instead.
		    var textComponent = view.GetComponent<TextMeshProUGUI>();
		    if(textComponent != null) {
			    var textLayout = textComponent.GetComponent<SLayout>();
			    if(textLayout != null) {
				    textLayout.textMeshPro.ForceMeshUpdate(true, true);
				    var height = textLayout.textMeshPro.GetPreferredValues().y;
				    textLayout.height = height;
			    }
		    }
            
		    y += view.layout.height;
		    if(i < items.Count - 1) y += spacing;
	    }
	    y += margin;
	    layout.height = y;

	    layout.centerY = layout.parentRect.height * 0.5f;
	    layout.centerX = layout.parentRect.width * 0.5f;
    }

    public void SetLayoutDirty() {
	    layoutDirty = true;
    }
}