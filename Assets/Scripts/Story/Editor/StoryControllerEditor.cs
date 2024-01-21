using UnityEngine;
using UnityEditor;
using Ink.Runtime;
using Ink.UnityIntegration;

[InitializeOnLoad]
[CustomEditor(typeof(StoryController))]
public class StoryControllerEditor : BaseEditor<StoryController> {
	static bool expanded;
	static InkPlayerWindow.InkPlayerParams playerParams;
    
    static StoryControllerEditor () {
        playerParams = InkPlayerWindow.InkPlayerParams.DisableInteraction;
        playerParams.disableSettingVariables = false;
        
        StoryController.OnCreateStory += OnCreateStory;
    }

    static void OnCreateStory (Story story) {
        if(StoryControllerEditorSettings.Instance.attachOnPlay) {
            if(!InkPlayerWindow.GetGameWindowIsMaximised())
                InkPlayerWindow.GetWindow(false);
            InkPlayerWindow.Attach(story, playerParams);
        }
    }
	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();

        EditorGUILayout.Separator();

		InkPlayerWindow.DrawStoryPropertyField(data.story, playerParams, ref expanded, new GUIContent("Story"));

        EditorGUI.BeginChangeCheck();
        StoryControllerEditorSettings.Instance.attachOnPlay = EditorGUILayout.Toggle(new GUIContent("Attach On Play"), StoryControllerEditorSettings.Instance.attachOnPlay);
        if(EditorGUI.EndChangeCheck()) {
            StoryControllerEditorSettings.Save();
        }
	}
}

public class StoryControllerEditorSettings : SerializedScriptableSingleton<StoryControllerEditorSettings> {
    public bool attachOnPlay = true;
}