public class DialogPanelTextUIView : DialogPageItemViewBase {
    public SLayout text;
    public DialogPanelLabelModel model;
    public void Init (DialogPanelLabelModel model) {
        this.model = model;
        this.model.onChange += SetLayoutDirty;
    }

    protected override void LayoutInternal() {
        text.textMeshPro.text = model.textStr;
        text.height = text.textMeshPro.GetTightPreferredValues(text.width).y;
    }
}