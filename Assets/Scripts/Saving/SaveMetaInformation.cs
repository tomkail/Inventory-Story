using System;

// Saved information about the state of the save
[System.Serializable]
public class SaveMetaInformation {
    public SaveVersion saveVersion;
	public DateTime saveDateTime;
	public string saveDescription;
}
