using UnityEngine;
using System.Text.RegularExpressions;
using Ink.Runtime;

[System.Serializable]
public class ButtonChoiceInstruction : ScriptChoice {
	public static int parserSortPriority => int.MinValue;
	public static ButtonChoiceInstruction TryParse (Choice choice) {
		if(string.IsNullOrWhiteSpace(choice.text)) {
			return null;
		}
		var arguments = InkParserUtility.ParseArguments(choice.text);
		var instruction = new ButtonChoiceInstruction(choice);
        instruction.text = arguments.textBeforeArguments;
    	arguments.TryGetValue("buttonStyle", ref instruction.buttonStyle);
    	arguments.TryGetValue("stylesheet", ref instruction.stylesheetOverride);
        return instruction;
	}

    public string text;
    public ButtonStyle buttonStyle = ButtonStyle.Default;
    public bool disabled;

	public ButtonChoiceInstruction (Choice storyChoice) : base(storyChoice) {}

	public override string ToString () {
		return string.Format ("["+GetType().Name+"] text {0}", text);
	}
}
public enum ButtonStyle {
	Default,
	Primary,
	Secondary
}