using System;
using System.Linq;
using EasyButtons;
using Ink.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(RectTransform))]
public class ItemSpawnLocationManager : MonoBehaviour {
    public ItemSpawnLocation[] itemSpawnLocations => GetComponentsInChildren<ItemSpawnLocation>();
    [Button]
    public ItemSpawnLocation CreateSpawnLocation(string itemFullName) {
        var spawnLocation = new GameObject($"{itemFullName}").AddComponent<ItemSpawnLocation>();
        spawnLocation.transform.SetParent(transform, false);
        
        var rectSize = new Vector2(Random.Range(80,300),Random.Range(80,300));
        spawnLocation.rectTransform.SetSizeWithCurrentAnchors(rectSize);
            
        var pivotOffset = rectSize * (spawnLocation.rectTransform.pivot);
        var localContainerRect = ((RectTransform)spawnLocation.transform.parent).rect;
        localContainerRect = new Rect(localContainerRect.x+pivotOffset.x, localContainerRect.y+pivotOffset.y, localContainerRect.width-rectSize.x, localContainerRect.height-rectSize.y);
            
        var randomLocalPos = new Vector2(Random.Range(localContainerRect.xMin, localContainerRect.xMax), Random.Range(localContainerRect.yMin, localContainerRect.yMax));
        spawnLocation.rectTransform.localPosition = randomLocalPos;
        return spawnLocation;
    }

    public ItemSpawnLocation FindForItem(ItemModel item) {
        var spawnLocation = itemSpawnLocations.FirstOrDefault(x => string.Equals(x.gameObject.name, item.inkListItemFullName, StringComparison.OrdinalIgnoreCase));
        if(spawnLocation == null) spawnLocation = itemSpawnLocations.FirstOrDefault(x => string.Equals(x.gameObject.name, item.inkListItemName, StringComparison.OrdinalIgnoreCase));
        return spawnLocation;
    }
}