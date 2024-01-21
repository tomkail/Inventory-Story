using System;
using UnityEngine;

[System.Serializable]
public class ModalMenuButtonModel : ModalMenuItemModelBase {
    public string textStr;
    public Sprite iconSprite;
    public Action onClick;
    
    public ModalMenuButtonModel (string textStr, Sprite iconSprite, Action onClick) {
        this.textStr = textStr;
        this.iconSprite = iconSprite;
        this.onClick = onClick;
    }
}