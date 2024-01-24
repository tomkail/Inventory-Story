using System;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class StoryController : MonoSingleton<StoryController> {
	public Story story {get; private set;}
	public bool hasStory => story != null;
	public bool begun { get; private set; }

	public List<ScriptContent> contents = new List<ScriptContent>();
	public List<ScriptChoice> choices = new List<ScriptChoice>();
	public static event Action<Story> OnCreateStory;
	public event Action OnParsedInstructions;

	public void InitStory (TextAsset storyJSONAsset, string storyStateJSON = null) {
		if(hasStory) DebugX.LogError("InitStory called but hasStory is true! This should never occur."); 
		
		story = new Story(storyJSONAsset.text);
		
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
		BindExternalFunctions();
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