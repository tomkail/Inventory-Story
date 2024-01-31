using TMPro;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TMP_Text))]
public abstract class BaseTextMeshProEffect : MonoBehaviour {
    public TMP_Text m_TextComponent;
    [System.NonSerialized] public bool isDirty;
    
    void Reset() {
        m_TextComponent = GetComponent<TMP_Text>();
    }

    void OnValidate() {
        isDirty = true;
    }

    void OnEnable() {
        m_TextComponent = GetComponent<TMP_Text>();
        // m_TextComponent.RegisterDirtyVerticesCallback(OnDirtyVerts);
        // m_TextComponent.RegisterDirtyLayoutCallback(OnDirtyVerts);
        // m_TextComponent.RegisterDirtyMaterialCallback(OnDirtyVerts);
        m_TextComponent.OnPreRenderText += OnPreRenderText;
        m_TextComponent.SetAllDirty();
        // Refresh();
    }

    void OnDisable() {
        // m_TextComponent.UnregisterDirtyVerticesCallback(OnDirtyVerts);
        // m_TextComponent.UnregisterDirtyLayoutCallback(OnDirtyVerts);
        // m_TextComponent.UnregisterDirtyMaterialCallback(OnDirtyVerts);
        m_TextComponent.OnPreRenderText -= OnPreRenderText;
        // Also refresh on disable if dirty so we catch anything that would have been updated had this component had a final update
        Clear();
    }
    
    void OnDirtyVerts() {
        Refresh();
    }

    void Update() {
        if (isDirty)
            Refresh();
    }

    public void Clear() {
        if (m_TextComponent == null) return;
        m_TextComponent.ForceMeshUpdate();
    }

    public void Refresh(bool evenIfInactive = false) {
        if (!isActiveAndEnabled && !evenIfInactive) return;
        isDirty = false;
        if (m_TextComponent == null || !m_TextComponent.enabled) return;
        // Force the text object to update right away so we can have geometry to modify right from the start.
        m_TextComponent.ForceMeshUpdate();
        // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
        m_TextComponent.UpdateVertexData();
    }
    
    // Effects are performed here!
    protected abstract void OnPreRenderText(TMP_TextInfo textInfo);
}