using System;
using UnityEngine;

[RequireComponent(typeof(SLayout))]
public class ItemView : MonoBehaviour {
    public Level level => GetComponentInParent<Level>();
    public SLayout layout => GetComponent<SLayout>();
    public RectTransform rectTransform => GetComponent<RectTransform>();
    
    [SerializeReference] public ItemModel itemModel;
    [SerializeField, Disable] ItemSpawnLocation itemSpawnLocation;
    [SerializeField, Disable] LevelSubPanel itemSubPanel;
    public ItemBoxView boxView => GetComponentInChildren<ItemBoxView>(true);
    
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
        
        if(itemModel.isZoomable)
            SetSubPanel(level.GetSubPanelOwnedByItem(itemModel, itemSpawnLocation));
    }

    public void SetSubPanel(LevelSubPanel levelSubPanel) {
        this.itemSubPanel = levelSubPanel;
    }

    void OnDestroy() {
        UnsubscribeFromItem();
    }

    void SubscribeToItem() {
        itemModel.onChangeState += SetState;
        itemModel.onChangeIsZoomable += OnChangeIsZoomable;
    }

    void OnChangeIsZoomable(bool isZoomable) {
        if (itemModel.isZoomable) {
            SetSubPanel(level.GetSubPanelOwnedByItem(itemModel, itemSpawnLocation));
        } else {
            Debug.LogWarning("ItemView: OnChangeIsZoomable: Hiding sub panel. This hasn't been tested.");
            SetSubPanel(null);
        }
    }

    void UnsubscribeFromItem() {
        if (itemModel == null) return;
        itemModel.onChangeState -= SetState;
        itemModel.onChangeIsZoomable -= OnChangeIsZoomable;
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
        } else if (itemModel.state == ItemModel.State.Searchable) {
            boxView.gameObject.SetActive(false);
        } else if (itemModel.state == ItemModel.State.Showing) {
            boxView.gameObject.SetActive(true);
        }
    }
    public void SetState(ItemModel.State state) {
        Layout();
    }

    public void StartZoomingOnContainer() {
        level.panelManager.ShowPanel(itemSubPanel);
    }
    public void EndZoomingOnContainer() {
        if (itemSubPanel.showProgress < 0.9f && level.panelManager.currentPanel == itemSubPanel) {
            level.panelManager.HidePanel(itemSubPanel);
        }
    }
}