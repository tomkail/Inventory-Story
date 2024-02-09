using Ink.Runtime;
using UnityEngine;

[System.Serializable]
public class LevelItemState {
    public InkListItem inkListItem;
    public Vector2 labelPosition;
    public ItemView.State state;
    public string labelText;
    public string tooltipText;
}