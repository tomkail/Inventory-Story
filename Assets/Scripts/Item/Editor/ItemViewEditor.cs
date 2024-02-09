using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemLabelView))]
public class ItemViewEditor : BaseEditor<ItemLabelView> {
	public override void OnInspectorGUI () {
		base.OnInspectorGUI();

		EditorGUI.BeginDisabledGroup(true);
		EditorGUILayout.Space();
		EditorGUILayout.ObjectField("Hovered slottable", data.hoveredSlottable as Object, typeof(ItemLabelView));
		
		EditorGUILayout.Space();
		
		EditorGUILayout.ObjectField("Hovered slot", data.hoveredSlot as Object, typeof(ItemLabelView));
		EditorGUILayout.ObjectField("Container slot", data.containerSlot as Object, typeof(ItemLabelView));
		EditorGUI.EndDisabledGroup();

		if (GUILayout.Button("Layout")) {
			data.Layout();
		} 
	}
}