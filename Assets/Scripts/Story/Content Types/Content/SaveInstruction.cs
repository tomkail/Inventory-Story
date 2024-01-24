using System.Collections.Generic;
using System.Text.RegularExpressions;

[System.Serializable]
public class SaveInstruction : ScriptContent {
	static Regex _parserRegex;
	static Regex parserRegex => _parserRegex ??= new Regex(InkParserUtility.BuildInstructionPrefixRegex("Save"), RegexOptions.IgnoreCase);

	public static SaveInstruction TryParse (string rawContent, List<string> tags) {
		var match = parserRegex.Match (rawContent);
		if(!match.Success) return null;
		return new SaveInstruction();
	}
}