using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class LevelRenderer : MonoBehaviour {
    public Level level;
    public RawImage rawImage;
    public RenderTexture renderTexture;
    public RenderTextureFormat renderTextureFormat = RenderTextureFormat.DefaultHDR;
    
    [Space]
    public bool postProcessing;

    void Update() {
        rawImage.enabled = Application.isPlaying;
        if(level == null || rawImage == null) return;
        UIImposterRenderer.Render(level.layout.rectTransform, new UIImposterOutputParams() {
            containerScalingMode = UIImposterOutputParams.ScalingMode.AspectFill,
            sizeMode = UIImposterOutputParams.SizeMode.OriginalCanvasSize,
        }, ref renderTexture, new RenderTextureDescriptor(0,0, renderTextureFormat, 0));
        if(postProcessing)
            GameController.Instance.projectorPostProcessor.ApplyPostProcessing(renderTexture);
        rawImage.texture = renderTexture;
    }
}
