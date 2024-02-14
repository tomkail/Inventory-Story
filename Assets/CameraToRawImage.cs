using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class CameraToRawImage : MonoBehaviour {
    public RawImage rawImage;
    public RenderTextureCreator renderTextureCreator;
    public Camera camera;

    void Update() {
        if(rawImage == null || renderTextureCreator == null || camera == null) return;
        
        renderTextureCreator.RefreshRenderTexture();
        camera.targetTexture = renderTextureCreator.renderTexture;
        camera.Render();
        camera.targetTexture = null;
        
        rawImage.texture = renderTextureCreator.renderTexture;
    }
}
