using UnityEditor;
using System.Reflection;

// https://forum.unity.com/threads/hierarchy-selection-select-first-child-of-selection-and-expand-collapse-selection.1059023/
public static class CollapseExpand {
    private static object[] m_ParametersExpand = new object[] { null, true };
    private static object[] m_ParametersCollapse = new object[] { null, false };
    private static System.Type m_SceneHierarchyWindowType = null;
    private static System.Type SceneHierarchyWindowType
    {
        get
        {
            if (m_SceneHierarchyWindowType == null)
            {
                var assembly = typeof(EditorWindow).Assembly;
                m_SceneHierarchyWindowType = assembly.GetType("UnityEditor.SceneHierarchyWindow");
            }
            return m_SceneHierarchyWindowType;
        }
    }
    private static MethodInfo m_SetExpandedRecursive = null;
    private static MethodInfo SetExpandedRecursiveImpl
    {
        get
        {
            if (m_SetExpandedRecursive == null)
                m_SetExpandedRecursive = m_SceneHierarchyWindowType.GetMethod("SetExpandedRecursive");
            return m_SetExpandedRecursive;
        }
    }
    public static void SetExpandedRecursive(int aInstanceID, bool aExpand)
    {
        var hierachyWindow = EditorWindow.GetWindow(SceneHierarchyWindowType);
        if (aExpand)
        {
            m_ParametersExpand[0] = aInstanceID;
            SetExpandedRecursiveImpl.Invoke(hierachyWindow, m_ParametersExpand);
        }
        else
        {
            m_ParametersCollapse[0] = aInstanceID;
            SetExpandedRecursiveImpl.Invoke(hierachyWindow, m_ParametersCollapse);
        }
    }
    //
    // /// <summary>
    // /// Expand or collapse object in Hierarchy recursively
    // /// </summary>
    // /// <param name="obj">The object to expand or collapse</param>
    // /// <param name="expand">A boolean to indicate if you want to expand or collapse the object</param>
    // public static void SetExpandedRecursive (GameObject obj, bool expand)
    // {
    //     var methodInfo = GetHierarchyWindowType().GetMethod("SetExpandedRecursive");
    //
    //     methodInfo.Invoke(GetHierarchyWindow(), new object[] { obj.GetInstanceID(), expand });
    // }
    //
    // /// <summary>
    // ///  Expand or collapse object in Hierarchy
    // /// </summary>
    // /// <param name="obj">The object to expand or collapse</param>
    // /// <param name="expand">A boolean to indicate if you want to expand or collapse the object</param>
    // public static void SetExpanded (GameObject obj, bool expand)
    // {
    //     object sceneHierarchy = GetHierarchyWindowType().GetProperty("sceneHierarchy").GetValue(GetHierarchyWindow());
    //     var methodInfo = sceneHierarchy.GetType().GetMethod("ExpandTreeViewItem", BindingFlags.NonPublic | BindingFlags.Instance);
    //
    //     methodInfo.Invoke(sceneHierarchy, new object[] { obj.GetInstanceID(), expand });
    // }

 
    [MenuItem("CONTEXT/GameObject/Expand GameObjects")]
    [MenuItem("GameObject/Expand GameObjects", priority = 40)]
    private static void ExpandGameObjects()
    {
        SetExpandedRecursive(Selection.activeGameObject.GetInstanceID(), true);
    }
 
    [MenuItem("CONTEXT/GameObject/Collapse GameObjects")]
    [MenuItem("GameObject/Collapse GameObjects", priority = 40)]
    private static void CollapseGameObjects()
    {
        SetExpandedRecursive(Selection.activeGameObject.GetInstanceID(), false);
    }
 
    [MenuItem("GameObject/Expand GameObjects", validate = true)]
    [MenuItem("GameObject/Collapse GameObjects", validate = true)]
    private static bool CanExpandOrCollapse()
    {
        return Selection.activeGameObject != null;
    }
}