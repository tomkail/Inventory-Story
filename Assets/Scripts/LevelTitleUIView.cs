using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelTitleUIView : MonoBehaviour {
    LevelController levelController => GetComponentInParent<LevelController>();
    
    SLayout layout => GetComponent<SLayout>();
    [SerializeField] SLayout titleText;
    [SerializeField] SLayout dateText;

    void Awake() {
        levelController.OnInit += Init;
    }

    void Init() {
        titleText.textMeshPro.text = levelController.levelState.titleText;
        titleText.textMeshPro.ApplyTightPreferredSize(titleText.width);
        dateText.textMeshPro.text = levelController.levelState.dateText;
        dateText.textMeshPro.ApplyTightPreferredSize(dateText.width);

        SLayoutUtils.AutoLayoutYWithSpacing(layout, new List<SLayout>() {titleText, dateText}, 20);
    }
}
