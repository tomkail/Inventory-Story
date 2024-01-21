using System.Collections.Generic;
using System.Text.RegularExpressions;

[System.Serializable]
public class TitleInstruction : ScriptContent {
    static Regex _parserRegex;
    static Regex parserRegex => _parserRegex ??= new Regex(InkParserUtility.BuildInstructionPrefixRegex("TITLE"), RegexOptions.IgnoreCase);

    public static TitleInstruction TryParse (string rawContent, List<string> tags) {
        var match = parserRegex.Match (rawContent);
        if(!match.Success) return null;
        var arguments = InkParserUtility.ParseArguments(match.Groups[1].Value);
        var instruction = new TitleInstruction();
        instruction.ParseDefaultArguments(arguments);
        
        arguments.TryGetValue("text", ref instruction.text);
		
        return instruction;
    }

    public string text;
}