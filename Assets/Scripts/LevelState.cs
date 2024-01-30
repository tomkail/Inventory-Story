using System.Collections.Generic;

[System.Serializable]
public class LevelState {
    public string titleText;
    public string dateText;
    public List<LevelItemState> itemStates = new List<LevelItemState>();
}