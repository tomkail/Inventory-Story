using UnityEngine;

public class ModalMenuLabel : ModalMenuItemViewBase {
    public SLayout text;
    public ModalMenuLabelModel model;
    public void Init (ModalMenuLabelModel model) {
        this.model = model;
        text.textMeshPro.text = model.textStr;
    }

    public override void Layout () {
        text.textMeshPro.fontSize = model.fontSize;
        text.size = text.textMeshPro.GetRenderedValues(text.textMeshPro.text, layout.width-margin.horizontal);
        layout.height = text.height + margin.vertical;
        text.centerY = layout.height * 0.5f;
        text.centerX = layout.width * 0.5f;
    }
}