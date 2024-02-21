using System;
using System.Collections.Generic;
using EasyButtons;
using UnityEngine;

public class LevelTitleUIView : MonoBehaviour {
    Level Level => GetComponentInParent<Level>();
    
    SLayout layout => GetComponent<SLayout>();
    [SerializeField] SLayout titleText;
    [SerializeField] SLayout dateText;

    void Awake() {
        Level.OnInit += Init;
    }

    void Init() {
        titleText.textMeshPro.text = Level.levelState.titleText;
        dateText.textMeshPro.text = Level.levelState.dateText;
        
        Layout();
    }

    [Button]
    void Layout() {
        titleText.textMeshPro.ApplyTightPreferredSize(titleText.width);
        dateText.textMeshPro.ApplyTightPreferredSize(dateText.width);
        SLayoutUtils.AutoLayoutYWithSpacing(layout, new List<SLayout>() {titleText, dateText}, 20);
    }
}
