using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(RectTransform))]
public class LevelPanelManager : MonoBehaviour {
    public SLayout panelSeparationLayer;
    public LevelPanelBase rootPanel => panels.First();
    public LevelPanelBase[] panels => GetComponentsInChildren<LevelPanelBase>(true);
    public LevelSubPanel[] subPanels => GetComponentsInChildren<LevelSubPanel>(true);
    
    public List<LevelPanelBase> currentPanelHierarchy;
    public LevelPanelBase currentPanel => currentPanelHierarchy.Last();

    void Awake() {
        panelSeparationLayer.GetComponent<Button>().onClick.AddListener(() => {
            if(currentPanel is LevelSubPanel subPanel) HidePanel(subPanel);
        });
    }

    public void Init() {
        foreach (var panel in panels) {
            panel.OnLoadLevel();
            // levelState.itemStates
            // panel.gameObject.name
        }
        currentPanelHierarchy = new List<LevelPanelBase>();
        currentPanelHierarchy.Add(rootPanel);

        RefreshPanelSeparationLayer();
    }

    public void ShowPanel(LevelSubPanel itemSubPanel) {
        if(itemSubPanel.shownOrShowing) return;
        itemSubPanel.parentPanel = currentPanel;
        
        currentPanelHierarchy.Add(itemSubPanel);
        
        itemSubPanel.transform.SetAsLastSibling();
        itemSubPanel.Show();

        panelSeparationLayer.Animate(0.2f, RefreshPanelSeparationLayer);
    }

    public void HidePanel(LevelSubPanel itemSubPanel) {
        if(itemSubPanel.hiddenOrHiding) return;
        var index = currentPanelHierarchy.IndexOf(itemSubPanel);
        while (currentPanelHierarchy.Count >= index && currentPanelHierarchy.Count > 1) {
            var removedItem = (LevelSubPanel)currentPanelHierarchy[^1];
            currentPanelHierarchy.RemoveAt(currentPanelHierarchy.Count-1);
            
            removedItem.Hide();
            
            panelSeparationLayer.Animate(0.2f, RefreshPanelSeparationLayer);
        }
    }

    void RefreshPanelSeparationLayer() {
        panelSeparationLayer.transform.SetSiblingIndex(currentPanel.transform.GetSiblingIndex()-1);
        panelSeparationLayer.canvasGroup.blocksRaycasts = panelSeparationLayer.canvasGroup.interactable = currentPanelHierarchy.Count > 1;
        panelSeparationLayer.groupAlpha = currentPanelHierarchy.Count > 1 ? 0.7f : 0;
    }
    
    [Button]
    // This creates a new sub panel for the item and returns it
    public LevelSubPanel CreateSubPanel(string itemFullName) {
        var levelSubPanel = Instantiate(PrefabDatabase.Instance.levelSubPanel, transform, false);
        levelSubPanel.name = $"{itemFullName}";
        
        var rectSize = new Vector2(Random.Range(320,460),Random.Range(320,460));
        levelSubPanel.rectTransform.SetSizeWithCurrentAnchors(rectSize);
        
        var pivotOffset = levelSubPanel.rectTransform.rect.size * (levelSubPanel.rectTransform.pivot);
        var localContainerRect = ((RectTransform)levelSubPanel.transform.parent).rect;
        localContainerRect = new Rect(localContainerRect.x+pivotOffset.x, localContainerRect.y+pivotOffset.y, localContainerRect.width-levelSubPanel.rectTransform.rect.size.x, localContainerRect.height-levelSubPanel.rectTransform.rect.size.y);
        
        var randomLocalPos = new Vector2(Random.Range(localContainerRect.xMin, localContainerRect.xMax), Random.Range(localContainerRect.yMin, localContainerRect.yMax));
        levelSubPanel.rectTransform.localPosition = randomLocalPos;
        
        return levelSubPanel;
    }

    public LevelSubPanel CreateTemporarySubPanel(string itemInkListItemFullName, ItemSpawnLocation itemSpawnLocation) {
        var levelSubPanel = CreateSubPanel(itemInkListItemFullName);
        
        // Sets the panel to the position of the item (clamped to the parent rect)
        var pivotOffset = levelSubPanel.rectTransform.rect.size * (levelSubPanel.rectTransform.pivot);
        var localContainerRect = ((RectTransform)levelSubPanel.transform.parent).rect;
        localContainerRect = new Rect(localContainerRect.x+pivotOffset.x, localContainerRect.y+pivotOffset.y, localContainerRect.width-levelSubPanel.rectTransform.rect.size.x, localContainerRect.height-levelSubPanel.rectTransform.rect.size.y);

        var worldSpawnPoint = itemSpawnLocation.rectTransform.TransformPoint(itemSpawnLocation.rectTransform.rect.center);
        var localSpawnPoint = levelSubPanel.transform.parent.InverseTransformPoint(worldSpawnPoint);
        var randomLocalPos = localContainerRect.ClosestPoint(localSpawnPoint);
        levelSubPanel.rectTransform.localPosition = randomLocalPos;
        
        levelSubPanel.OnLoadLevel();
        return levelSubPanel;
    }

    public LevelSubPanel FindSubPanelForItem(ItemModel item) {
        var spawnLocation = subPanels.FirstOrDefault(x => string.Equals(x.gameObject.name, item.inkListItemFullName, StringComparison.OrdinalIgnoreCase));
        if(spawnLocation == null) spawnLocation = subPanels.FirstOrDefault(x => string.Equals(x.gameObject.name, item.inkListItemName, StringComparison.OrdinalIgnoreCase));
        return spawnLocation;
    }
    
    

    public LevelSubPanel GetFirstExpandableSubPanelAtScreenPosition(Vector2 screenPoint) {
        return subPanels.FirstOrDefault(x => RectTransformUtility.RectangleContainsScreenPoint(x.layout.rectTransform, screenPoint, x.layout.canvas.rootCanvas.worldCamera));
    }
}