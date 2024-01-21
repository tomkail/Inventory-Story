using TMPro;

public class DialogPanelInputFieldUIView : DialogPageItemViewBase {
    public TMP_InputField inputField;
    public DialogPanelInputFieldModel model;
    public void Init (DialogPanelInputFieldModel model) {
        this.model = model;
        ((TextMeshProUGUI) inputField.placeholder).text = model.placeholderText;
        if(model.startSelected) inputField.Select();
        model.GetCurrentText = () => inputField.text;
        this.model.onChange += SetLayoutDirty;
    }

    protected override void LayoutInternal() { }
}