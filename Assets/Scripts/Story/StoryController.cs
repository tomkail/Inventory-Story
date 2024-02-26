using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ink.Runtime;

public class StoryController : MonoSingleton<StoryController> {
	public Story story {get; private set;}
	public bool hasStory => story != null;
	public bool begun { get; private set; }

	[SerializeReference] public List<ScriptContent> contents = new List<ScriptContent>();
	[SerializeReference] public List<ScriptChoice> choices = new List<ScriptChoice>();
	public static event Action<Story> OnCreateStory;
	public event Action OnParsedInstructions;

	public void InitStory (TextAsset storyJSONAsset, string storyStateJSON = null) {
		if(hasStory) DebugX.LogError("InitStory called but hasStory is true! This should never occur."); 
		
		story = new Story(storyJSONAsset.text);
		BindExternalFunctions();
		
		if(storyStateJSON != null) {
			try {
				story.state.LoadJson(storyStateJSON);
				if(!story.canContinue && story.currentChoices.Count == 0) {
					Debug.LogWarning("Content and choices are both empty! Restarting story "+storyJSONAsset.name+"...");
					story.ResetState();
				}
			} catch {
				DebugX.LogWarning("Failed loading story state for "+storyJSONAsset.name+"! Resetting.\n"+storyStateJSON);
				story.ResetState();
			}
		}
		if(OnCreateStory != null) OnCreateStory(story);
	}
	
	public void Begin () {
		if(begun) return;
        begun = true;
		if(contents == null) contents = new List<ScriptContent>();
		if(choices == null) choices = new List<ScriptChoice>();
        GatherInstructionsAndRefreshView();
	}

	public void ChoosePathString (string path) {
		story.ChoosePathString(path);
		OnChoseStoryPath();
	}
	
	public void MakeChoice (int choiceIndex) {
		if(choiceIndex < 0 || choiceIndex >= story.currentChoices.Count) {
			Debug.LogError("Index "+choiceIndex+" is not valid as length of choice array is "+story.currentChoices.Count +".");
			return;
		}
		story.ChooseChoiceIndex(choiceIndex);
		OnChoseStoryPath();
	}

	void OnChoseStoryPath () {
		if(begun) {
			GatherInstructionsAndRefreshView();
		}
	}
	
	public void GatherInstructionsAndRefreshView () { 
		// Clear
		contents.Clear();
		choices.Clear();

		bool interruptedMidContent = false;
		// Parse
		// We parse the content and the choice each time the story changes so that both are availiable at any time.
		{	
			while(story.canContinue) {
				// Continue gets the next line of the story
				string text = story.Continue ();
				// This removes any white space from the text.
				text = text.Trim();
				// Replaces some alt unicode characters that aren't expected in the font.
				text = text.Replace('’', '\'');
				
				if(string.IsNullOrWhiteSpace(text)) continue;
				ScriptContent parsedContent = null;
				if(ScriptContent.TryParse(text, story.currentTags, out parsedContent)) {
					contents.Add(parsedContent);
					if(parsedContent is SaveInstruction) SaveLoadManager.Save();
				}
			}
			
			if (!interruptedMidContent && story.currentChoices.Count > 0) {
				for(int i = 0; i < story.currentChoices.Count; i++) {
					choices.Add(ScriptChoice.ParseChoice(story.currentChoices[i]));
					// DebugX.Log(story.currentChoices[i].text);
				}
			}
		}
		
		if(OnParsedInstructions != null) OnParsedInstructions();
	}
	
	/// <summary>
	/// Called to end the story.
	/// </summary>
	public void EndStory () {
		begun = false;
        story = null;
	}
	
