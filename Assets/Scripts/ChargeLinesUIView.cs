using ThisOtherThing.UI.ShapeUtils;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(ThisOtherThing.UI.Shapes.Line))]
public class ChargeLinesUIView : MonoBehaviour {
    RectTransform rectTransform => GetComponent<RectTransform>();
    ThisOtherThing.UI.Shapes.Line lineRenderer => GetComponent<ThisOtherThing.UI.Shapes.Line>();
    
    [Range(0f,1f)]
    public float progress;
    
    [Space]
    public int numLines = 12;
    

    void Update() {
        progress = Mathf.Clamp01(progress);
        
        var lineWeight = lineRenderer.OutlineProperties.LineWeight;
        var lineSpace = lineRenderer.OutlineProperties.LineWeight;
        var range = new Range(lineWeight * 0.5f, rectTransform.rect.width - lineWeight * 0.5f);
        numLines = Mathf.CeilToInt(rectTransform.rect.width / (lineWeight + lineSpace));
        var numVisibleLines = Mathf.CeilToInt(numLines * progress);
        
        lineRenderer.PointListsProperties.PointListProperties = new PointsList.PointListProperties[numVisibleLines];
        var pivotOffset = -(rectTransform.pivot) * rectTransform.rect.size;
        for (int i = 0; i < numVisibleLines; i++) {
            var x = i / ((float) numLines - 1);
            
            Vector2 start = new Vector2(range.Lerp(x), lineRenderer.LineProperties.LineCap == Lines.LineProperties.LineCapTypes.Close ? 0 : 0+lineRenderer.OutlineProperties.LineWeight*0.5f);
            Vector2 end = new Vector2(start.x, lineRenderer.LineProperties.LineCap == Lines.LineProperties.LineCapTypes.Close ? rectTransform.rect.height : rectTransform.rect.height-lineRenderer.OutlineProperties.LineWeight*0.5f);
            
            lineRenderer.PointListsProperties.PointListProperties[i] = new PointsList.PointListProperties();
            lineRenderer.PointListsProperties.PointListProperties[i].Positions = new Vector2[2];
            lineRenderer.PointListsProperties.PointListProperties[i].Positions[0] = start + pivotOffset;
            lineRenderer.PointListsProperties.PointListProperties[i].Positions[1] = end + pivotOffset;
            lineRenderer.PointListsProperties.PointListProperties[i].SetPoints();
        }
        lineRenderer.ForceMeshUpdate();
    }
}