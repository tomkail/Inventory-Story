using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

using Ellipse = ThisOtherThing.UI.Shapes.Ellipse;

[CustomEditor(typeof(Ellipse))]
[CanEditMultipleObjects]
public class EllipseEditor : GraphicEditor
{
	Ellipse ellipse;

	protected SerializedProperty spriteProp;
	
	protected SerializedProperty shapePropertiesProp;
	protected SerializedProperty ellipsePropertiesProp;
	protected SerializedProperty outlinePropertiesProp;
	protected SerializedProperty shadowPropertiesProp;
	protected SerializedProperty antiAliasingPropertiesProp;

	protected override void OnEnable()
	{
		base.OnEnable();
		ellipse = (Ellipse)target;

		spriteProp = serializedObject.FindProperty("Sprite");

		shapePropertiesProp = serializedObject.FindProperty("ShapeProperties");
		ellipsePropertiesProp = serializedObject.FindProperty("EllipseProperties");
		outlinePropertiesProp = serializedObject.FindProperty("OutlineProperties");
		shadowPropertiesProp = serializedObject.FindProperty("ShadowProperties");
		antiAliasingPropertiesProp = serializedObject.FindProperty("AntiAliasingProperties");
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Tools.hidden = false;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		serializedObject.Update();
		EditorGUILayout.PropertyField(spriteProp);
		
		EditorGUILayout.PropertyField(shapePropertiesProp, true);
		EditorGUILayout.PropertyField(ellipsePropertiesProp, true);

		if (ellipse.ShapeProperties.DrawOutline)
		{
			EditorGUILayout.PropertyField(outlinePropertiesProp, true);
		}

		EditorGUILayout.PropertyField(shadowPropertiesProp, true);
		EditorGUILayout.PropertyField(antiAliasingPropertiesProp, true);

		serializedObject.ApplyModifiedProperties();
	}
}
