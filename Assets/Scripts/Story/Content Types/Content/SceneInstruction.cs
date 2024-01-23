using System.Collections.Generic;
using System.Text.RegularExpressions;

[System.Serializable]
public class SceneInstruction : ScriptContent {
	static Regex _parserRegex;
	static Regex parserRegex => _parserRegex ??= new Regex(InkParserUtility.BuildInstructionPrefixRegex("Scene"), RegexOptions.IgnoreCase);

	public static SceneInstruction TryParse (string rawContent, List<string> tags) {
		var match = parserRegex.Match (rawContent);
		if(!match.Success) return null;
		var arguments = InkParserUtility.ParseArguments(match.Groups[1].Value);
		var instruction = new SceneInstruction();
		instruction.ParseDefaultArguments(arguments);
        
		arguments.TryGetValue("sceneName", ref instruction.text);
		
		return instruction;
	}

	public string text;
}