using UnityEngine;

[RequireComponent(typeof(SLayout))]
public class ItemContainerView : MonoBehaviour {
    public SLayout layout => GetComponent<SLayout>();
}