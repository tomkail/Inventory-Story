using System;
using UnityEngine;

public class ScannerSettings : ScriptableObject {
    public float moveSpeed = 100;
    public float zoom = 1.2f;
    public float minSuccessScore = 0.8f;
    
    [Space]
    public ScanSettings scanSettings;
    [Serializable]
    public class ScanSettings {
        public float scanTime = 1;
        public AnimationCurve zoomOverProgress;
    }
    
    [Space]
    public ItemScoreSettings itemScoreSettings;
    [Serializable]
    public class ItemScoreSettings {
        public AnimationCurve scoreOverItemDistance;
    }

    public ScoreNoiseModifier scoreNoiseModifier;
    [Serializable]
    public class ScoreNoiseModifier {
        public float strength = 0.2f;
        public NoiseSampler scoreNoiseSampler;
        public float positionFrequency = 10;
        public float timeFrequency = 10;
        
    }
    
    public VisualSettings visualSettings;

    [Serializable]
    public class VisualSettings {
        public AnimationCurve pulseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public AnimationCurve pulseSpeedOverStrength = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public AnimationCurve pulseAlphaOverStrength = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }
}