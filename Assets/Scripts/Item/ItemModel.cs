using System;
using System.Text;
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
    
    [SerializeField, Disable] bool _isZoomable;
    public bool isZoomable {
        get => _isZoomable;
        set {
            if(_isZoomable == value) return;
            _isZoomable = value;
            onChangeIsZoomable?.Invoke(value);
        }
    }
    public Action<bool> onChangeIsZoomable;
    
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
        inkListItemFullName = inkListItem.fullName;
        state = State.Searchable;
        RefreshInkVariables();
    }
    
    public void RefreshInkVariables() {
        labelText = GetItemName(GameController.Instance.story, inkListItem);
        tooltipText = GetItemTooltip(GameController.Instance.story, inkListItem);
        isZoomable = GetItemIsZoomable(GameController.Instance.story, inkListItem);
    }

    static string GetItemName(Story story, InkListItem inkListItem) {
        var str = story.RunInkFunction<string>("getItemName", InkList.FromString(inkListItem.fullName, story));
        if (str.ToLower() != str) {
            // No item name exists for this item
            // Debug.LogWarning("getItemName didn't return improved name for " + inkListItem.itemName);
            StringBuilder sb = new StringBuilder();
            sb.Append(char.ToUpper(str[0])); // Ensure the first character is uppercase.
            for (int i = 1; i < str.Length; i++) {
                if (char.IsUpper(str[i])) {
                    sb.Append(' ');
                    sb.Append(char.ToLower(str[i]));
                } else {
                    sb.Append(str[i]);
                }
            }
            return sb.ToString();
        }
        return InkStylingUtility.ProcessText(str);
    }
    static string GetItemTooltip(Story story, InkListItem inkListItem) {
        var str = story.RunInkFunction<string>("getItemTooltip", InkList.FromString(inkListItem.fullName, story));
        return InkStylingUtility.ProcessText(str);
    }

    static bool GetItemIsZoomable(Story story, InkListItem inkListItem) {
        return story.RunInkFunction<bool>("isZoomable", InkList.FromString(inkListItem.fullName, story));
    }
}