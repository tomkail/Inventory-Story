using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ProjectorScreenPostProcessor : MonoBehaviour {
    public RawImage rawImage;
    public new Camera camera;
    
    public void ApplyPostProcessing(RenderTexture renderTexture) {
        if(rawImage == null || camera == null) return;

        rawImage.texture = renderTexture;
        
        camera.targetTexture = renderTexture;
        camera.Render();
        camera.targetTexture = null;
        //
        // CheckForHDRValues(renderTexture);
        // void CheckForHDRValues(RenderTexture rt)
        // {
        //     // Temporary Texture2D to hold the RenderTexture's pixels
        //     Texture2D tempTexture = new Texture2D(rt.width, rt.height, TextureFormat.RGBAFloat, false);
        //
        //     // Save the current active RenderTexture
        //     RenderTexture currentActiveRT = RenderTexture.active;
        //
        //     // Set the RenderTexture as active so we can read from it
        //     RenderTexture.active = rt;
        //
        //     // Read the pixels from the RenderTexture into the Texture2D
        //     tempTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        //     tempTexture.Apply();
        //
        //     // Restore the previously active RenderTexture
        //     RenderTexture.active = currentActiveRT;
        //
        //     // Check the pixels for HDR values
        //     Color[] pixels = tempTexture.GetPixels();
        //     foreach (Color pixel in pixels)
        //     {
        //         if (pixel.r > 1 || pixel.g > 1 || pixel.b > 1 || pixel.r < 0 || pixel.g < 0 || pixel.b < 0)
        //         {
        //             Debug.Log("This RenderTexture contains HDR values.");
        //             return;
        //         }
        //     }
        //     Debug.Log("This RenderTexture does not contain HDR values.");
        // }
    }
}
