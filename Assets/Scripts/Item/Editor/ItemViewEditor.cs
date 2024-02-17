using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemDraggableGhostView))]
public class ItemViewEditor : BaseEditor<ItemDraggableGhostView> {
	public override void OnInspectorGUI () {
		base.OnInspectorGUI();

		EditorGUI.BeginDisabledGroup(true);
		EditorGUILayout.Space();
		EditorGUILayout.ObjectField("Hovered slottable", data.hoveredSlottable as Object, typeof(ItemDraggableGhostView));
		
		EditorGUILayout.Space();
		
		EditorGUILayout.ObjectField("Hovered slot", data.hoveredSlot as Object, typeof(ItemDraggableGhostView));
		EditorGUILayout.ObjectField("Container slot", data.containerSlot as Object, typeof(ItemDraggableGhostView));
		EditorGUI.EndDisabledGroup();

		if (GUILayout.Button("Layout")) {
			data.Layout();
		} 
	}
}