using UnityEngine.UI;

public class ModalMenuButton : ModalMenuItemViewBase {
    public Button button;
    public SLayout text;
    public SLayout icon;
    public ModalMenuButtonModel model;

    void Awake () {
        button.onClick.AddListener(OnClickButton);
    }
    public void Init (ModalMenuButtonModel model) {
        this.model = model;
        text.textMeshPro.text = model.textStr;
        icon.image.sprite = model.iconSprite;
        icon.image.enabled = icon.image.sprite != null;
    }

    void OnClickButton () {
        model.onClick?.Invoke();
    }

    public override void Layout () {
        icon.rightX = layout.width - margin.right;
        icon.centerY = layout.height * 0.5f;

        text.size = text.textMeshPro.GetRenderedValues(text.textMeshPro.text, layout.width-margin.horizontal);
        text.centerY = layout.height * 0.5f;
        text.x = margin.left;
    }
}