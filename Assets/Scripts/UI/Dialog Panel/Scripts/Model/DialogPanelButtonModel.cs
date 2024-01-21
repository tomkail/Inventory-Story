using System;

[System.Serializable]
public class DialogPanelButtonModel : DialogPageItemModelBase {
    public string textStr;
    public Action onClick;
    public enum ButtonType {
        Standard,
        Label
    }
    public ButtonType buttonType;
    public bool interactable;
    public DialogPanelButtonModel (string textStr, Action onClick, ButtonType buttonType = ButtonType.Standard, bool interactable = true) {
        this.textStr = textStr;
        this.onClick = onClick;
        this.buttonType = buttonType;
        this.interactable = interactable;
    }
}