[System.Serializable]
public class ModalMenuHeaderModel : ModalMenuItemModelBase {
    public string textStr;

    public ModalMenuHeaderModel (string textStr) {
        this.textStr = textStr;
    }
}