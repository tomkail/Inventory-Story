using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemView))]
public class ItemViewEditor : BaseEditor<ItemView> {
	public override void OnInspectorGUI () {
		base.OnInspectorGUI();

		EditorGUI.BeginDisabledGroup(true);
		EditorGUILayout.Space();
		EditorGUILayout.ObjectField("Hovered slottable", data.hoveredSlottable as Object, typeof(ItemView));
		
		EditorGUILayout.Space();
		
		EditorGUILayout.ObjectField("Hovered slot", data.hoveredSlot as Object, typeof(ItemView));
		EditorGUILayout.ObjectField("Container slot", data.containerSlot as Object, typeof(ItemView));
		EditorGUI.EndDisabledGroup();
	}
}