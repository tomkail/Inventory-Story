using System.Text.RegularExpressions;
using Ink.Runtime;

// This works just like RunFunctionInstruction, except it's shown as a choice.
[System.Serializable]
public class RunFunctionInstructionChoice : ScriptChoice {
    static Regex _parserRegex;
	static Regex parserRegex => _parserRegex ??= new Regex(InkParserUtility.BuildInstructionPrefixRegex("RUN"), RegexOptions.IgnoreCase);

	public static RunFunctionInstructionChoice TryParse (Choice choice) {
        var match = parserRegex.Match (choice.text);
		if(!match.Success) return null;
        var arguments = InkParserUtility.ParseArguments(match.Groups[1].Value);
        if(!InvokeMethodParams.TryParse(arguments, out var invokeMethodParams)) return null;
        var instruction = new RunFunctionInstructionChoice(choice);
        instruction.invokeMethodParams = invokeMethodParams;
        if(!arguments.TryGetRequiredValue("text", ref instruction.text)) return null;
        arguments.TryGetValue("buttonStyle", ref instruction.buttonStyle);
        return instruction;
	}

    public string text;
    public ButtonStyle buttonStyle = ButtonStyle.Default;

    public InvokeMethodParams invokeMethodParams;

	public RunFunctionInstructionChoice (Choice storyChoice) : base(storyChoice) {}
}
