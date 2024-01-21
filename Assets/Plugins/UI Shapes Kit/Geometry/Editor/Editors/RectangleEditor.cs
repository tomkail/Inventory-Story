using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using Rectangle = ThisOtherThing.UI.Shapes.Rectangle;

[CustomEditor(typeof(Rectangle))]
[CanEditMultipleObjects]
public class RectangleEditor : BaseShapesEditor
{
	Rectangle rectangle;

	protected SerializedProperty spriteProp;

	protected SerializedProperty shapePropertiesProp;
	protected SerializedProperty roundedPropertiesProp;
	protected SerializedProperty outlinePropertiesProp;
	protected SerializedProperty shadowPropertiesProp;
	protected SerializedProperty antiAliasingPropertiesProp;

	protected override void OnEnable() 
	{
		base.OnEnable();
		rectangle = (Rectangle)target;

		spriteProp = serializedObject.FindProperty("Sprite");

		shapePropertiesProp = serializedObject.FindProperty("ShapeProperties");
		roundedPropertiesProp = serializedObject.FindProperty("RoundedProperties");
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
		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(shapePropertiesProp, true);
		EditorGUILayout.PropertyField(roundedPropertiesProp, true);

		if (rectangle.ShapeProperties.DrawOutline)
		{
			EditorGUILayout.PropertyField(outlinePropertiesProp, true);
		}

		EditorGUILayout.PropertyField(shadowPropertiesProp, true);
		EditorGUILayout.PropertyField(antiAliasingPropertiesProp, true);

		serializedObject.ApplyModifiedProperties();
	}
}
