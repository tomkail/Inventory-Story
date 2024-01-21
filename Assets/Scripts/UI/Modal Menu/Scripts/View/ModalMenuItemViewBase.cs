using UnityEngine;

[RequireComponent(typeof(SLayout))]
[ExecuteAlways]
public abstract class ModalMenuItemViewBase : MonoBehaviour {
    public SLayout layout => GetComponent<SLayout>();
    public RectOffset margin;
    void Update () {
        Layout();
    }

    public abstract void Layout ();
}
