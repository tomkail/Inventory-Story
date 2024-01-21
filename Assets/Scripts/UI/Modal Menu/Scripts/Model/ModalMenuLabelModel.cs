[System.Serializable]
public class ModalMenuLabelModel : ModalMenuItemModelBase {
    public string textStr;
    public float fontSize = 42;

    public ModalMenuLabelModel (string textStr, float fontSize = 42) {
        this.textStr = textStr;
        this.fontSize = fontSize;
    }
}
