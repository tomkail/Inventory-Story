using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[System.Serializable]
public class BackgroundInstruction : ScriptContent {
	static Regex _parserRegex;
	static Regex parserRegex => _parserRegex ??= new Regex(InkParserUtility.BuildInstructionPrefixRegex("BACKGROUND"), RegexOptions.IgnoreCase);

	public static BackgroundInstruction TryParse (string rawContent, List<string> tags) {
		var match = parserRegex.Match (rawContent);
		if(!match.Success) return null;
        var arguments = InkParserUtility.ParseArguments(match.Groups[1].Value);
        var instruction = new BackgroundInstruction();
		instruction.ParseDefaultArguments(arguments);
        
		// If not a video, get the path/url in case it's an image or something else.
		arguments.TryGetValue("path", ref instruction.assetPath);

        arguments.TryGetValue("color", ref instruction.color);
        instruction.useGradient = InkParserUtility.TryParseGradient(arguments, out instruction.gradient);
        
		var cssArguments = InkParserUtility.ParseCSSStyleArguments(tags);
        cssArguments.TryGetValue("color", ref instruction.color);
		
		return instruction;
	}

    public string assetPath;
    public Color color = Color.white;
    
    // This is necessary because gradient is un-nulled on being serialized.
    public bool useGradient;
    public Gradient gradient = null;
}