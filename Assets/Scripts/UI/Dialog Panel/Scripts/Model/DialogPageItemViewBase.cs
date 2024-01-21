using UnityEngine;

[RequireComponent(typeof(SLayout))]
[ExecuteAlways]
public abstract class DialogPageItemViewBase : MonoBehaviour {
    public DialogPanelPage page => GetComponentInParent<DialogPanelPage>();
    public SLayout layout => GetComponent<SLayout>();
    public bool layoutDirty { get; private set; }
    
    void Update () {
        LayoutInternal();
    }

    public void SetLayoutDirty() {
        page.SetLayoutDirty();
    }

    public void Layout() {
        layoutDirty = false;
        LayoutInternal();
    }
    protected abstract void LayoutInternal ();
}