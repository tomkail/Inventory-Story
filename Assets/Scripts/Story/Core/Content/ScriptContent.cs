using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

/// <summary>
/// Script content.
/// </summary>
[System.Serializable]
public abstract class ScriptContent : ScriptLine {
	public bool useDelay;
	// If set to -1, an automatic delay time is used instead, using the estimated "read time" of the previous content.
	public bool useAutomaticDelay => useDelay && delay == -1;
	public bool useDefinedDelay => useDelay && !useAutomaticDelay;
	public float delay;

	public float fadeInTime;
	public float extraSpacing;
	
	public ScriptContent () : base () {}
	public ScriptContent (ScriptContent toClone) : base (toClone) {
        this.useDelay = toClone.useDelay;
        this.delay = toClone.delay;
        this.fadeInTime = toClone.fadeInTime;
        this.extraSpacing = toClone.extraSpacing;
	}

	protected override void ParseDefaultCSSArguments (InkParserUtility.ParsedArgumentCollection cssArguments) {
		useDelay = cssArguments.ContainsKey("delay");
		if(useDelay && !cssArguments.TryGetValue("delay", ref delay)) delay = -1;
		fadeInTime = 0.2f;
		cssArguments.TryGetValue("fadeInTime", ref fadeInTime);
		cssArguments.TryGetValue("extraSpacing", ref extraSpacing);
		base.ParseDefaultCSSArguments(cssArguments);
	}


	public float CalculateDelay () {
		if(!useDelay) return 0;
		else if (delay > 0) return delay;
		else return CalculateReadTime();
	}

	// How long it'll take to "read" this piece of content. This can then be used for automatic delay times.
	public virtual float CalculateReadTime () {
		return 0.5f;
	}

	public class ScriptContentParserScriptInfo : ParserScriptInfo {
		public bool expectsTags;
		public ScriptContentParserScriptInfo (Type parserType) : base (parserType) {}
	}
	
	static bool foundParsers = false;
	static List<ScriptContentParserScriptInfo> parseMethodList;
	public static IEnumerable<ScriptContentParserScriptInfo> parseMethods {
		get {
			if(parseMethodList == null) GetParsers();
			return parseMethodList;
		}
	}
	
	/// <summary>
	/// Turns raw content into ScriptContent.
	/// </summary>
	/// <returns>The content.</returns>
	/// <param name="rawContent">Raw content.</param>
	public static bool TryParse (string rawContent, List<string> tags, out ScriptContent scriptContent) {
		scriptContent = null;
		if(rawContent == null) {
			Debug.LogError("rawContent is null. This is not allowed.");
			return false;
		}
		
		var contentAndTags = new object[] {rawContent, tags};
		var contentOnly = new object[] {rawContent};
		foreach(var parseMethodInfo in parseMethods) {
			if (parseMethodInfo.expectsTags) {
				var parsed = parseMethodInfo.parseMethod.Invoke(null, contentAndTags);
				if(parsed != null) {
					scriptContent = (ScriptContent) parsed;
					return true;
				}
			} else {
				var parsed = parseMethodInfo.parseMethod.Invoke(null, contentOnly);
				if(parsed != null) {
					scriptContent = (ScriptContent) parsed;
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Gets all the content parser methods in the assembly via reflection. This means that we don't need to manually add them to a list.
	/// /// ScriptChoice contains a similar method for choices.
	/// Note code stripping would exclude parser methods from builds; the InkParserGenerateLinkXml class creates a link.xml file at build time to prevent this.
	/// </summary>
	static void GetParsers () {
		if(foundParsers) return;
		parseMethodList = new List<ScriptContentParserScriptInfo>();
		var derivedClassList = Assembly.GetAssembly(typeof(ScriptContent)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(ScriptContent))).ToList();
		foreach(var type in derivedClassList) {
			ScriptContentParserScriptInfo parserScript = new ScriptContentParserScriptInfo(type);

			var parameters = parserScript.parseMethod.GetParameters();
			if (parameters.Length == 2 && parameters[0].ParameterType == typeof(string) && parameters[1].ParameterType == typeof(List<string>)) {
				parserScript.expectsTags = true;
			} else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string)) {
				parserScript.expectsTags = false;
			} else {
				Debug.LogError("Parser script for " + type.Name + " has the wrong signature. It should be either TryParse(string, List<string>) or TryParse(string)");
				continue;
			}
			parseMethodList.Add(parserScript);
		}
		parseMethodList = parseMethodList.OrderByDescending(x => x.priority).ToList();
		foundParsers = true;
	}
}