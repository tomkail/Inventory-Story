using System.Text.RegularExpressions;

[System.Serializable]
public class SaveInstruction : ScriptContent {
	static Regex _parserRegex;
	static Regex parserRegex {
		get {
			if(_parserRegex == null) _parserRegex = new Regex(InkParserUtility.BuildInstructionPrefixRegex("SAVE"), RegexOptions.IgnoreCase);
			return _parserRegex;
		}
	}
	public static SaveInstruction TryParse (string rawContent) {
		var match = parserRegex.Match (rawContent);
		if(!match.Success) return null;        
        return new SaveInstruction();
	}
}