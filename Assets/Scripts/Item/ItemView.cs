using System;
using UnityEngine;

[RequireComponent(typeof(SLayout))]
public class ItemView : MonoBehaviour {
    public Level Level => GetComponentInParent<Level>();
    public SLayout layout => GetComponent<SLayout>();
    
    public ItemModel itemModel;
    public ItemSpawnLocation itemSpawnLocation;
    public ItemHintView hintView;
    public ItemBoxView boxView;
    
    public void Init(ItemModel itemModel, ItemSpawnLocation itemSpawnLocation) {
        this.itemModel = itemModel;
        SubscribeToItem();
        this.itemSpawnLocation = itemSpawnLocation;
        
        boxView.Init(this);
        
        
        // labelView = CreateItemLabelView();
        
        gameObject.name = $"Item View: {this.itemModel.inkListItemName}";
        
        var screenRect = itemSpawnLocation.rectTransform.GetScreenRect();
        layout.rect = layout.ScreenToSLayoutRect(screenRect);

        SetState(itemModel.state);
        Layout();
    }

    void OnDestroy() {
        UnsubscribeFromItem();
    }

    void SubscribeToItem() {
        itemModel.onChangeState += SetState;
    }

    void UnsubscribeFromItem() {
        itemModel.onChangeState -= SetState;
    }


    // public ItemLabelView CreateItemLabelView() {
    //     var itemLabel = Instantiate(PrefabDatabase.Instance.itemLabelViewPrefab, transform);
    //     itemLabel.itemSpawnLocation = itemSpawnLocation;
    //     var localSpawnPoint = (Vector2)itemLabel.layout.parentRectTransform.InverseTransformPoint(itemSpawnLocation.rectTransform.TransformPoint(itemSpawnLocation.rectTransform.rect.center)) + RandomX.onUnitCircle * Random.Range(100, 200);
    //     localSpawnPoint = itemLabel.draggable.dragTargetPosition = RectTransformX.GetClampedLocalPositionInsideScreenRect(itemLabel.draggable.rectTransform, localSpawnPoint, itemLabel.draggable.viewRect.GetScreenRect(), layout.rootCanvas.worldCamera);
    //     itemLabel.SetWorldPosition(itemLabel.layout.parentRectTransform.TransformPoint(localSpawnPoint));
    //     return itemLabel;
    // }
    
    void Layout() {
        if (itemModel.state == ItemModel.State.Hidden) {
            boxView.gameObject.SetActive(false);
            hintView.gameObject.SetActive(false);
        } else if (itemModel.state == ItemModel.State.Searchable) {
            boxView.gameObject.SetActive(false);
            // hintView.gameObject.SetActive(true);
        } else if (itemModel.state == ItemModel.State.Showing) {
            boxView.gameObject.SetActive(true);
            hintView.gameObject.SetActive(false);
        }
    }
    public void SetState(ItemModel.State state) {
        Layout();
    }
}