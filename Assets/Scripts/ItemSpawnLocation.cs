using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ItemSpawnLocation : MonoBehaviour {
    public RectTransform rectTransform => GetComponent<RectTransform>();

    void OnDrawGizmosSelected() {
        GizmosX.BeginMatrix(rectTransform.localToWorldMatrix);
        GizmosX.DrawWireRect(rectTransform.rect);
        GizmosX.EndMatrix();
    }
}