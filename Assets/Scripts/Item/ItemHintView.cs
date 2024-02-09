using Ink.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SLayout))]
public class ItemHintView : MonoBehaviour, IPointerClickHandler {
    public ItemView itemView => GetComponentInParent<ItemView>();
    public SLayout layout => GetComponent<SLayout>();
    public InkListItem inkListItem;
    

    public void OnPointerClick(PointerEventData eventData) {
        itemView.SetState(ItemView.State.Showing);
    }
}