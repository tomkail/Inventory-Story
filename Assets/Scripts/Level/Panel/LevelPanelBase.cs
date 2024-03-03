using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(SLayout))]
public abstract class LevelPanelBase : MonoBehaviour {
    public SLayout layout => GetComponent<SLayout>(); 
    public RectTransform rectTransform => GetComponent<RectTransform>();
    public Level level => GetComponentInParent<Level>();
    
    public ItemSpawnLocationManager itemSpawnLocationManager => GetComponentInChildren<ItemSpawnLocationManager>(true);
    
    public Image background;

    public RectTransform itemContainer { get; private set; }

    public virtual void OnLoadLevel() {
        CreateItemContainer();
    }
    
    void CreateItemContainer() {
        itemContainer = new GameObject("Items").AddComponent<RectTransform>();
        itemContainer.gameObject.AddComponent<SLayout>();
        itemContainer.SetParent(transform, false);
        itemContainer.SetAsLastSibling();
        itemContainer.anchorMin = new Vector2(0, 0);
        itemContainer.anchorMax = new Vector2(1,1);
        itemContainer.sizeDelta = new Vector2(0,0);
    }
}