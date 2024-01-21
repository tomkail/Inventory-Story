using System.Text.RegularExpressions;
using System.Collections.Generic;

[System.Serializable]
public class RunFunctionInstruction : ScriptContent {
    static Regex _parserRegex;
	static Regex parserRegex {
		get {
			if(_parserRegex == null) _parserRegex = new Regex(InkParserUtility.BuildInstructionPrefixRegex("RUN"), RegexOptions.IgnoreCase);
			return _parserRegex;
		}
	}
	public static RunFunctionInstruction TryParse (string rawContent, List<string> tags) {
        var match = parserRegex.Match (rawContent);
		if(!match.Success) return null;
        var arguments = InkParserUtility.ParseArguments(match.Groups[1].Value);
		if(!InvokeMethodParams.TryParse(arguments, out var invokeMethodParams)) return null;
        var instruction = new RunFunctionInstruction();
		instruction.invokeMethodParams = invokeMethodParams;

		var cssArguments = InkParserUtility.ParseCSSStyleArguments(tags);
		instruction.ParseDefaultCSSArguments(cssArguments);

		return instruction;
	}

    public InvokeMethodParams invokeMethodParams;
}