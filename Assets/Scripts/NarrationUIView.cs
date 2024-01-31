using TMPro;
using UnityEngine;

public class NarrationUIView : MonoBehaviour {
    public SLayout layout => GetComponent<SLayout>();
    public TextMeshProUGUI voText;
    public TextFader textFader;
    public float fadeSpeed = 40;
    
    public void ShowText(string textStr) {
        voText.text = textStr;
        layout.CancelAnimations();
        layout.AnimateCustom(voText.text.Length / fadeSpeed, (progress) => {
            textFader.animationProgress = progress;
        });
    }
}