using System;

[System.Serializable]
public class DialogPanelInputFieldModel : DialogPageItemModelBase {
    public string placeholderText;
    public bool startSelected;
    
    public DialogPanelInputFieldModel (string placeholderText, bool startSelected) {
        this.placeholderText = placeholderText;
        this.startSelected = startSelected;
    }
    
    public Func<string> GetCurrentText;
}