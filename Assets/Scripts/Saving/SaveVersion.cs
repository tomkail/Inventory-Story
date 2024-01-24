[System.Serializable]
public struct SaveVersion {
    public const int buildSaveVersion = 2;
    // INCLUSIVE!
    public const int minCompatableSaveVersion = 1;

    public int version;
    public SaveVersion (int version) {
        this.version = version;
    }

    public bool IsCompatableWithBuild () {
        return 
        // Check that we can upgrade from here
        version >= minCompatableSaveVersion
        // And that we've not loaded an old version of the game
            && !(buildSaveVersion < version);
    }

    public override string ToString() {
        return string.Format("[{0}] Version:{1}", GetType().Name, version);
    }
}