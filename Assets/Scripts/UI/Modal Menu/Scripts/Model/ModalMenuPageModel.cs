using System;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ModalMenuPageModel {
    [System.Serializable]
    public struct ModalMenuPagePositionParams {
        public Vector2 pivotPosition;
        public ArrowDirection arrowDirection;
        public enum ArrowDirection {
            Top,
            Bottom,
            Left,
            Right,
            BottomRight
        }
    }

    public ModalMenuPagePositionParams positionParams;


    public bool showBackground = true;
    public bool canCloseByClickingBackground = true;
    public List<ModalMenuItemModelBase> items = new List<ModalMenuItemModelBase>();
    public Action OnClose;
}
