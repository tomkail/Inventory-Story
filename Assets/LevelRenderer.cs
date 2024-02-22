using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class LevelRenderer : MonoBehaviour {
    public Level level;
    public RawImage rawImage;
    [Disable] public RenderTexture renderTexture;
    RenderTextureFormat renderTextureFormat = RenderTextureFormat.DefaultHDR;
    
    [Space]
    public bool postProcessing;

    void Update() {
        if (rawImage == null) return;
        rawImage.enabled = Application.isPlaying && UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject) == null;
        if(!rawImage.enabled) return;
        if(level == null) return;
        UIImposterRenderer.Render(level.layout.rectTransform, new UIImposterOutputParams() {
            containerScalingMode = UIImposterOutputParams.ScalingMode.AspectFill,
            sizeMode = UIImposterOutputParams.SizeMode.OriginalCanvasSize,
        }, ref renderTexture, new RenderTextureDescriptor(0,0, renderTextureFormat, 0));
        if(postProcessing)
            GameController.Instance.projectorPostProcessor.ApplyPostProcessing(renderTexture);
        rawImage.texture = renderTexture;
    }
}
