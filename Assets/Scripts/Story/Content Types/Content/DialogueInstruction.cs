using TMPro;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueInstruction : ScriptContent {
	public string speaker;
	public string text;

    public string textStyle;
    public TextAlignmentOptions? textAlignment;
    public Color? color;
	
	public static int parserSortPriority => int.MinValue;
	
    public static DialogueInstruction TryParse (string rawContent, List<string> tags) {
        var instruction = new DialogueInstruction();
		
        if(!TryParseDialogue(rawContent, out string speaker, out string text)) return null;

        if(text.IsNullOrEmpty()) return null;

		instruction.speaker = speaker;
		instruction.text = text;
		
		var cssArguments = InkParserUtility.ParseCSSStyleArguments(tags);
		instruction.ParseDefaultCSSArguments(cssArguments);
		cssArguments.TryGetValue("text-style", ref instruction.textStyle);
		cssArguments.TryGetValue("text-align", ref instruction.textAlignment);
		cssArguments.TryGetValue("color", ref instruction.color);
		return instruction;
	}
    
    
    // Parses times in the format
    // 10:23
    // Does not parse AM or PM following the time.
    public const string digitalTimeRegex = @"([0-1]?[0-9]|2[0-3]):[0-5][0-9]";
    static Regex _digitalTimeRegexParser;
    public static Regex digitalTimeRegexParser {
	    get {
		    if(_digitalTimeRegexParser == null) _digitalTimeRegexParser = new Regex(digitalTimeRegex);
		    return _digitalTimeRegexParser;
	    }
    }

    // Parses dialogue in the format
    // El: Six, are you sure this is where you intended to land?
    // Extracting the speaker before the first colon and the content after it.
    public const string dialogueRegex = @"([^:]*)(?:\s*:?\s*)(.*)";
    static Regex _dialogueRegexParser;
    public static Regex dialogueRegexParser {
	    get {
		    if(_dialogueRegexParser == null) _dialogueRegexParser = new Regex(dialogueRegex);
		    return _dialogueRegexParser;
	    }
    }
    public static bool TryParseDialogue (string rawContent, out string speaker, out string content) {
	    speaker = null;
	    content = null;
        
	    if(rawContent == null) return false;
	    var match = dialogueRegexParser.Match (rawContent);
	    if(!match.Success) return false;

	    // Text in the format "Wait for 10:00am" would be parsed as dialogue since it contains a :
	    // If we match a time, return false.
	    var timeMatch = digitalTimeRegexParser.Match (rawContent);
	    if(timeMatch.Success) return false;
        
	    speaker = match.Groups[1].Value.Trim();
	    content = match.Groups[2].Value.Trim();
	    return true;
    }
}