using System;
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
        dateText.textMeshPro.text = levelController.levelState.dateText;
    }
}
