using System;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class DialogPanelModel {
    public bool hasCloseButton = false;
    public bool canCloseByClickingBackground = false;
    public List<DialogPageItemModelBase> items = new List<DialogPageItemModelBase>();
    
    Action onClose;
}