	void BindExternalFunctions () {
		story.onError += OnStoryError;
		story.variablesState["DEBUG"] = false;
		story.BindExternalFunction ("Save", SaveLoadManager.Save, false);
		story.BindExternalFunctionGeneral("StartScene", (args) => {
			var levelLoadParams = new LevelLoadParams();
			levelLoadParams.levelId = ((InkList)TryCoerce<InkList>(args[0])).FirstOrDefault().Key; 
			levelLoadParams.titleText = (string)TryCoerce<string>(args[1]); 
			levelLoadParams.dateText = (string)TryCoerce<string>(args[2]); 
			levelLoadParams.slotCount = (int)TryCoerce<int>(args[3]);
			levelLoadParams.startingItems = (InkList) TryCoerce<InkList>(args[4]);
	
			GameController.Instance.levelsManager.LoadAndStartNewLevel(levelLoadParams);
			return null;
		}, false);
		// story.BindExternalFunction("StartScene", (InkListItem sceneId, string titleText, string dataText, int slotCount, InkList startingItems) => {
		// 	GameController.Instance.sceneController.StartScene(sceneId, titleText, dataText, slotCount, startingItems);
		// });
		// story.BindExternalFunction ("Testing", () => false);
	}
	
	// Event triggered when the story has an error.
	void OnStoryError (string message, Ink.ErrorType type) {
		if(type == Ink.ErrorType.Author) {
			DebugX.Log("Ink Note: "+message);
		} else if(type == Ink.ErrorType.Warning) {
			DebugX.LogWarning("Ink Warning: "+message);
		} else if(type == Ink.ErrorType.Error) {
			DebugX.LogError("Ink Error: "+message);
		}
	}





	public static T TryCast<T>(object value) {
		if (value is T value1) return value1;
		else {
			Debug.LogError ("Failed to cast " + value.GetType ().Name + " to " + typeof(T).Name);
			return default;
		}
	}

	public static object TryCoerce<T>(object value) {
		if (value == null)
			return null;

		if (value is T)
			return (T) value;

		if (value is float && typeof(T) == typeof(int)) {
			int intVal = (int)Math.Round ((float)value);
			return intVal;
		}

		if (value is int && typeof(T) == typeof(float)) {
			float floatVal = (float)(int)value;
			return floatVal;
		}

		if (value is int && typeof(T) == typeof(bool)) {
			int intVal = (int)value;
			return intVal == 0 ? false : true;
		}

		if (value is bool && typeof(T) == typeof(int)) {
			bool boolVal = (bool)value;
			return boolVal ? 1 : 0;
		}

		if (typeof(T) == typeof(string)) {
			return value.ToString ();
		}

		Debug.LogError ("Failed to cast " + value.GetType ().Name + " to " + typeof(T).Name);

		return null;
	}
	

	// void MakeChoice(Choice choice) {
	// 	story.ChooseChoiceIndex(choice.index);
	// 	story.ContinueMaximally();
	// }
	//
	// void AdvanceContent() {
	// 	if(story.canContinue) {
	// 		story.Continue();
	// 		ParseCurrentInstruction();
	// 	} else if(!hasParsedChoices) {
	// 		ParseChoices();
	// 		if(currentChoiceInstructions.Count() == 0) {
	// 			Debug.LogWarning("Story has no choices and no content!");
	// 		}
	// 	} else {
	// 		Debug.LogWarning("Story has no choices and no content!");
	// 	}
	// }
	//
	// public void ParseCurrentInstruction () {
	// 	string text = story.currentText;
	// 	text = text.Trim();
	// 	// If the line is blank just continue and pretend it was never there.
	// 	if(string.IsNullOrWhiteSpace(text)) {
	// 		AdvanceContent();
	// 	} else {
	// 		currentInstruction = null;
	// 		BaseContentInstruction.TryParse(text, story.currentTags, out currentInstruction);
	// 		if(!(currentInstruction is DebuggingInstruction))
	// 			GameTime.Instance.UpdateTime();
	// 		if(OnParseContent != null) OnParseContent(currentInstruction);
	// 	}
	// }
	// public void ParseChoices () {
	// 	currentChoiceInstructions.Clear();
	// 	if (story.currentChoices.Count > 0) {
	// 		for(int i = 0; i < story.currentChoices.Count; i++) {
	// 			currentChoiceInstructions.Add(BaseChoiceInstruction.ParseChoice(story.currentChoices[i]));
	// 		}
	// 	}
	// 	hasParsedChoices = true;
	// 	GameTime.Instance.UpdateTime();
	// 	if(OnParseChoices != null) OnParseChoices(currentChoiceInstructions);
	// }
}