using UnityEngine;

public class ModalMenuHeader : ModalMenuItemViewBase {
    public SLayout text;
    public ModalMenuHeaderModel model;
    public void Init (ModalMenuHeaderModel model) {
        this.model = model;
        text.textMeshPro.text = model.textStr;
    }

    public override void Layout () {
        // text.textMeshPro.ForceMeshUpdate();
        text.size = text.textMeshPro.GetRenderedValues(text.textMeshPro.text, layout.width-margin.horizontal);
        text.centerY = layout.height * 0.5f;
        text.x = margin.left;
    }
}