using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Ink.Runtime;

[System.Serializable]
public abstract class ScriptChoice : ScriptLine {
	public Choice storyChoice;

	public ScriptChoice (Choice storyChoice) : base() {
		this.storyChoice = storyChoice;
	}
	
	public override string ToString () {
		string s = "["+this.GetType()+"]\n";
		return s;
	}

	
	static bool foundParsers = false;
	static List<ParserScriptInfo> parseMethodList;
	public static IEnumerable<ParserScriptInfo> parseMethods {
		get {
			if(parseMethodList == null) GetParsers();
			return parseMethodList;
		}
	}
	

	/// <summary>
	/// Turns Ink.Runtime.Choice into ScriptChoice.
	/// </summary>
	/// <returns>The choice content.</returns>
	/// <param name="choice">Choice.</param>
	/// <param name="choiceIndex">Choice index.</param>
	public static ScriptChoice ParseChoice (Choice choice) {
		if(choice == null) {
			// Debug.LogError("choice is null. This is not allowed.");
			return null;
		}
		
		var choiceParams = new object[] {choice};
		foreach(var parseMethodInfo in parseMethods) {
			var parsed = parseMethodInfo.parseMethod.Invoke(null, choiceParams);
			if(parsed != null) {
				return (ScriptChoice) parsed;
			}
		}
		return null;
	}
	
	public static bool TryParse (Choice choice, out ScriptChoice scriptChoice) {
		scriptChoice = ParseChoice(choice);
		if(scriptChoice == null) {
			// Debug.LogWarning("Unexpected text in ink choice with no particular markup: "+choice.text);
			return false;
		} else
			return true;
	}

	/// <summary>
	/// Gets all the choice parser methods in the assembly via reflection. This means that we don't need to manually add them to a list.
	/// ScriptContent contains a similar method for content.
	/// Note code stripping would exclude parser methods from builds; the InkParserGenerateLinkXml class creates a link.xml file at build time to prevent this.
	/// </summary>
	static void GetParsers () {
		if(foundParsers) return;
		parseMethodList = new List<ParserScriptInfo>();
		var derivedClassList = Assembly.GetAssembly(typeof(ScriptChoice)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(ScriptChoice))).ToList();
		foreach(var type in derivedClassList) {
			ParserScriptInfo parserScript = new ParserScriptInfo(type);
			
			var parameters = parserScript.parseMethod.GetParameters();
			if (!(parameters.Length == 1 && parameters[0].ParameterType == typeof(Choice))) {
				Debug.LogError("Parser script for " + type.Name + " has the wrong signature. It should be either TryParse(string, List<string>) or TryParse(string)");
				continue;
			}
			parseMethodList.Add(parserScript);
		}
		parseMethodList = parseMethodList.OrderByDescending(x => x.priority).ToList();
		foundParsers = true;
	}
}