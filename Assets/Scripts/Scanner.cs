using System;
using System.Linq;
using UnityEngine;

public class Scanner : MonoBehaviour {
    public Level level => GetComponentInParent<Level>();
    public RectTransform rectTransform => GetComponent<RectTransform>();

    public ScannerSettings scannerSettings;

    [Disable, SerializeField] bool scanning;
    [Disable, SerializeField] float scanTime = -1;
    [Disable, SerializeField] float scanProgress;
    
    [Space]
    public ZoomedInImage zoomedInImage;
    public SLayout pulseFill;
    [Disable, SerializeField] float finalStrength = 0;
    [Disable, SerializeField] float itemDistanceStrength = 0;
    [Disable, SerializeField] float pulseTime = 0;

    void OnEnable() {
        scanTime = -1;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)rectTransform.parent, ScreenX.screenRect.ClosestPoint(Input.mousePosition), rectTransform.GetCanvasEventCamera(), out Vector2 localPoint);
        rectTransform.localPosition = localPoint;
        
        Update();
    }

    void Update() {
        zoomedInImage.target = level.viewport.background;
        
        if(!scanning) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)rectTransform.parent, ScreenX.screenRect.ClosestPoint(Input.mousePosition), rectTransform.GetCanvasEventCamera(), out Vector2 localPoint);
            rectTransform.localPosition = Vector2.MoveTowards(rectTransform.localPosition, localPoint, scannerSettings.moveSpeed * Time.deltaTime);
        }
        

        itemDistanceStrength = 0;
        var validItems = level.itemViews.Where(itemView => itemView != null);
        ItemView validItem = null;
        foreach (var itemView in validItems) {
            var localPosition = itemView.layout.rectTransform.TransformPointTo(itemView.layout.rectTransform.rect.center, rectTransform);
            var distance = Vector2.Distance(RectX.ClosestPoint(rectTransform.rect, localPosition), localPosition);
            var strength = scannerSettings.itemScoreSettings.scoreOverItemDistance.Evaluate(distance);
            itemDistanceStrength = Mathf.Max(strength, itemDistanceStrength);
            if(strength > scannerSettings.minSuccessScore) {
                validItem = itemView;
            }
        }

        scannerSettings.scoreNoiseModifier.scoreNoiseSampler.position = new Vector3(rectTransform.localPosition.x * scannerSettings.scoreNoiseModifier.positionFrequency, rectTransform.localPosition.y * scannerSettings.scoreNoiseModifier.positionFrequency, Time.time * scannerSettings.scoreNoiseModifier.timeFrequency);
        var noiseStrength = scannerSettings.scoreNoiseModifier.strength * scannerSettings.scoreNoiseModifier.scoreNoiseSampler.Sample().value;
        finalStrength = itemDistanceStrength + noiseStrength;

        pulseTime += Time.deltaTime * scannerSettings.visualSettings.pulseSpeedOverStrength.Evaluate(finalStrength);

        if (Input.GetMouseButtonDown(0)) {
            scanning = true;
            scanTime = 0;
        }

        if (scanning) {
            scanTime += Time.deltaTime;
            scanProgress = scanTime / scannerSettings.scanSettings.scanTime;
            zoomedInImage.zoom = scannerSettings.scanSettings.zoomOverProgress.Evaluate(scanProgress);
            pulseFill.groupAlpha = 0;

            if (scanProgress >= 1) {
                if(validItem != null)
                    validItem.itemModel.state = ItemModel.State.Showing;
                scanning = false;
                
                level.scanModeFlags = (ScanModeStateFlags)FlagsX.SetFlag((int)level.scanModeFlags, (int)ScanModeStateFlags.Active, false);
            }
        } else {
            scanProgress = 0;
            zoomedInImage.zoom = scannerSettings.zoom;
            pulseFill.groupAlpha = scannerSettings.visualSettings.pulseCurve.Evaluate(pulseTime) * scannerSettings.visualSettings.pulseAlphaOverStrength.Evaluate(finalStrength);
        }
        
        
        if (Input.GetMouseButtonUp(0)) {
            scanning = false;
            scanTime = -1;
        }
    }
}