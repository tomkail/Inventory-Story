using System.Collections;
using System.Collections.Generic;
using ThisOtherThing.UI.ShapeUtils;
using UnityEngine;

[ExecuteAlways]
public class UILineGrid : MonoBehaviour {
    RectTransform rectTransform => lineRenderer.rectTransform;
    ThisOtherThing.UI.Shapes.Line lineRenderer => GetComponent<ThisOtherThing.UI.Shapes.Line>();
    UnityEngine.UI.GridLayout gridLayout => GetComponent<UnityEngine.UI.GridLayout>();
    void Update() {
        List<PointsList.PointListProperties> pointListProperties = new List<PointsList.PointListProperties>();
        
        for (int y = 0; y < gridLayout.yAxis.GetCellCount(); y++) {
            for (int x = 0; x < gridLayout.xAxis.GetCellCount(); x++) {
                var localRect = gridLayout.GetLocalRectForGridCoord(new Vector2Int(x,y));
                var lineProperties = new PointsList.PointListProperties();
                lineProperties.Positions = new Vector2[3];
                lineProperties.Positions[0] = new Vector2(localRect.min.x, localRect.max.y-lineRenderer.OutlineProperties.LineWeight * 0.5f);
                lineProperties.Positions[1] = localRect.min;
                lineProperties.Positions[2] = new Vector2(localRect.max.x-lineRenderer.OutlineProperties.LineWeight * 0.5f, localRect.min.y);
                lineProperties.SetPoints();
                pointListProperties.Add(lineProperties);
            }    
        }

        lineRenderer.PointListsProperties.PointListProperties = pointListProperties.ToArray();
        lineRenderer.ForceMeshUpdate();
    }
}
