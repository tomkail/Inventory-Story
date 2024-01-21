[System.Serializable]
public class DialogPanelLabelModel : DialogPageItemModelBase {
    public string textStr;
    public TextType textType;
    
    public enum TextType {
        Header,
        Body
    }
    
    public DialogPanelLabelModel (string textStr, TextType textType) {
        this.textStr = textStr;
        this.textType = textType;
    }
}