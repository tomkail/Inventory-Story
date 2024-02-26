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
        foreach (var subPanel in subPanels) subPanel.HideImmediate();
        currentPanelHierarchy = new List<LevelPanelBase>();
        currentPanelHierarchy.Add(rootPanel);

        RefreshPanelSeparationLayer();
    }

    public void ShowPanel(LevelSubPanel itemSubPanel) {
        itemSubPanel.parentPanel = currentPanel;
        
        currentPanelHierarchy.Add(itemSubPanel);
        
        itemSubPanel.transform.SetAsLastSibling();
        itemSubPanel.Show();

        panelSeparationLayer.Animate(0.2f, RefreshPanelSeparationLayer);
    }

    public void HidePanel(LevelSubPanel itemSubPanel) {
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
    public LevelSubPanel CreateSubPanel(string itemFullName) {
        var spawnLocation = new GameObject($"{itemFullName}").AddComponent<LevelSubPanel>();
        spawnLocation.transform.SetParent(transform, false);
        
        var rectSize = new Vector2(Random.Range(320,460),Random.Range(320,460));
        spawnLocation.rectTransform.SetSizeWithCurrentAnchors(rectSize);
        
        var pivotOffset = rectSize * (spawnLocation.rectTransform.pivot);
        var localContainerRect = ((RectTransform)spawnLocation.transform.parent).rect;
        localContainerRect = new Rect(localContainerRect.x+pivotOffset.x, localContainerRect.y+pivotOffset.y, localContainerRect.width-rectSize.x, localContainerRect.height-rectSize.y);
        
        var randomLocalPos = new Vector2(Random.Range(localContainerRect.xMin, localContainerRect.xMax), Random.Range(localContainerRect.yMin, localContainerRect.yMax));
        spawnLocation.rectTransform.localPosition = randomLocalPos;
        return spawnLocation;
    }

    public LevelSubPanel FindSubPanelForItem(ItemModel item) {
        var spawnLocation = subPanels.FirstOrDefault(x => string.Equals(x.gameObject.name, item.inkListItemFullName, StringComparison.OrdinalIgnoreCase));
        if(spawnLocation == null) spawnLocation = subPanels.FirstOrDefault(x => string.Equals(x.gameObject.name, item.inkListItemName, StringComparison.OrdinalIgnoreCase));
        return spawnLocation;
    }
}