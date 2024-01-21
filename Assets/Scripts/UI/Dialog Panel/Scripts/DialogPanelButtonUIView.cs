using TMPro;
using UnityEngine.UI;

public class DialogPanelButtonUIView : DialogPageItemViewBase {
    public DialogPanelButtonModel model;
    public SLayout textLayout => layout.GetComponentInChildren<TextMeshProUGUI>().GetComponent<SLayout>();
    public void Init (DialogPanelButtonModel model) {
        this.model = model;
        textLayout.textMeshPro.text = model.textStr;
        textLayout.textMeshPro.ForceMeshUpdate();
        var button = layout.GetComponent<Button>();
        button.interactable = model.interactable;
        button.onClick.AddListener(() => {
            model.onClick?.Invoke();
        });
    }

    protected override void LayoutInternal() {
        textLayout.width = textLayout.textMeshPro.GetTightPreferredValues(layout.parentRect.width - 24 * 2).x;
        layout.width = textLayout.width + 24 * 2;
        textLayout.x = 24;
        layout.centerX = layout.parentRect.width / 2;
    }
}
