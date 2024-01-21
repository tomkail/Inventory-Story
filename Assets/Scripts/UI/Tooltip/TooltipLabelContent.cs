using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TooltipLine {
    public string text;
    public TooltipLineType lineType;
    public enum TooltipLineType {
        Normal,
        Error
    }

    public TooltipLine (string text, TooltipLineType lineType) {
        this.text = text;
        this.lineType = lineType;
    }
}
public class TooltipLabelContent : MonoBehaviour {
    public Prototype prototype;
    public SLayout layout;
    public RectOffset padding;
    [Space]
    public float contentSpacing = 20;
    public float maxLineWidth = 320;


    [SerializeField]
    Prototype normalTextPrototype = null;
    [SerializeField]
    Prototype errorTextPrototype = null;
    
    [SerializeField, Disable]
    List<Prototype> contents = new List<Prototype>();

    void OnDisable () {
        foreach(var content in contents) content.ReturnToPool();
        contents.Clear();
    }
    
    public void Init (params TooltipLine[] lines) {
        for(int i = 0; i < lines.Length; i++) {
            var lineParams = lines[i];
            SLayout line = null;
            if(lineParams.lineType == TooltipLine.TooltipLineType.Normal) {
                line = normalTextPrototype.Instantiate<SLayout>();
            } else if(lineParams.lineType == TooltipLine.TooltipLineType.Error) {
                line = errorTextPrototype.Instantiate<SLayout>();
            }
            if(line != null) {
                line.transform.SetAsLastSibling();
                line.textMeshPro.text = lineParams.text;
                line.size = TextMeshProUtils.GetRenderedValues(line.textMeshPro, line.textMeshPro.text, maxLineWidth, Mathf.Infinity);
                line.textMeshPro.ForceMeshUpdate();
                var prototype = line.GetComponent<Prototype>();
                contents.Add(prototype);
            }
        }
        Layout();
    }

    public void Layout () {
        float y = padding.top;
        float largestContentWidth = 0f;
        for(int i = 0; i < contents.Count; i++) {
            var lineLayout = contents[i].GetComponent<SLayout>();
            largestContentWidth = Mathf.Max(largestContentWidth, lineLayout.size.x);
            y += lineLayout.height;
            if(i < contents.Count-1) y += contentSpacing;
        }
        y += padding.bottom;

        layout.width = largestContentWidth + padding.horizontal;
        layout.height = y;

        y = padding.top;
        for(int i = 0; i < contents.Count; i++) {
            var lineLayout = contents[i].GetComponent<SLayout>();

            lineLayout.centerX = lineLayout.parent.width * 0.5f;
            lineLayout.y = y;
            
            y += lineLayout.height;
            if(i < contents.Count-1) y += contentSpacing;
        }
    }
}