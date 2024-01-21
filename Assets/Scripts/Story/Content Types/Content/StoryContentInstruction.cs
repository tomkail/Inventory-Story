using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class StoryContentInstruction : ScriptContent {
    public string text;
    public string textStyle;
    public TextAlignmentOptions? textAlignment;
    public Color? color;
	
	public static int parserSortPriority => int.MinValue;
	
    public static StoryContentInstruction TryParse (string rawContent, List<string> tags) {
        var instruction = new StoryContentInstruction();
		
		instruction.text = rawContent;
		
		var cssArguments = InkParserUtility.ParseCSSStyleArguments(tags);
		instruction.ParseDefaultCSSArguments(cssArguments);
		cssArguments.TryGetValue("text-style", ref instruction.textStyle);
		cssArguments.TryGetValue("text-align", ref instruction.textAlignment);
		cssArguments.TryGetValue("color", ref instruction.color);
		return instruction;
	}
}
