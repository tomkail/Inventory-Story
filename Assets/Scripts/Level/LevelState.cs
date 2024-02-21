using System.Collections.Generic;

[System.Serializable]
public class LevelState {
    public string uuid;
    public string sceneId;
    public string titleText;
    public string dateText;
    public List<ItemModel> itemStates = new List<ItemModel>();
}