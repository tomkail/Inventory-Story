using System.Collections.Generic;

[System.Serializable]
public class SaveState {
    public string saveDescription;
    public GameMetaInformation gameMetaInfo;
    public SaveMetaInformation saveMetaInfo;
    
    public string storySaveJson;
    public List<LevelState> levelStates = new List<LevelState>();

    public SaveState () {}
}
