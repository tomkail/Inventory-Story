using System;
using Ink.Runtime;
using UnityEngine;

[System.Serializable]
public class ItemModel {
    [SerializeField, Disable] public InkListItem inkListItem;
    
    [SerializeField, Disable] string _inkListItemName;
    public string inkListItemName {
        get => _inkListItemName;
        set {
            if(_inkListItemName == value) return;
            _inkListItemName = value;
        }
    }
    
    [SerializeField, Disable] string _inkListItemFullName;
    public string inkListItemFullName {
        get => _inkListItemFullName;
        set {
            if(_inkListItemFullName == value) return;
            _inkListItemFullName = value;
        }
    }
    
    [SerializeField, Disable] string _labelText;
    public string labelText {
        get => _labelText;
        set {
            if(_labelText == value) return;
            _labelText = value;
            onChangeLabelText?.Invoke(value);
        }
    }
    public Action<string> onChangeLabelText;
    
    [SerializeField, Disable] string _tooltipText;
    public string tooltipText {
        get => _tooltipText;
        set {
            if(_tooltipText == value) return;
            _tooltipText = value;
            onChangeTooltipText?.Invoke(value);
        }
    }
    public Action<string> onChangeTooltipText;
    
    [SerializeField, Disable] State _state;
    public State state {
        get => _state;
        set {
            if(_state == value) return;
            _state = value;
            onChangeState?.Invoke(value);
        }
    }
    public Action<State> onChangeState;
    public enum State {
        Hidden,
        Searchable,
        Showing
    }
    
    public ItemModel(InkListItem inkListItem) {
        this.inkListItem = inkListItem;
        inkListItemName = inkListItem.itemName;
        inkListItemFullName = inkListItem.itemName;
        state = State.Searchable;
        RefreshInkVariables();
    }
    
    public void RefreshInkVariables() {
        labelText = GameController.Instance.GetItemName(inkListItem);
        tooltipText = GameController.Instance.GetItemTooltip(inkListItem);
    }
}