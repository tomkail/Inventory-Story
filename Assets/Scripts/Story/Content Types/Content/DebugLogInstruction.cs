using System.Text.RegularExpressions;

[System.Serializable]
public class DebugLogInstruction : ScriptContent {
	static Regex _parserRegex;
	static Regex parserRegex {
		get {
			if(_parserRegex == null) _parserRegex = new Regex(InkParserUtility.BuildInstructionPrefixRegex("LOG", false), RegexOptions.IgnoreCase);
			return _parserRegex;
		}
	}

	public static DebugLogInstruction TryParse (string rawContent) {
		var match = parserRegex.Match (rawContent);
		if(!match.Success) return null;
        var arguments = InkParserUtility.ParseArguments(match.Groups[1].Value);
        var instruction = new DebugLogInstruction() {
			log = match.Groups[1].Value
		};
		return instruction;
	}
	
	public string log;

	public override string ToString () {
		return string.Format ("["+GetType().Name+"] log {0}", log);
	}
}